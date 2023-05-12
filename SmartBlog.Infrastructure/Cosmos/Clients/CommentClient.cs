using SmartBlog.Application.Clients;
using SmartBlog.Infrastructure.Cosmos.Entities;
using SmartBlog.Shared.Models;

namespace SmartBlog.Infrastructure.Cosmos.Clients;

public class CommentClient : ICommentClient
{
    private readonly ICosmosDb cosmosDb;

    public CommentClient(ICosmosDb cosmosDb)
    {
        this.cosmosDb = cosmosDb;
    }

    public async Task<IEnumerable<Comment>> GetPostComments(string postId)
    {
        var comments = await cosmosDb.Comments.GetItemLinqQueryable<CommentEntity>()
            .Where(x => x.PostId == postId)
            .ExecuteAsync();

        return comments.Select(x => new Comment
        {
            Id = x.Id,
            PostId = x.PostId,
            Author = x.Author,
            PostedOn = x.PostedOn,
            Content = x.Content
        });
    }

    public async Task AddComment(Comment comment)
    {
        var postEntity = new CommentEntity
        {
            Id = Guid.NewGuid().ToString(),
            PostId = comment.PostId,
            Author = comment.Author,
            Content = comment.Content,
            PostedOn = DateTimeOffset.UtcNow
        };

        await cosmosDb.Comments.CreateItemAsync(postEntity);
    }
}
