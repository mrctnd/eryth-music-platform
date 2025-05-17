namespace Eryth.API.DTOs
{
    public class UserSummaryDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
    }
}