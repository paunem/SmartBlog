using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SmartBlog.Application.Clients;

namespace SmartBlog.Infrastructure.Clients;

public class BlobClient : IBlobClient
{
    private readonly BlobContainerClient blobContainerClient;

    public BlobClient(BlobServiceClient blobServiceClient)
    {
        blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
        blobContainerClient.CreateIfNotExists();
    }

    public async Task<string> UploadFile(IFormFile file)
    {
        var fileExtention = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString() + fileExtention;
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(file.OpenReadStream());

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task<string> UploadFile(Stream file)
    {
        var fileName = Guid.NewGuid().ToString() + ".png";
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(file);

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task DeleteFile(string fileName)
    {
        await blobContainerClient.DeleteBlobIfExistsAsync(fileName);
    }
}
