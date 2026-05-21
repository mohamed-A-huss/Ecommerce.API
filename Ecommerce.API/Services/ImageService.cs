namespace Ecommerce.API.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> SaveImageAsync(IFormFile file, string folderPath)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Only .jpg, .jpeg, and .png files are allowed.");
            }
            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new Exception("File size cannot exceed 10 MB.");
            }
            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                folderPath);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName =
                Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);
            using var stream = new FileStream(
                filePath,
                FileMode.Create);
            await file.CopyToAsync(stream);

            return uniqueFileName;
        }
        public Task DeleteImageAsync(string imageName, string folderPath)
        {
            var imagePath = Path.Combine(
                _environment.WebRootPath,
                "images",
                folderPath,
                imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            return Task.CompletedTask;
        }
    }
}
