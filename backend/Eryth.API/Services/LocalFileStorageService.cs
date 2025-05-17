using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için
using System;
using System.IO; // Path, FileStream, Directory işlemleri için
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // ILogger için (opsiyonel)

namespace Eryth.API.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string _baseUploadsFolder = "uploads"; // wwwroot altındaki ana yükleme klasörü

        public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<LocalFileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string?> SaveFileAsync(IFormFile file, string subfolder)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                // wwwroot/uploads/{subfolder} yolunu oluştur
                var targetFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, _baseUploadsFolder, subfolder);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullPath = Path.Combine(targetFolder, uniqueFileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Veritabanına kaydedilecek ve istemciye dönecek göreli URL
                var relativeUrl = $"/{_baseUploadsFolder}/{subfolder}/{uniqueFileName}";
                _logger.LogInformation("Dosya başarıyla kaydedildi: {FilePath}", relativeUrl);
                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya kaydedilirken hata oluştu. Subfolder: {Subfolder}, FileName: {FileName}", subfolder, file.FileName);
                return null;
            }
        }

        public Task<bool> DeleteFileAsync(string? fileRelativePath)
        {
            if (string.IsNullOrEmpty(fileRelativePath))
            {
                return Task.FromResult(false);
            }

            try
            {
                // Göreli URL'den tam dosya yolunu oluştur
                // fileRelativePath başında '/' varsa kaldır
                var pathToDelete = fileRelativePath.TrimStart('/');
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, pathToDelete);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Dosya başarıyla silindi: {FilePath}", fullPath);
                    return Task.FromResult(true);
                }
                else
                {
                    _logger.LogWarning("Silinecek dosya bulunamadı: {FilePath}", fullPath);
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya silinirken hata oluştu: {FilePath}", fileRelativePath);
                return Task.FromResult(false);
            }
        }
    }
}