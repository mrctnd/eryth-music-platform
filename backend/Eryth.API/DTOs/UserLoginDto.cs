using System.ComponentModel.DataAnnotations;

namespace Eryth.API.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adı veya e-posta gereklidir.")]
        [StringLength(255)]
        public string LoginCredential { get; set; } = null!; // Username or Email

        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; } = null!;
    }
}