using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eryth.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateNet8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    genre_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parent_genre_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    cover_image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genres", x => x.genre_id);
                    table.ForeignKey(
                        name: "fk_genres_genres_parent_genre_id",
                        column: x => x.parent_genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "moods",
                columns: table => new
                {
                    mood_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cover_image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moods", x => x.mood_id);
                });

            migrationBuilder.CreateTable(
                name: "user_defined_tags",
                columns: table => new
                {
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    usage_count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_defined_tags", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    profile_picture_url = table.Column<string>(type: "text", nullable: true),
                    cover_photo_url = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<string>(type: "text", nullable: true),
                    website_url = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    email_verification_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    privacy_settings = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    album_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    creator_user_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    album_type = table.Column<int>(type: "integer", nullable: false),
                    primary_artist_display_text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    release_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    genre_id = table.Column<long>(type: "bigint", nullable: true),
                    upc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    record_label_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_play_count = table.Column<long>(type: "bigint", nullable: false),
                    total_like_count = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.album_id);
                    table.ForeignKey(
                        name: "fk_albums_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_albums_users_creator_user_id",
                        column: x => x.creator_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follows",
                columns: table => new
                {
                    follow_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    follower_user_id = table.Column<long>(type: "bigint", nullable: false),
                    following_user_id = table.Column<long>(type: "bigint", nullable: false),
                    followed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_follows", x => x.follow_id);
                    table.ForeignKey(
                        name: "fk_follows_users_follower_user_id",
                        column: x => x.follower_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_follows_users_following_user_id",
                        column: x => x.following_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "musics",
                columns: table => new
                {
                    music_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uploader_user_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    primary_artist_display_text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    featured_artists_display_text = table.Column<List<string>>(type: "text[]", nullable: true),
                    genre_id = table.Column<long>(type: "bigint", nullable: true),
                    sub_genre_id = table.Column<long>(type: "bigint", nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    audio_file_path = table.Column<string>(type: "text", nullable: false),
                    waveform_data_path = table.Column<string>(type: "text", nullable: true),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    lyrics = table.Column<string>(type: "text", nullable: true),
                    release_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    is_downloadable = table.Column<bool>(type: "boolean", nullable: false),
                    isrc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bpm = table.Column<int>(type: "integer", nullable: true),
                    key_signature = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    audio_file_format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    audio_bitrate_kbitps = table.Column<int>(type: "integer", nullable: true),
                    audio_file_size_mb = table.Column<decimal>(type: "numeric", nullable: true),
                    play_count = table.Column<long>(type: "bigint", nullable: false),
                    like_count = table.Column<long>(type: "bigint", nullable: false),
                    comment_count = table.Column<long>(type: "bigint", nullable: false),
                    repost_count = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_musics", x => x.music_id);
                    table.ForeignKey(
                        name: "fk_musics_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_musics_genres_sub_genre_id",
                        column: x => x.sub_genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_musics_users_uploader_user_id",
                        column: x => x.uploader_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_user_id = table.Column<long>(type: "bigint", nullable: false),
                    notification_type = table.Column<int>(type: "integer", nullable: false),
                    actor_user_id = table.Column<long>(type: "bigint", nullable: true),
                    primary_entity_type = table.Column<int>(type: "integer", nullable: true),
                    primary_entity_id = table.Column<long>(type: "bigint", nullable: true),
                    content_text = table.Column<string>(type: "text", nullable: true),
                    link_url = table.Column<string>(type: "text", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_notifications_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_notifications_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlists",
                columns: table => new
                {
                    playlist_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    creator_user_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    can_subscribers_add_tracks = table.Column<bool>(type: "boolean", nullable: false),
                    total_duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    track_count = table.Column<int>(type: "integer", nullable: false),
                    follower_count = table.Column<long>(type: "bigint", nullable: false),
                    like_count = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists", x => x.playlist_id);
                    table.ForeignKey(
                        name: "fk_playlists_users_creator_user_id",
                        column: x => x.creator_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_activity_logs",
                columns: table => new
                {
                    activity_log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    activity_type = table.Column<int>(type: "integer", nullable: false),
                    primary_entity_type = table.Column<int>(type: "integer", nullable: true),
                    primary_entity_id = table.Column<long>(type: "bigint", nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_activity_logs", x => x.activity_log_id);
                    table.ForeignKey(
                        name: "fk_user_activity_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_authentication_providers",
                columns: table => new
                {
                    user_auth_provider_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    provider_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    provider_user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_authentication_providers", x => x.user_auth_provider_id);
                    table.ForeignKey(
                        name: "fk_user_authentication_providers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "text", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.session_id);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_musics",
                columns: table => new
                {
                    album_music_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    album_id = table.Column<long>(type: "bigint", nullable: false),
                    music_id = table.Column<long>(type: "bigint", nullable: false),
                    disc_number = table.Column<int>(type: "integer", nullable: false),
                    track_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_musics", x => x.album_music_id);
                    table.ForeignKey(
                        name: "fk_album_musics_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "album_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_musics_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "music_moods",
                columns: table => new
                {
                    music_mood_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    music_id = table.Column<long>(type: "bigint", nullable: false),
                    mood_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_music_moods", x => x.music_mood_id);
                    table.ForeignKey(
                        name: "fk_music_moods_moods_mood_id",
                        column: x => x.mood_id,
                        principalTable: "moods",
                        principalColumn: "mood_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_music_moods_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "music_user_defined_tags",
                columns: table => new
                {
                    music_tag_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    music_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_music_user_defined_tags", x => x.music_tag_id);
                    table.ForeignKey(
                        name: "fk_music_user_defined_tags_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_music_user_defined_tags_user_defined_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "user_defined_tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    comment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    commented_entity_type = table.Column<int>(type: "integer", nullable: false),
                    commented_entity_id = table.Column<long>(type: "bigint", nullable: false),
                    parent_comment_id = table.Column<long>(type: "bigint", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    like_count = table.Column<long>(type: "bigint", nullable: false),
                    commented_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    album_id = table.Column<long>(type: "bigint", nullable: true),
                    music_id = table.Column<long>(type: "bigint", nullable: true),
                    playlist_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "fk_comments_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "album_id");
                    table.ForeignKey(
                        name: "fk_comments_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id");
                    table.ForeignKey(
                        name: "fk_comments_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "playlist_id");
                    table.ForeignKey(
                        name: "fk_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_collaborators",
                columns: table => new
                {
                    playlist_collaborator_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    playlist_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_collaborators", x => x.playlist_collaborator_id);
                    table.ForeignKey(
                        name: "fk_playlist_collaborators_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "playlist_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_collaborators_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_musics",
                columns: table => new
                {
                    playlist_music_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    playlist_id = table.Column<long>(type: "bigint", nullable: false),
                    music_id = table.Column<long>(type: "bigint", nullable: false),
                    added_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    custom_order = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_musics", x => x.playlist_music_id);
                    table.ForeignKey(
                        name: "fk_playlist_musics_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_musics_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "playlist_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_musics_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saves",
                columns: table => new
                {
                    save_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    saved_entity_type = table.Column<int>(type: "integer", nullable: false),
                    saved_entity_id = table.Column<long>(type: "bigint", nullable: false),
                    saved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    album_id = table.Column<long>(type: "bigint", nullable: true),
                    playlist_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saves", x => x.save_id);
                    table.ForeignKey(
                        name: "fk_saves_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "album_id");
                    table.ForeignKey(
                        name: "fk_saves_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "playlist_id");
                    table.ForeignKey(
                        name: "fk_saves_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "likes",
                columns: table => new
                {
                    like_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    liked_entity_type = table.Column<int>(type: "integer", nullable: false),
                    liked_entity_id = table.Column<long>(type: "bigint", nullable: false),
                    liked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    album_id = table.Column<long>(type: "bigint", nullable: true),
                    comment_id = table.Column<long>(type: "bigint", nullable: true),
                    music_id = table.Column<long>(type: "bigint", nullable: true),
                    playlist_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_likes", x => x.like_id);
                    table.ForeignKey(
                        name: "fk_likes_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "album_id");
                    table.ForeignKey(
                        name: "fk_likes_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "fk_likes_musics_music_id",
                        column: x => x.music_id,
                        principalTable: "musics",
                        principalColumn: "music_id");
                    table.ForeignKey(
                        name: "fk_likes_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "playlist_id");
                    table.ForeignKey(
                        name: "fk_likes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_participants",
                columns: table => new
                {
                    chat_participant_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_read_message_id = table.Column<long>(type: "bigint", nullable: true),
                    notifications_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_participants", x => x.chat_participant_id);
                    table.ForeignKey(
                        name: "fk_chat_participants_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_type = table.Column<int>(type: "integer", nullable: false),
                    last_message_id = table.Column<long>(type: "bigint", nullable: true),
                    last_message_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.chat_id);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_user_id = table.Column<long>(type: "bigint", nullable: false),
                    message_type = table.Column<int>(type: "integer", nullable: false),
                    content_text = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    deleted_for_sender = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_for_everyone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.message_id);
                    table.ForeignKey(
                        name: "fk_messages_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "chat_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_musics_album_id_disc_number_track_number",
                table: "album_musics",
                columns: new[] { "album_id", "disc_number", "track_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_album_musics_album_id_music_id",
                table: "album_musics",
                columns: new[] { "album_id", "music_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_album_musics_music_id",
                table: "album_musics",
                column: "music_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_creator_user_id",
                table: "albums",
                column: "creator_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_genre_id",
                table: "albums",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_participants_chat_id_user_id",
                table: "chat_participants",
                columns: new[] { "chat_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chat_participants_last_read_message_id",
                table: "chat_participants",
                column: "last_read_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_participants_user_id",
                table: "chat_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_chats_last_message_id",
                table: "chats",
                column: "last_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_album_id",
                table: "comments",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_music_id",
                table: "comments",
                column: "music_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_playlist_id",
                table: "comments",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_follows_follower_user_id_following_user_id",
                table: "follows",
                columns: new[] { "follower_user_id", "following_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_follows_following_user_id",
                table: "follows",
                column: "following_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_genres_parent_genre_id",
                table: "genres",
                column: "parent_genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_genres_slug",
                table: "genres",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_likes_album_id",
                table: "likes",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_likes_comment_id",
                table: "likes",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_likes_music_id",
                table: "likes",
                column: "music_id");

            migrationBuilder.CreateIndex(
                name: "ix_likes_playlist_id",
                table: "likes",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "ix_likes_user_id_liked_entity_type_liked_entity_id",
                table: "likes",
                columns: new[] { "user_id", "liked_entity_type", "liked_entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_messages_chat_id",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_user_id",
                table: "messages",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_moods_slug",
                table: "moods",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_music_moods_mood_id",
                table: "music_moods",
                column: "mood_id");

            migrationBuilder.CreateIndex(
                name: "ix_music_moods_music_id_mood_id",
                table: "music_moods",
                columns: new[] { "music_id", "mood_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_music_user_defined_tags_music_id_tag_id",
                table: "music_user_defined_tags",
                columns: new[] { "music_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_music_user_defined_tags_tag_id",
                table: "music_user_defined_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_musics_genre_id",
                table: "musics",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_musics_sub_genre_id",
                table: "musics",
                column: "sub_genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_musics_uploader_user_id",
                table: "musics",
                column: "uploader_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_actor_user_id",
                table: "notifications",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_receiver_user_id",
                table: "notifications",
                column: "receiver_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_collaborators_playlist_id_user_id",
                table: "playlist_collaborators",
                columns: new[] { "playlist_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_playlist_collaborators_user_id",
                table: "playlist_collaborators",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_musics_added_by_user_id",
                table: "playlist_musics",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_musics_music_id",
                table: "playlist_musics",
                column: "music_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_musics_playlist_id_music_id",
                table: "playlist_musics",
                columns: new[] { "playlist_id", "music_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_playlists_creator_user_id",
                table: "playlists",
                column: "creator_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_saves_album_id",
                table: "saves",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_saves_playlist_id",
                table: "saves",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "ix_saves_user_id_saved_entity_type_saved_entity_id",
                table: "saves",
                columns: new[] { "user_id", "saved_entity_type", "saved_entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_activity_logs_user_id",
                table: "user_activity_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_authentication_providers_provider_name_provider_user_id",
                table: "user_authentication_providers",
                columns: new[] { "provider_name", "provider_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_authentication_providers_user_id_provider_name",
                table: "user_authentication_providers",
                columns: new[] { "user_id", "provider_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_defined_tags_slug",
                table: "user_defined_tags",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_chat_participants_chats_chat_id",
                table: "chat_participants",
                column: "chat_id",
                principalTable: "chats",
                principalColumn: "chat_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chat_participants_messages_last_read_message_id",
                table: "chat_participants",
                column: "last_read_message_id",
                principalTable: "messages",
                principalColumn: "message_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_chats_messages_last_message_id",
                table: "chats",
                column: "last_message_id",
                principalTable: "messages",
                principalColumn: "message_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_sender_user_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_chats_chat_id",
                table: "messages");

            migrationBuilder.DropTable(
                name: "album_musics");

            migrationBuilder.DropTable(
                name: "chat_participants");

            migrationBuilder.DropTable(
                name: "follows");

            migrationBuilder.DropTable(
                name: "likes");

            migrationBuilder.DropTable(
                name: "music_moods");

            migrationBuilder.DropTable(
                name: "music_user_defined_tags");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "playlist_collaborators");

            migrationBuilder.DropTable(
                name: "playlist_musics");

            migrationBuilder.DropTable(
                name: "saves");

            migrationBuilder.DropTable(
                name: "user_activity_logs");

            migrationBuilder.DropTable(
                name: "user_authentication_providers");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "moods");

            migrationBuilder.DropTable(
                name: "user_defined_tags");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "musics");

            migrationBuilder.DropTable(
                name: "playlists");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "messages");
        }
    }
}
