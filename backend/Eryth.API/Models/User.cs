using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // [Key], [Required] vb. için
using System.ComponentModel.DataAnnotations.Schema; // [Table] vb. için (opsiyonel)

namespace Eryth.API.Models
{
    // [Table("Users")] // Tablo adı C# sınıf adıyla aynıysa bu genellikle gereksizdir.
    public class User
    {
        [Key] // Primary Key olduğunu belirtir
        public long UserId { get; set; }

        [Required] // Zorunlu alan
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [EmailAddress] // E-posta formatını doğrular (ASP.NET Core tarafında)
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string? ProfilePictureUrl { get; set; }
        public string? CoverPhotoUrl { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? WebsiteUrl { get; set; }

        public UserRole Role { get; set; } = UserRole.User;
        public bool EmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public UserStatus Status { get; set; } = UserStatus.PendingVerification;

        [Column(TypeName = "jsonb")] // PostgreSQL'e özgü tip için
        public string? PrivacySettings { get; set; } // JSON string olarak tutulacak

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (İlişkisel özellikler)
        // Bir kullanıcının yüklediği müzikler
        public virtual ICollection<Music> UploadedMusics { get; set; } = new List<Music>();
        // Bir kullanıcının oluşturduğu albümler
        public virtual ICollection<Album> CreatedAlbums { get; set; } = new List<Album>();
        // Bir kullanıcının oluşturduğu çalma listeleri
        public virtual ICollection<Playlist> CreatedPlaylists { get; set; } = new List<Playlist>();
        // Bir kullanıcının yaptığı beğeniler
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        // Bir kullanıcının takip ettikleri (FollowerUserID = this.UserId)
        public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
        // Bir kullanıcının takipçileri (FollowingUserID = this.UserId)
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
        // Bir kullanıcının yaptığı yorumlar
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // Bir kullanıcının kaydettiği öğeler
        public virtual ICollection<Save> Saves { get; set; } = new List<Save>();
         // Bir kullanıcının gönderdiği mesajlar
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        // Bir kullanıcının dahil olduğu sohbetler (ChatParticipants üzerinden)
        public virtual ICollection<ChatParticipant> ChatParticipations { get; set; } = new List<ChatParticipant>();
        // Bir kullanıcının aldığı bildirimler
        public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
        // Bir kullanıcının tetiklediği bildirimler
        public virtual ICollection<Notification> TriggeredNotifications { get; set; } = new List<Notification>();
        // Bir kullanıcının aktivite logları
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();
        // Bir kullanıcının oturumları
        public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        // Bir kullanıcının sosyal medya bağlantıları
        public virtual ICollection<UserAuthenticationProvider> AuthProviders { get; set; } = new List<UserAuthenticationProvider>();
        // Bir kullanıcının çalma listesi ortaklıkları
        public virtual ICollection<PlaylistCollaborator> PlaylistCollaborations { get; set; } = new List<PlaylistCollaborator>();
        // Bir kullanıcının çalma listesine eklediği şarkılar (PlaylistMusics.AddedByUserID)
        public virtual ICollection<PlaylistMusic> AddedPlaylistMusics { get; set; } = new List<PlaylistMusic>();
    }
}