using Microsoft.AspNetCore.Http;

namespace SmartBlog.Application.Clients;

public interface IBlobClient
{
    Task<string> UploadFile(IFormFile file);

    Task<string> UploadFile(Stream file);

    Task DeleteFile(string fileName);
}
