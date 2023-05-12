using Microsoft.Extensions.Configuration;
using SmartBlog.Application.Clients;
using SmartBlog.Application.Models;
using SmartBlog.Shared;
using SmartBlog.Shared.Models;

namespace SmartBlog.Application.Services;

public class PostService : IPostService
{
    private readonly IPostClient postClient;
    private readonly ICommentClient commentClient;
    private readonly IBlobClient blobClient;
    private readonly IScriptClient scriptClient;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public PostService(IPostClient postClient, ICommentClient commentClient, IBlobClient blobClient, IScriptClient scriptClient, HttpClient httpClient, IConfiguration configuration)
    {
        this.postClient = postClient;
        this.commentClient = commentClient;
        this.blobClient = blobClient;
        this.scriptClient = scriptClient;
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<PagedResponse<Post>> GetPagedPosts(string continuationToken)
    {
        return await postClient.GetPagedPosts(continuationToken);
    }

    public async Task<IEnumerable<Post>> GetAllPosts()
    {
        return await postClient.GetAllPosts();
    }

    public async Task<Post?> GetPostById(string id)
    {
        return await postClient.GetPostById(id);
    }

    public async Task<string> AddBlogPost(Post post)
    {
        var newImage = await httpClient.GetStreamAsync(post.ImageUrl);
        post.ImageUrl = await blobClient.UploadFile(newImage);

        return await postClient.AddPost(post);
    }

    public async Task<string> AddBlogPost(AddPostModel postData)
    {
        var imageUrl = await blobClient.UploadFile(postData.Image);

        var post = new Post
        {
            Id = postData.Id,
            Author = postData.Author,
            Title = postData.Title,
            Summary = postData.Summary,
            Content = postData.Content,
            ImageUrl = imageUrl,
        };

        return await postClient.AddPost(post);
    }

    public async Task UpdateBlogPost(AddPostModel postData)
    {
        var post = await postClient.GetPostById(postData.Id);
        var imageFileName = GetFileNameFromUrl(post.ImageUrl);
        await blobClient.DeleteFile(imageFileName);

        var imageUrl = await blobClient.UploadFile(postData.Image);

        var updatedPost = new Post
        {
            Id = postData.Id,
            Author = postData.Author,
            Title = postData.Title,
            Summary = postData.Summary,
            Content = postData.Content,
            ImageUrl = imageUrl,
        };

        await postClient.UpdatePost(updatedPost);
    }

    public async Task UpdateBlogPost(Post post)
    {
        var oldPost = await postClient.GetPostById(post.Id);

        if (!oldPost.ImageUrl.Equals(post.ImageUrl, StringComparison.OrdinalIgnoreCase)) 
        {
            var imageFileName = GetFileNameFromUrl(oldPost.ImageUrl);
            await blobClient.DeleteFile(imageFileName);

            var newImage = await httpClient.GetStreamAsync(post.ImageUrl);
            post.ImageUrl = await blobClient.UploadFile(newImage);
        }

        await postClient.UpdatePost(post);
    }

    public async Task DeleteBlogPost(string postId)
    {
        var post = await postClient.GetPostById(postId);
        var imageFileName = GetFileNameFromUrl(post.ImageUrl);
        await blobClient.DeleteFile(imageFileName);

        await postClient.DeletePost(postId);
    }

    public async Task<IEnumerable<Comment>> GetComments(string postId)
    {
        return await commentClient.GetPostComments(postId);
    }

    public async Task AddComment(Comment comment)
    {
        await commentClient.AddComment(comment);

        if (configuration.GetValue<bool>(Constants.AutoComments))
        {
            AnswerQuestion(comment);
        }        
    }

    public static string GetFileNameFromUrl(string url)
    {
        var segments = url.Split('/');
        var fileName = segments.LastOrDefault();

        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        return fileName;
    }

    private async Task AnswerQuestion(Comment comment)
    {
        var post = await postClient.GetPostById(comment.PostId);

        var answer = await scriptClient.AnswerQuestion(comment.Content, post.Content);

        var answerComment = new Comment
        {
            PostId = comment.PostId,
            Author = "T5-auto",
            Content = answer
        };

        await commentClient.AddComment(answerComment);
    }
}
