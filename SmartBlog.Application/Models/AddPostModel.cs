using Microsoft.AspNetCore.Http;

namespace SmartBlog.Application.Models;

public class AddPostModel
{
    public string? Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Summary { get; set; }

    public string Content { get; set; }

    public IFormFile Image { get; set; }
}
