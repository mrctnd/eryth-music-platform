-- UUID Fonksiyonları için Gerekli Eklenti (Eğer kurulu değilse)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ENUM Types (Özel Veri Tipleri)
CREATE TYPE UserStatus AS ENUM ('active', 'suspended', 'deleted', 'pending_verification');
CREATE TYPE UserRole AS ENUM ('user', 'artist', 'admin');
CREATE TYPE MusicVisibility AS ENUM ('public', 'private', 'unlisted');
CREATE TYPE MusicStatus AS ENUM ('processing', 'active', 'failed_processing', 'taken_down_by_user', 'taken_down_by_admin');
CREATE TYPE AlbumType AS ENUM ('album', 'ep', 'single', 'compilation');
CREATE TYPE AlbumVisibility AS ENUM ('public', 'private', 'unlisted');
CREATE TYPE AlbumStatus AS ENUM ('draft', 'active', 'taken_down');
CREATE TYPE PlaylistVisibility AS ENUM ('public', 'private', 'collaborative');
CREATE TYPE PlaylistStatus AS ENUM ('active', 'archived');
CREATE TYPE PlaylistCollaboratorRole AS ENUM ('editor', 'viewer');
CREATE TYPE LikedEntityType AS ENUM ('Music', 'Album', 'Playlist', 'Comment');
CREATE TYPE CommentedEntityType AS ENUM ('Music', 'Album', 'Playlist');
CREATE TYPE CommentStatus AS ENUM ('active', 'hidden_by_user', 'deleted_by_moderator');
CREATE TYPE NotificationType AS ENUM (
    'USER_FOLLOWED_YOU', 'MUSIC_LIKED', 'ALBUM_LIKED', 'PLAYLIST_LIKED', 'COMMENT_LIKED',
    'COMMENT_ON_YOUR_MUSIC', 'COMMENT_ON_YOUR_ALBUM', 'COMMENT_ON_YOUR_PLAYLIST',
    'REPLY_TO_YOUR_COMMENT', 'NEW_MUSIC_BY_FOLLOWED_USER', 'NEW_ALBUM_BY_FOLLOWED_USER'
);
CREATE TYPE PrimaryEntityType AS ENUM ('User', 'Music', 'Album', 'Playlist', 'Comment');
CREATE TYPE ChatType AS ENUM ('one_to_one');
CREATE TYPE MessageType AS ENUM ('text', 'image_link', 'music_link', 'album_link', 'playlist_link');
CREATE TYPE MessageStatus AS ENUM ('sent', 'delivered_to_server', 'read_by_receiver', 'failed');
CREATE TYPE ActivityType AS ENUM (
    'USER_REGISTERED', 'USER_LOGGED_IN', 'USER_UPDATED_PROFILE', 'MUSIC_UPLOADED',
    'MUSIC_PLAYED', 'ALBUM_CREATED', 'PLAYLIST_CREATED', 'ITEM_LIKED', 'ITEM_UNLIKED',
    'COMMENT_POSTED', 'USER_FOLLOWED', 'USER_UNFOLLOWED'
);

-- Trigger Fonksiyonları
CREATE OR REPLACE FUNCTION trigger_set_timestamp()
RETURNS TRIGGER AS $$
BEGIN
  NEW.UpdatedAt = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION trigger_set_last_accessed()
RETURNS TRIGGER AS $$
BEGIN
  NEW.LastAccessedAt = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Tablo Oluşturma Komutları

-- Users (Kullanıcılar)
CREATE TABLE Users (
    UserID BIGSERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    DisplayName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash TEXT NOT NULL,
    ProfilePictureURL TEXT,
    CoverPhotoURL TEXT,
    Bio TEXT,
    Location VARCHAR(100),
    WebsiteURL TEXT,
    Role UserRole DEFAULT 'user' NOT NULL,
    EmailVerified BOOLEAN DEFAULT FALSE NOT NULL,
    EmailVerificationToken TEXT,
    PasswordResetToken TEXT,
    PasswordResetExpiresAt TIMESTAMPTZ,
    LastLoginDate TIMESTAMPTZ,
    Status UserStatus DEFAULT 'pending_verification' NOT NULL,
    PrivacySettings JSONB,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_users_timestamp BEFORE UPDATE ON Users FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_users_username ON Users(Username);
CREATE INDEX idx_users_status ON Users(Status);
CREATE INDEX idx_users_privacy_settings ON Users USING GIN (PrivacySettings);

-- Genres (Müzik Türleri)
CREATE TABLE Genres (
    GenreID BIGSERIAL PRIMARY KEY,
    Name VARCHAR(100) UNIQUE NOT NULL,
    Slug VARCHAR(100) UNIQUE NOT NULL,
    ParentGenreID BIGINT REFERENCES Genres(GenreID) ON DELETE SET NULL,
    Description TEXT,
    CoverImageURL TEXT
);
CREATE INDEX idx_genres_parent ON Genres(ParentGenreID);
CREATE INDEX idx_genres_slug ON Genres(Slug);

-- Moods (Modlar/Duygu Durumları)
CREATE TABLE Moods (
    MoodID BIGSERIAL PRIMARY KEY,
    Name VARCHAR(100) UNIQUE NOT NULL,
    Slug VARCHAR(100) UNIQUE NOT NULL,
    CoverImageURL TEXT
);
CREATE INDEX idx_moods_slug ON Moods(Slug);

-- UserDefinedTags (Kullanıcı Tanımlı Etiketler)
CREATE TABLE UserDefinedTags (
    TagID BIGSERIAL PRIMARY KEY,
    Name VARCHAR(50) UNIQUE NOT NULL,
    Slug VARCHAR(50) UNIQUE NOT NULL,
    UsageCount BIGINT DEFAULT 0 NOT NULL
);
CREATE INDEX idx_tags_slug ON UserDefinedTags(Slug);
CREATE INDEX idx_tags_usage ON UserDefinedTags(UsageCount DESC);

-- Albums (Albümler)
CREATE TABLE Albums (
    AlbumID BIGSERIAL PRIMARY KEY,
    CreatorUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    Title VARCHAR(255) NOT NULL,
    AlbumType AlbumType NOT NULL,
    PrimaryArtistDisplayText VARCHAR(255) NOT NULL,
    CoverImageURL TEXT,
    Description TEXT,
    ReleaseDate DATE,
    GenreID BIGINT REFERENCES Genres(GenreID) ON DELETE SET NULL,
    UPC VARCHAR(20),
    RecordLabelName VARCHAR(100),
    Visibility AlbumVisibility DEFAULT 'public' NOT NULL,
    Status AlbumStatus DEFAULT 'active' NOT NULL,
    TotalPlayCount BIGINT DEFAULT 0 NOT NULL,
    TotalLikeCount BIGINT DEFAULT 0 NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_albums_timestamp BEFORE UPDATE ON Albums FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_albums_creator ON Albums(CreatorUserID);
CREATE INDEX idx_albums_genre ON Albums(GenreID);
CREATE INDEX idx_albums_visibility ON Albums(Visibility);
CREATE INDEX idx_albums_status ON Albums(Status);
CREATE INDEX idx_albums_release_date ON Albums(ReleaseDate);

-- Musics (Müzikler)
CREATE TABLE Musics (
    MusicID BIGSERIAL PRIMARY KEY,
    UploaderUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    Title VARCHAR(255) NOT NULL,
    PrimaryArtistDisplayText VARCHAR(255) NOT NULL,
    FeaturedArtistsDisplayText TEXT[],
    GenreID BIGINT REFERENCES Genres(GenreID) ON DELETE SET NULL,
    SubGenreID BIGINT REFERENCES Genres(GenreID) ON DELETE SET NULL,
    -- MoodIDs BIGINT[] alanı kaldırıldı, ilişki MusicMoods üzerinden kurulacak.
    Tags TEXT[], -- Kullanıcı tarafından girilen serbest metin etiketler için
    DurationSeconds INTEGER NOT NULL,
    AudioFilePath TEXT NOT NULL,
    WaveformDataPath TEXT,
    CoverImageURL TEXT,
    Description TEXT,
    Lyrics TEXT,
    ReleaseDate DATE,
    UploadDate TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    Visibility MusicVisibility DEFAULT 'public' NOT NULL,
    Status MusicStatus DEFAULT 'processing' NOT NULL,
    IsDownloadable BOOLEAN DEFAULT FALSE NOT NULL,
    ISRC VARCHAR(20),
    BPM INTEGER,
    KeySignature VARCHAR(10),
    AudioFileFormat VARCHAR(10),
    AudioBitrateKbitps INTEGER,
    AudioFileSizeMB DECIMAL(10,2),
    PlayCount BIGINT DEFAULT 0 NOT NULL,
    LikeCount BIGINT DEFAULT 0 NOT NULL,
    CommentCount BIGINT DEFAULT 0 NOT NULL,
    RepostCount BIGINT DEFAULT 0 NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_musics_timestamp BEFORE UPDATE ON Musics FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_musics_uploader ON Musics(UploaderUserID);
CREATE INDEX idx_musics_genre ON Musics(GenreID);
CREATE INDEX idx_musics_subgenre ON Musics(SubGenreID);
CREATE INDEX idx_musics_visibility ON Musics(Visibility);
CREATE INDEX idx_musics_status ON Musics(Status);
CREATE INDEX idx_musics_release_date ON Musics(ReleaseDate);
-- idx_musics_mood_array indeksi kaldırıldı.
CREATE INDEX idx_musics_tags_array ON Musics USING GIN (Tags);

-- MusicMoods (Müzik-Mood Ara Tablosu)
CREATE TABLE MusicMoods (
    MusicMoodID BIGSERIAL PRIMARY KEY,
    MusicID BIGINT NOT NULL REFERENCES Musics(MusicID) ON DELETE CASCADE,
    MoodID BIGINT NOT NULL REFERENCES Moods(MoodID) ON DELETE CASCADE,
    UNIQUE (MusicID, MoodID)
);
CREATE INDEX idx_music_moods_music ON MusicMoods(MusicID);
CREATE INDEX idx_music_moods_mood ON MusicMoods(MoodID);

-- AlbumMusics (Albümdeki Müzikler - Ara Tablo)
CREATE TABLE AlbumMusics (
    AlbumMusicID BIGSERIAL PRIMARY KEY,
    AlbumID BIGINT NOT NULL REFERENCES Albums(AlbumID) ON DELETE CASCADE,
    MusicID BIGINT NOT NULL REFERENCES Musics(MusicID) ON DELETE CASCADE,
    DiscNumber INTEGER DEFAULT 1 NOT NULL,
    TrackNumber INTEGER NOT NULL,
    UNIQUE (AlbumID, MusicID),
    UNIQUE (AlbumID, DiscNumber, TrackNumber)
);
CREATE INDEX idx_album_musics_album ON AlbumMusics(AlbumID);
CREATE INDEX idx_album_musics_music ON AlbumMusics(MusicID);

-- MusicUserDefinedTags (Müzik-Etiket Ara Tablosu)
CREATE TABLE MusicUserDefinedTags (
    MusicTagID BIGSERIAL PRIMARY KEY,
    MusicID BIGINT NOT NULL REFERENCES Musics(MusicID) ON DELETE CASCADE,
    TagID BIGINT NOT NULL REFERENCES UserDefinedTags(TagID) ON DELETE CASCADE,
    UNIQUE (MusicID, TagID)
);
CREATE INDEX idx_music_tags_music ON MusicUserDefinedTags(MusicID);
CREATE INDEX idx_music_tags_tag ON MusicUserDefinedTags(TagID);

-- UserAuthenticationProviders (Kullanıcı Kimlik Doğrulama Sağlayıcıları)
CREATE TABLE UserAuthenticationProviders (
    UserAuthProviderID BIGSERIAL PRIMARY KEY,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    ProviderName VARCHAR(50) NOT NULL,
    ProviderUserID TEXT NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UNIQUE (UserID, ProviderName),
    UNIQUE (ProviderName, ProviderUserID)
);
CREATE INDEX idx_auth_providers_user ON UserAuthenticationProviders(UserID);

-- Playlists (Çalma Listeleri)
CREATE TABLE Playlists (
    PlaylistID BIGSERIAL PRIMARY KEY,
    CreatorUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    Title VARCHAR(150) NOT NULL,
    Description TEXT,
    CoverImageURL TEXT,
    Visibility PlaylistVisibility DEFAULT 'public' NOT NULL,
    Status PlaylistStatus DEFAULT 'active' NOT NULL,
    CanSubscribersAddTracks BOOLEAN DEFAULT FALSE NOT NULL,
    TotalDurationSeconds INTEGER DEFAULT 0 NOT NULL,
    TrackCount INTEGER DEFAULT 0 NOT NULL,
    FollowerCount BIGINT DEFAULT 0 NOT NULL,
    LikeCount BIGINT DEFAULT 0 NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_playlists_timestamp BEFORE UPDATE ON Playlists FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_playlists_creator ON Playlists(CreatorUserID);
CREATE INDEX idx_playlists_visibility ON Playlists(Visibility);
CREATE INDEX idx_playlists_status ON Playlists(Status);

-- PlaylistMusics (Çalma Listesindeki Müzikler - Ara Tablo)
CREATE TABLE PlaylistMusics (
    PlaylistMusicID BIGSERIAL PRIMARY KEY,
    PlaylistID BIGINT NOT NULL REFERENCES Playlists(PlaylistID) ON DELETE CASCADE,
    MusicID BIGINT NOT NULL REFERENCES Musics(MusicID) ON DELETE CASCADE,
    AddedByUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    AddedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CustomOrder INTEGER,
    UNIQUE (PlaylistID, MusicID)
);
CREATE INDEX idx_playlist_musics_playlist ON PlaylistMusics(PlaylistID);
CREATE INDEX idx_playlist_musics_music ON PlaylistMusics(MusicID);
CREATE INDEX idx_playlist_musics_added_by ON PlaylistMusics(AddedByUserID);

-- PlaylistCollaborators (Çalma Listesi Ortak Çalışanları)
CREATE TABLE PlaylistCollaborators (
    PlaylistCollaboratorID BIGSERIAL PRIMARY KEY,
    PlaylistID BIGINT NOT NULL REFERENCES Playlists(PlaylistID) ON DELETE CASCADE,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    Role PlaylistCollaboratorRole DEFAULT 'viewer' NOT NULL,
    AddedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UNIQUE (PlaylistID, UserID)
);
CREATE INDEX idx_playlist_collaborators_playlist ON PlaylistCollaborators(PlaylistID);
CREATE INDEX idx_playlist_collaborators_user ON PlaylistCollaborators(UserID);

-- Likes (Beğeniler - Genel)
CREATE TABLE Likes (
    LikeID BIGSERIAL PRIMARY KEY,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    LikedEntityType LikedEntityType NOT NULL,
    LikedEntityID BIGINT NOT NULL,
    LikedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UNIQUE (UserID, LikedEntityType, LikedEntityID)
);
CREATE INDEX idx_likes_user ON Likes(UserID);
CREATE INDEX idx_likes_entity ON Likes(LikedEntityType, LikedEntityID);
CREATE INDEX idx_likes_timestamp ON Likes(LikedAt);

-- Follows (Kullanıcı Takipleri)
CREATE TABLE Follows (
    FollowID BIGSERIAL PRIMARY KEY,
    FollowerUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    FollowingUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    FollowedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CHECK (FollowerUserID <> FollowingUserID),
    UNIQUE (FollowerUserID, FollowingUserID)
);
CREATE INDEX idx_follows_follower ON Follows(FollowerUserID);
CREATE INDEX idx_follows_following ON Follows(FollowingUserID);
CREATE INDEX idx_follows_timestamp ON Follows(FollowedAt);

-- Saves (Kaydedilenler - Albüm/Playlist Kütüphaneye Ekleme)
CREATE TABLE Saves (
    SaveID BIGSERIAL PRIMARY KEY,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    SavedEntityType LikedEntityType NOT NULL, -- 'Album' veya 'Playlist' olabilir
    SavedEntityID BIGINT NOT NULL,
    SavedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UNIQUE (UserID, SavedEntityType, SavedEntityID)
);
CREATE INDEX idx_saves_user ON Saves(UserID);
CREATE INDEX idx_saves_entity ON Saves(SavedEntityType, SavedEntityID);
CREATE INDEX idx_saves_timestamp ON Saves(SavedAt);

-- Comments (Yorumlar - Genel)
CREATE TABLE Comments (
    CommentID BIGSERIAL PRIMARY KEY,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    CommentedEntityType CommentedEntityType NOT NULL,
    CommentedEntityID BIGINT NOT NULL,
    ParentCommentID BIGINT REFERENCES Comments(CommentID) ON DELETE CASCADE,
    Text TEXT NOT NULL,
    Status CommentStatus DEFAULT 'active' NOT NULL,
    LikeCount BIGINT DEFAULT 0 NOT NULL,
    CommentedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_comments_timestamp BEFORE UPDATE ON Comments FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_comments_user ON Comments(UserID);
CREATE INDEX idx_comments_entity ON Comments(CommentedEntityType, CommentedEntityID);
CREATE INDEX idx_comments_parent ON Comments(ParentCommentID);
CREATE INDEX idx_comments_status ON Comments(Status);
CREATE INDEX idx_comments_timestamp ON Comments(CommentedAt);

-- Chats (Sohbetler)
CREATE TABLE Chats (
    ChatID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ChatType ChatType DEFAULT 'one_to_one' NOT NULL,
    LastMessageID BIGINT, 
    LastMessageAt TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_chats_timestamp BEFORE UPDATE ON Chats FOR EACH ROW EXECUTE PROCEDURE trigger_set_timestamp();
CREATE INDEX idx_chats_last_message_at ON Chats(LastMessageAt DESC);

-- Messages (Mesajlar)
CREATE TABLE Messages (
    MessageID BIGSERIAL PRIMARY KEY,
    ChatID UUID NOT NULL REFERENCES Chats(ChatID) ON DELETE CASCADE,
    SenderUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    MessageType MessageType DEFAULT 'text' NOT NULL,
    ContentText TEXT NOT NULL,
    SentAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    Status MessageStatus DEFAULT 'sent' NOT NULL,
    DeletedForSender BOOLEAN DEFAULT FALSE NOT NULL,
    DeletedForEveryone BOOLEAN DEFAULT FALSE NOT NULL
);
CREATE INDEX idx_messages_chat ON Messages(ChatID);
CREATE INDEX idx_messages_sender ON Messages(SenderUserID);
CREATE INDEX idx_messages_sent_at ON Messages(SentAt);
CREATE INDEX idx_messages_status ON Messages(Status);

-- ChatParticipants (Sohbet Katılımcıları - Ara Tablo)
CREATE TABLE ChatParticipants (
    ChatParticipantID BIGSERIAL PRIMARY KEY,
    ChatID UUID NOT NULL REFERENCES Chats(ChatID) ON DELETE CASCADE,
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    JoinedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    LastReadMessageID BIGINT REFERENCES Messages(MessageID) ON DELETE SET NULL,
    NotificationsEnabled BOOLEAN DEFAULT TRUE NOT NULL,
    UNIQUE (ChatID, UserID)
);
CREATE INDEX idx_chat_participants_chat ON ChatParticipants(ChatID);
CREATE INDEX idx_chat_participants_user ON ChatParticipants(UserID);
CREATE INDEX idx_chat_participants_last_read ON ChatParticipants(LastReadMessageID);

-- Chats tablosundaki LastMessageID için Foreign Key (Messages tablosu oluşturulduktan sonra)
ALTER TABLE Chats
ADD CONSTRAINT fk_chats_last_message FOREIGN KEY (LastMessageID)
REFERENCES Messages(MessageID) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED;

-- Notifications (Bildirimler)
CREATE TABLE Notifications (
    NotificationID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ReceiverUserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    NotificationType NotificationType NOT NULL,
    ActorUserID BIGINT REFERENCES Users(UserID) ON DELETE SET NULL,
    PrimaryEntityType PrimaryEntityType,
    PrimaryEntityID BIGINT,
    ContentText TEXT,
    LinkURL TEXT,
    IsRead BOOLEAN DEFAULT FALSE NOT NULL,
    ReadAt TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE INDEX idx_notifications_receiver ON Notifications(ReceiverUserID);
CREATE INDEX idx_notifications_actor ON Notifications(ActorUserID);
CREATE INDEX idx_notifications_is_read ON Notifications(IsRead) WHERE IsRead = FALSE;
CREATE INDEX idx_notifications_created_at ON Notifications(CreatedAt DESC);
CREATE INDEX idx_notifications_entity ON Notifications(PrimaryEntityType, PrimaryEntityID);

-- UserActivityLog (Kullanıcı Aktivite Kayıtları)
CREATE TABLE UserActivityLog (
    ActivityLogID BIGSERIAL PRIMARY KEY,
    UserID BIGINT REFERENCES Users(UserID) ON DELETE SET NULL,
    ActivityType ActivityType NOT NULL,
    PrimaryEntityType PrimaryEntityType,
    PrimaryEntityID BIGINT,
    Details JSONB,
    Timestamp TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE INDEX idx_activity_log_user ON UserActivityLog(UserID);
CREATE INDEX idx_activity_log_timestamp ON UserActivityLog(Timestamp DESC);
CREATE INDEX idx_activity_log_type ON UserActivityLog(ActivityType);
CREATE INDEX idx_activity_log_entity ON UserActivityLog(PrimaryEntityType, PrimaryEntityID);
CREATE INDEX idx_activity_log_details ON UserActivityLog USING GIN (Details);

-- UserSessions (Kullanıcı Oturumları)
CREATE TABLE UserSessions (
    SessionID UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserID BIGINT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    RefreshTokenHash TEXT NOT NULL,
    UserAgent TEXT,
    IPAddress VARCHAR(45),
    ExpiresAt TIMESTAMPTZ NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
    LastAccessedAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE TRIGGER set_user_sessions_last_accessed BEFORE UPDATE ON UserSessions FOR EACH ROW EXECUTE PROCEDURE trigger_set_last_accessed();
CREATE INDEX idx_user_sessions_user ON UserSessions(UserID);
CREATE INDEX idx_user_sessions_expires ON UserSessions(ExpiresAt);
CREATE INDEX idx_user_sessions_last_accessed ON UserSessions(LastAccessedAt);

-- Veritabanı Yorumu
COMMENT ON DATABASE current_database() IS 'Created on 2025-05-17 by ErythPlatformDev. Music streaming platform database schema.';