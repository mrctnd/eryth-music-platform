using Eryth.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Eryth.API.Data
{
    public class ErythDbContext : DbContext
    {
        public ErythDbContext(DbContextOptions<ErythDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Mood> Moods { get; set; } = null!;
        public DbSet<UserDefinedTag> UserDefinedTags { get; set; } = null!;
        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Music> Musics { get; set; } = null!;
        public DbSet<MusicMood> MusicMoods { get; set; } = null!;
        public DbSet<AlbumMusic> AlbumMusics { get; set; } = null!;
        public DbSet<MusicUserDefinedTag> MusicUserDefinedTags { get; set; } = null!;
        public DbSet<UserAuthenticationProvider> UserAuthenticationProviders { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistMusic> PlaylistMusics { get; set; } = null!;
        public DbSet<PlaylistCollaborator> PlaylistCollaborators { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        public DbSet<Save> Saves { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<ChatParticipant> ChatParticipants { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<UserActivityLog> UserActivityLogs { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                entity.HasMany(u => u.UploadedMusics).WithOne(m => m.UploaderUser).HasForeignKey(m => m.UploaderUserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.CreatedAlbums).WithOne(a => a.CreatorUser).HasForeignKey(a => a.CreatorUserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.CreatedPlaylists).WithOne(p => p.CreatorUser).HasForeignKey(p => p.CreatorUserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.Likes).WithOne(l => l.User).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.Saves).WithOne(s => s.User).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.SentMessages).WithOne(m => m.SenderUser).HasForeignKey(m => m.SenderUserId).OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(u => u.ReceivedNotifications).WithOne(n => n.ReceiverUser).HasForeignKey(n => n.ReceiverUserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.TriggeredNotifications).WithOne(n => n.ActorUser).HasForeignKey(n => n.ActorUserId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                
                entity.HasMany(u => u.ActivityLogs).WithOne(al => al.User).HasForeignKey(al => al.UserId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(u => u.Sessions).WithOne(us => us.User).HasForeignKey(us => us.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.AuthProviders).WithOne(ap => ap.User).HasForeignKey(ap => ap.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.AddedPlaylistMusics).WithOne(pm => pm.AddedByUser).HasForeignKey(pm => pm.AddedByUserId).OnDelete(DeleteBehavior.Cascade); 
                entity.HasMany(u => u.PlaylistCollaborations).WithOne(pc => pc.User).HasForeignKey(pc => pc.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.ChatParticipations).WithOne(cp => cp.User).HasForeignKey(cp => cp.UserId).OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(g => g.Slug).IsUnique();
                entity.HasOne(g => g.ParentGenre).WithMany(g => g.SubGenres).HasForeignKey(g => g.ParentGenreId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(g => g.Musics).WithOne(m => m.Genre).HasForeignKey(m => m.GenreId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(g => g.SubGenreMusics).WithOne(m => m.SubGenre).HasForeignKey(m => m.SubGenreId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(g => g.Albums).WithOne(a => a.Genre).HasForeignKey(a => a.GenreId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Mood>(entity =>
            {
                entity.HasIndex(m => m.Slug).IsUnique();
            });

            modelBuilder.Entity<UserDefinedTag>(entity =>
            {
                entity.HasIndex(t => t.Slug).IsUnique();
            });

            modelBuilder.Entity<AlbumMusic>(entity =>
            {
                entity.HasIndex(am => new { am.AlbumId, am.MusicId }).IsUnique();
                entity.HasIndex(am => new { am.AlbumId, am.DiscNumber, am.TrackNumber }).IsUnique();
                entity.HasOne(am => am.Album).WithMany(a => a.AlbumMusics).HasForeignKey(am => am.AlbumId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(am => am.Music).WithMany(m => m.AlbumMusics).HasForeignKey(am => am.MusicId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MusicMood>(entity =>
            {
                entity.HasIndex(mm => new { mm.MusicId, mm.MoodId }).IsUnique();
                entity.HasOne(mm => mm.Music).WithMany(m => m.MusicMoods).HasForeignKey(mm => mm.MusicId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(mm => mm.Mood).WithMany(m => m.MusicMoods).HasForeignKey(mm => mm.MoodId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MusicUserDefinedTag>(entity =>
            {
                entity.HasIndex(mt => new { mt.MusicId, mt.TagId }).IsUnique();
                entity.HasOne(mt => mt.Music).WithMany(m => m.MusicUserDefinedTags).HasForeignKey(mt => mt.MusicId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(mt => mt.UserDefinedTag).WithMany(t => t.MusicUserDefinedTags).HasForeignKey(mt => mt.TagId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserAuthenticationProvider>(entity =>
            {
                entity.HasIndex(uap => new { uap.UserId, uap.ProviderName }).IsUnique();
                entity.HasIndex(uap => new { uap.ProviderName, uap.ProviderUserId }).IsUnique();
            });

            modelBuilder.Entity<PlaylistMusic>(entity =>
            {
                entity.HasIndex(pm => new { pm.PlaylistId, pm.MusicId }).IsUnique();
                entity.HasOne(pm => pm.Playlist).WithMany(p => p.PlaylistMusics).HasForeignKey(pm => pm.PlaylistId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pm => pm.Music).WithMany(m => m.PlaylistMusics).HasForeignKey(pm => pm.MusicId).OnDelete(DeleteBehavior.Cascade);
                // AddedByUserId ilişkisi User entity'sinde .WithMany(u => u.AddedPlaylistMusics) ile kuruldu.
            });

            modelBuilder.Entity<PlaylistCollaborator>(entity =>
            {
                entity.HasIndex(pc => new { pc.PlaylistId, pc.UserId }).IsUnique();
                entity.HasOne(pc => pc.Playlist).WithMany(p => p.Collaborators).HasForeignKey(pc => pc.PlaylistId).OnDelete(DeleteBehavior.Cascade);
                // UserId ilişkisi User entity'sinde .WithMany(u => u.PlaylistCollaborations) ile kuruldu.
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasIndex(l => new { l.UserId, l.LikedEntityType, l.LikedEntityId }).IsUnique();
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasIndex(f => new { f.FollowerUserId, f.FollowingUserId }).IsUnique();
                entity.HasOne(f => f.FollowerUser).WithMany(u => u.Following).HasForeignKey(f => f.FollowerUserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(f => f.FollowingUser).WithMany(u => u.Followers).HasForeignKey(f => f.FollowingUserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Save>(entity =>
            {
                entity.HasIndex(s => new { s.UserId, s.SavedEntityType, s.SavedEntityId }).IsUnique();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.ParentComment).WithMany(c => c.Replies).HasForeignKey(c => c.ParentCommentId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Notification>(entity =>
            {
                // ReceiverUser ve ActorUser ilişkileri User entity'sinde tanımlandı.
            });

            modelBuilder.Entity<ChatParticipant>(entity =>
            {
                entity.HasIndex(cp => new { cp.ChatId, cp.UserId }).IsUnique();
                entity.HasOne(cp => cp.Chat).WithMany(c => c.Participants).HasForeignKey(cp => cp.ChatId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(cp => cp.LastReadMessage).WithMany().HasForeignKey(cp => cp.LastReadMessageId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
                // UserId ilişkisi User entity'sinde tanımlandı.
            });
            
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Chat).WithMany(c => c.Messages).HasForeignKey(m => m.ChatId).OnDelete(DeleteBehavior.Cascade);
                // SenderUserId ilişkisi User entity'sinde tanımlandı.
            });
            
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasOne(c => c.LastMessage).WithMany().HasForeignKey(c => c.LastMessageId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}