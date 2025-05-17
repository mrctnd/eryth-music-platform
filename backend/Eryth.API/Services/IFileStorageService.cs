using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Verilen IFormFile'ı belirtilen klasöre benzersiz bir isimle kaydeder.
        /// </summary>
        /// <param name="file">Kaydedilecek dosya.</param>
        /// <param name="folderPath">Dosyanın kaydedileceği ana klasörün altındaki göreli yol (örn: "music_files", "cover_images").</param>
        /// <returns>Kaydedilen dosyanın sunucuya göreli URL'ini veya null (hata durumunda).</returns>
        Task<string?> SaveFileAsync(IFormFile file, string subfolder);

        /// <summary>
        /// Verilen yoldaki dosyayı siler.
        /// </summary>
        /// <param name="fileRelativePath">Sunucuya göreli dosya yolu (örn: "/uploads/music_files/dosya.mp3").</param>
        Task<bool> DeleteFileAsync(string? fileRelativePath);
    }
}