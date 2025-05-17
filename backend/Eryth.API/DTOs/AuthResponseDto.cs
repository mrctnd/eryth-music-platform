namespace Eryth.API.DTOs
{
    public class AuthResponseDto
    {
    public string UserId { get; set; } = null!; // String olarak tutmak daha esnek olabilir (BIGSERIAL veya UUID için)
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime TokenExpiration { get; set; }
    // public string? RefreshToken { get; set; } // İleride refresh token eklenirse
    }
}