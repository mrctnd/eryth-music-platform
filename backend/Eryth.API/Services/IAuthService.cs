using Eryth.API.DTOs;
using Eryth.API.Models; // User entity'si için
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string? ErrorMessage, User? User)> RegisterUserAsync(UserRegisterDto registerDto);
        Task<(bool Succeeded, string? ErrorMessage, AuthResponseDto? AuthResponse)> LoginUserAsync(UserLoginDto loginDto);
        // string GenerateJwtToken(User user); // Token oluşturma bu serviste veya ayrı bir TokenService'te olabilir
    }
}