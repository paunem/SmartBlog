using Microsoft.Azure.Cosmos;
using SmartBlog.Application.Clients;
using SmartBlog.Infrastructure.Cosmos.Entities;
using SmartBlog.Shared.Models;

namespace SmartBlog.Infrastructure.Cosmos.Clients;

public class PostClient : IPostClient
{
    private readonly ICosmosDb cosmosDb;

    public PostClient(ICosmosDb cosmosDb)
    {
        this.cosmosDb = cosmosDb;
    }

    public async Task<PagedResponse<Post>> GetPagedPosts(string continuationToken)
    {
        var requestOptions = new QueryRequestOptions
        {
            MaxItemCount = 5
        };

        (string ContinuationToken, List<PostEntity> Items) = await cosmosDb.Posts.GetItemLinqQueryable<PostEntity>(
            requestOptions: requestOptions, continuationToken: continuationToken)
            .ExecutePagedAsync();

        return new PagedResponse<Post>
        {
            ContinuationToken = ContinuationToken,
            Items = Items.Select(x => new Post
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author,
                PostedOn = x.PostedOn,
                Content = x.Content,
                Summary = x.Summary,
                ImageUrl = x.ImageUrl
            })
        };
    }

    public async Task<IEnumerable<Post>> GetAllPosts()
    {
        var posts = await cosmosDb.Posts.GetItemLinqQueryable<PostEntity>().ExecuteAsync();

        return posts.Select(x => new Post
        {
            Id = x.Id,
            Title = x.Title,
            Author = x.Author,
            PostedOn = x.PostedOn,
            Content = x.Content,
            Summary = x.Summary,
            ImageUrl = x.ImageUrl
        });
    }

    public async Task<Post?> GetPostById(string postId)
    {
        var response = await cosmosDb.Posts.ReadItemAsync<PostEntity>(postId, new PartitionKey(postId));

        if (response is null)
            return null;

        var post = response.Resource;

        return new Post
        {
            Id = post.Id,
            Title = post.Title,
            Author = post.Author,
            PostedOn = post.PostedOn,
            Content = post.Content,
            Summary = post.Summary,
            ImageUrl = post.ImageUrl
        };
    }

    public async Task<string> AddPost(Post post)
    {
        var id = Guid.NewGuid().ToString();
        var postEntity = new PostEntity
        {
            Id = id,
            PartitionKey = id,
            Title = post.Title,
            Author = post.Author,
            Content = post.Content,
            Summary = post.Summary,
            PostedOn = DateTimeOffset.UtcNow,
            ImageUrl = post.ImageUrl
        };

        var response = await cosmosDb.Posts.CreateItemAsync(postEntity);

        return response.Resource.Id;
    }

    public async Task UpdatePost(Post post)
    {
        var operations = new List<PatchOperation>
        {
            PatchOperation.Add($"/title", post.Title),
            PatchOperation.Add($"/summary", post.Summary),
            PatchOperation.Add($"/content", post.Content),
            PatchOperation.Add($"/imageUrl", post.ImageUrl)
        };

        await cosmosDb.Posts.PatchItemAsync<PostEntity>(post.Id, new PartitionKey(post.Id), operations);
    }

    public async Task DeletePost(string postId)
    {
        await cosmosDb.Posts.DeleteItemAsync<PostEntity>(postId, new PartitionKey(postId));
    }
}
