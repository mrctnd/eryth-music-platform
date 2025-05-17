// Eryth.API/Services/AlbumService.cs
using Eryth.API.Data;
using Eryth.API.DTOs;
using Eryth.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // IConfiguration için (kullanılmıyor ama kalabilir)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly ErythDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        // private readonly IConfiguration _configuration; // Şu an kullanılmıyor, gerekirse eklenebilir

        public AlbumService(ErythDbContext context, /* IConfiguration configuration, */ IFileStorageService fileStorageService)
        {
            _context = context;
            // _configuration = configuration;
            _fileStorageService = fileStorageService;
        }

        public async Task<(bool Succeeded, string? ErrorMessage, AlbumViewDto? Album)> CreateAlbumAsync(AlbumCreateDto albumCreateDto, long creatorUserId)
        {
            var creator = await _context.Users.FindAsync(creatorUserId);
            if (creator == null)
            {
                return (false, "Albümü oluşturan kullanıcı bulunamadı.", null);
            }

            string? coverImageRelativeUrl = null;
            if (albumCreateDto.CoverImageFile != null && albumCreateDto.CoverImageFile.Length > 0)
            {
                coverImageRelativeUrl = await _fileStorageService.SaveFileAsync(albumCreateDto.CoverImageFile, "cover_images");
                if (string.IsNullOrEmpty(coverImageRelativeUrl))
                {
                    return (false, "Albüm kapak resmi kaydedilemedi.", null);
                }
            }

            var album = new Album
            {
                CreatorUserId = creatorUserId,
                Title = albumCreateDto.Title,
                AlbumType = albumCreateDto.AlbumType,
                PrimaryArtistDisplayText = albumCreateDto.PrimaryArtistDisplayText,
                CoverImageUrl = coverImageRelativeUrl,
                Description = albumCreateDto.Description,
                ReleaseDate = albumCreateDto.ReleaseDate,
                GenreId = albumCreateDto.GenreId,
                Upc = albumCreateDto.Upc,
                RecordLabelName = albumCreateDto.RecordLabelName,
                Visibility = albumCreateDto.Visibility,
                Status = AlbumStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _context.Albums.AddAsync(album);
                await _context.SaveChangesAsync(); 
            }
            catch (DbUpdateException ex)
            {
                if (!string.IsNullOrEmpty(coverImageRelativeUrl))
                {
                    await _fileStorageService.DeleteFileAsync(coverImageRelativeUrl);
                }
                return (false, $"Albüm veritabanına kaydedilirken bir hata oluştu: {ex.InnerException?.Message ?? ex.Message}", null);
            }

            if (albumCreateDto.MusicIds != null && albumCreateDto.MusicIds.Any())
            {
                int trackNumber = 1;
                foreach (var musicId in albumCreateDto.MusicIds)
                {
                    var music = await _context.Musics.FindAsync(musicId);
                    if (music != null && music.UploaderUserId == creatorUserId) 
                    {
                        var albumMusic = new AlbumMusic
                        {
                            AlbumId = album.AlbumId,
                            MusicId = music.MusicId,
                            TrackNumber = trackNumber++
                        };
                        _context.AlbumMusics.Add(albumMusic);
                    }
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Burada idealde albümün kendisi de silinmeli veya işlem bir transaction içinde yapılmalı.
                    // Şimdilik hatayı döndürüyoruz.
                    return (false, $"Albüme müzikler eklenirken bir hata oluştu: {ex.InnerException?.Message ?? ex.Message}", null);
                }
            }
            
            // Oluşturulan albümün tam görünümünü almak için iç metodu çağır
            var (succeededView, errorMessageView, albumView) = await GetAlbumViewByIdAsyncInternal(album.AlbumId, creator);
            return (succeededView, errorMessageView, albumView);
        }

        public async Task<AlbumViewDto?> GetAlbumByIdAsync(long albumId)
        {
            var album = await _context.Albums
                .Include(a => a.CreatorUser) // Creator bilgisini çek
                .Include(a => a.Genre)
                .Include(a => a.AlbumMusics)
                    .ThenInclude(am => am.Music)
                .FirstOrDefaultAsync(a => a.AlbumId == albumId && a.Status == AlbumStatus.Active);

            if (album == null) return null;
            
            // CreatorUser zaten yüklendiği için MapAlbumToViewDto'ya direkt geçebiliriz.
            return MapAlbumToViewDto(album, album.CreatorUser);
        }

        private async Task<(bool Succeeded, string? ErrorMessage, AlbumViewDto? Album)> GetAlbumViewByIdAsyncInternal(long albumId, User loadedCreator)
        {
            var albumWithDetails = await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.AlbumMusics)
                    .ThenInclude(am => am.Music)
                .FirstOrDefaultAsync(a => a.AlbumId == albumId);

            if (albumWithDetails == null) return (false, "Albüm bulunamadı (detay alınırken).", null);

            // albumWithDetails.CreatorUser alanı bu sorguda yüklenmedi, bu yüzden loadedCreator'ı kullanıyoruz.
            return (true, null, MapAlbumToViewDto(albumWithDetails, loadedCreator));
        }
        
        private AlbumViewDto MapAlbumToViewDto(Album album, User creator) // preloadedCreator User? -> User olarak güncellendi
        {
            return new AlbumViewDto
            {
                AlbumId = album.AlbumId,
                Title = album.Title,
                AlbumType = album.AlbumType,
                PrimaryArtistDisplayText = album.PrimaryArtistDisplayText,
                CoverImageUrl = album.CoverImageUrl,
                Description = album.Description,
                ReleaseDate = album.ReleaseDate,
                GenreName = album.Genre?.Name,
                Creator = new UserSummaryDto
                {
                    UserId = creator.UserId,
                    Username = creator.Username,
                    DisplayName = creator.DisplayName,
                    ProfilePictureUrl = creator.ProfilePictureUrl
                },
                Tracks = album.AlbumMusics
                            .OrderBy(am => am.DiscNumber)
                            .ThenBy(am => am.TrackNumber)
                            .Select(am => new MusicTrackViewDto
                            {
                                MusicId = am.MusicId,
                                Title = am.Music.Title, // Music nesnesinin null olmamasını bekliyoruz
                                TrackNumber = am.TrackNumber,
                                DiscNumber = am.DiscNumber,
                                DurationSeconds = am.Music.DurationSeconds,
                                AudioFileUrl = am.Music.AudioFilePath,
                                PlayCount = am.Music.PlayCount
                            }).ToList(),
                TotalPlayCount = album.TotalPlayCount,
                TotalLikeCount = album.TotalLikeCount,
                CreatedAt = album.CreatedAt
            };
        }
    }
}