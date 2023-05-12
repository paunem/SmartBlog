namespace SmartBlog.Infrastructure.Cosmos.Entities;

public class CommentEntity
{
    public string Id { get; set; }

    public string PostId { get; set; }

    public string Author { get; set; }

    public DateTimeOffset PostedOn { get; set; }

    public string Content { get; set; }
}
