namespace Ecommerce.API.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folderPath);
        Task DeleteImageAsync(string imageName, string folderPath);
    }
}
