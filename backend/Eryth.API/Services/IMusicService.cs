using Eryth.API.DTOs;
using Eryth.API.Models; // Music entity'si için
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public interface IMusicService
    {
        Task<(bool Succeeded, string? ErrorMessage, MusicViewDto? Music)> CreateMusicAsync(MusicCreateDto musicCreateDto, long uploaderUserId);
        Task<MusicViewDto?> GetMusicByIdAsync(long musicId);
        Task<IEnumerable<MusicViewDto>> GetRecentlyUploadedMusicsAsync(int count = 20);
        // Task<IEnumerable<MusicViewDto>> GetMusicsByGenreAsync(long genreId, int page = 1, int pageSize = 20);
        // Task<bool> DeleteMusicAsync(long musicId, long currentUserId); // Yetkilendirme önemli
    }
}