using Eryth.API.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql; // Global ENUM eşleştirmesi için gerekebilir (ileride bakacağız)
using Eryth.API.Models; // ENUM'ları burada eşlemek için gerekirse
using Eryth.API.Services; // IAuthService ve AuthService için
using Microsoft.AspNetCore.Authentication.JwtBearer; // JwtBearerDefaults için
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey için
using System.Text;
using Microsoft.OpenApi.Models; // Swagger/OpenAPI için 
using Eryth.API.Middleware; // GlobalExceptionMiddleware için
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/eryth-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // Seq sunucusu adresi
    .CreateLogger();

builder.Host.UseSerilog();

// --- Servislerin Konteyner'a Eklenmesi ---

// 1. Veritabanı bağlantı dizesini al
var connectionString = builder.Configuration.GetConnectionString("ErythDbConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'ErythDbConnection' not found.");
}

// 2. ErythDbContext'i PostgreSQL için yapılandır ve servislere ekle
builder.Services.AddDbContext<ErythDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptionsAction: sqlOptions =>
    {
        // Opsiyonel: SQL oluşturma seçenekleri (örn: RetryOnFailure)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    })
    // PostgreSQL'de tablo ve kolon isimlerinin snake_case (örn: user_id) olmasını sağlar
    // C# entity ve property isimleri PascalCase (örn: UserId) iken.
    .UseSnakeCaseNamingConvention()
    // Geliştirme sırasında hassas verilerin loglanmasını etkinleştir (production'da kapatılmalı)
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
);


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
// Eğer IFileStorageService gibi ayrı bir servisiniz olursa onu da kaydedin:
// builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// 5. JWT Kimlik Doğrulama Servislerini Ekle
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT SecretKey appsettings.json dosyasında bulunamadı veya boş olamaz.");
    }
    if (secretKey.Length < 32)
    {
        throw new InvalidOperationException("JWT SecretKey en az 32 karakter uzunluğunda olmalıdır.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Issuer'ı doğrula
        ValidateAudience = true, // Audience'ı doğrula
        ValidateLifetime = true, // Token'ın süresinin dolup dolmadığını doğrula
        ValidateIssuerSigningKey = true, // İmza anahtarını doğrula

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// ÖNEMLİ: PostgreSQL ENUM Eşleştirmesi (Gerekirse Uygulama Başlangıcında)
// Eğer DbContext.OnModelCreating içinde modelBuilder.HasPostgresEnum<MyEnum>() kullanmıyorsak
// veya Npgsql'in otomatik eşleştirmesi yeterli olmazsa, burada global bir eşleştirme yapabiliriz.
// Bu genellikle NpgsqlDataSourceBuilder üzerinden yapılır.
// Örnek:
// var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
// dataSourceBuilder.MapEnum<UserStatus>("user_status"); // "user_status" veritabanındaki enum adıdır
// dataSourceBuilder.MapEnum<UserRole>("user_role");
// // ... diğer tüm enumlar için ...
// var dataSource = dataSourceBuilder.Build();
// builder.Services.AddDbContext<ErythDbContext>(options =>
//     options.UseNpgsql(dataSource, ...) // dataSource kullanılır
//     .UseSnakeCaseNamingConvention()
// );
// Şimdilik Npgsql'in varsayılan davranışlarına ve OnModelCreating'deki potansiyel HasPostgresEnum çağrılarına güveneceğiz.
// Sorun çıkarsa bu kısma geri döneriz.

builder.Services.AddAuthorization();
// 3. API Controller'larını ekle
builder.Services.AddControllers();

// 4. Swagger/OpenAPI yapılandırması (API dokümantasyonu ve testi için)
builder.Services.AddEndpointsApiExplorer(); // Minimal API'ler için Endpoint Metadata'sını keşfeder
builder.Services.AddSwaggerGen(options =>
{
    // JWT Bearer token ile Swagger UI'da yetkilendirme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer ' ve ardından token'ınızı girin (Örn: Bearer eyJhbGciOiJI...) ",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey, // ApiKey olarak tanımlıyoruz ama Bearer şemasını kullanacak
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Yukarıdaki AddSecurityDefinition'daki "Bearer" ile aynı olmalı
                }
            },
            new string[] {} // Kapsamlar (scopes), şimdilik boş
        }
    });
    // Diğer SwaggerGen seçenekleri (varsa)
});

// CORS servisini ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
    // Production için örnek:
    // options.AddPolicy("ProdCorsPolicy", builder =>
    // {
    //     builder.WithOrigins("https://senin-frontend-domainin.com")
    //            .AllowAnyHeader()
    //            .AllowAnyMethod();
    // });
});

// Rate Limiting servisini ekle
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // Dakikada 5 istek
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        ));
    options.RejectionStatusCode = 429;
});

// Response Compression servisini ekle
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "Database");

// --- Uygulamanın İnşa Edilmesi ---
var app = builder.Build();

// --- HTTP Request Pipeline'ının Yapılandırılması ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Geliştirme sırasında detaylı hata sayfaları
    // Geliştirme ortamında CORS'u aktif et
    app.UseCors("DevCorsPolicy");
}
else
{
    // Production ortamında daha genel bir hata handler'ı
    app.UseExceptionHandler("/Error"); // Örnek, özel bir endpoint'e yönlendirebilirsiniz
    app.UseHsts();
    // Production için CORS'u aktif et (örnek)
    // app.UseCors("ProdCorsPolicy");

    // Swagger UI'ya erişimi kimliği doğrulanmış kullanıcılara aç
    app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger"), subApp =>
    {
        subApp.UseAuthentication();
        subApp.UseAuthorization();
        subApp.Use(async (context, next) =>
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Swagger UI sadece yetkili kullanıcılar için erişilebilir.");
                return;
            }
            await next();
        });
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Response Compression middleware'ini ekle (UseHttpsRedirection'dan önce)
app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseStaticFiles(); // wwwroot klasöründeki dosyaların sunulmasını sağlar

// Global Exception Middleware'i ekle
app.UseMiddleware<GlobalExceptionMiddleware>();

// Rate Limiting middleware'i ekle
app.UseRateLimiter();

// ÖNEMLİ: Kimlik Doğrulama ve Yetkilendirme Middleware'leri
// JWT token tabanlı kimlik doğrulamayı eklediğimizde bu satırların yorumunu kaldıracağız.
app.UseAuthentication(); // Kimin istek attığını belirler
app.UseAuthorization();  // İstek atanın ne yapmaya yetkili olduğunu kontrol eder

// API Controller endpoint'lerini haritala
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Prometheus metrikleri
app.UseMetricServer(); // /metrics endpointi
app.UseHttpMetrics(); // HTTP istek metrikleri

app.Run();