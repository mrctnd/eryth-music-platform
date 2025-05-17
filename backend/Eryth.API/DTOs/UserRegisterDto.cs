using System.ComponentModel.DataAnnotations;

namespace Eryth.API.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Görünen ad 3 ile 100 karakter arasında olmalıdır.")]
        public string DisplayName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        // Şifre karmaşıklığı kuralları eklenebilir (örn: büyük harf, küçük harf, sayı, özel karakter)
        public string Password { get; set; } = null!;
    }
}