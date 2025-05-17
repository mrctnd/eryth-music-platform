using Eryth.API.DTOs;
using Eryth.API.Models; // Album entity'si için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public interface IAlbumService
    {
        Task<(bool Succeeded, string? ErrorMessage, AlbumViewDto? Album)> CreateAlbumAsync(AlbumCreateDto albumCreateDto, long creatorUserId);
        Task<AlbumViewDto?> GetAlbumByIdAsync(long albumId);
        // Task<IEnumerable<AlbumViewDto>> GetAlbumsByUserAsync(long userId);
    }
}