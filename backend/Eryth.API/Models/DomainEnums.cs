namespace Eryth.API.Models
{
    public enum UserStatus
    {
        Active,
        Suspended,
        Deleted,
        PendingVerification
    }

    public enum UserRole
    {
        User,
        Artist,
        Admin
    }

    public enum MusicVisibility
    {
        Public,
        Private,
        Unlisted
    }

    public enum MusicStatus
    {
        Processing,
        Active,
        FailedProcessing,
        TakenDownByUser,
        TakenDownByAdmin
    }

    public enum AlbumType
    {
        Album,
        Ep,
        Single,
        Compilation
    }

    public enum AlbumVisibility
    {
        Public,
        Private,
        Unlisted
    }

    public enum AlbumStatus
    {
        Draft,
        Active,
        TakenDown
    }

    public enum PlaylistVisibility
    {
        Public,
        Private,
        Collaborative
    }

    public enum PlaylistStatus
    {
        Active,
        Archived
    }

    public enum PlaylistCollaboratorRole
    {
        Editor,
        Viewer
    }

    public enum LikedEntityType
    {
        Music,
        Album,
        Playlist,
        Comment
    }

    public enum CommentedEntityType
    {
        Music,
        Album,
        Playlist
    }

    public enum CommentStatus
    {
        Active,
        HiddenByUser,
        DeletedByModerator
    }

    public enum NotificationType
    {
        UserFollowedYou,
        MusicLiked,
        AlbumLiked,
        PlaylistLiked,
        CommentLiked,
        CommentOnYourMusic,
        CommentOnYourAlbum,
        CommentOnYourPlaylist,
        ReplyToYourComment,
        NewMusicByFollowedUser,
        NewAlbumByFollowedUser
    }

    public enum PrimaryEntityType
    {
        User,
        Music,
        Album,
        Playlist,
        Comment
    }

    public enum ChatType
    {
        OneToOne
    }

    public enum MessageType
    {
        Text,
        ImageLink,
        MusicLink,
        AlbumLink,
        PlaylistLink
    }

    public enum MessageStatus
    {
        Sent,
        DeliveredToServer,
        ReadByReceiver,
        Failed
    }

    public enum ActivityType
    {
        UserRegistered,
        UserLoggedIn,
        UserUpdatedProfile,
        MusicUploaded,
        MusicPlayed,
        AlbumCreated,
        PlaylistCreated,
        ItemLiked,
        ItemUnliked,
        CommentPosted,
        UserFollowed,
        UserUnfollowed
    }
}