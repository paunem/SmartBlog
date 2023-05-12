namespace SmartBlog.Infrastructure.Cosmos.Entities;

public class PostEntity
{
    public string Id { get; set; }

    public string PartitionKey { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTimeOffset PostedOn { get; set; }

    public string Content { get; set; }

    public string Summary { get; set; }

    public string ImageUrl { get; set; }
}
