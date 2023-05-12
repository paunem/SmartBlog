namespace SmartBlog.Shared.Models;

public class Post
{
    public string? Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTimeOffset PostedOn { get; set; }

    public string Content { get; set; }

    public string Summary { get; set; }

    public string? ImageUrl { get; set; }
}
