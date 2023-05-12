using Microsoft.AspNetCore.Components.Forms;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Services;

public interface IPostService
{
    Task<IReadOnlyList<Post>> GetAllPosts();

    Task<Post?> GetPostById(string id);

    Task<string> AddBlogPost(Post post);

    Task<string> AddBlogPost(Post post, IBrowserFile file);

    Task UpdateBlogPost(Post post);

    Task UpdateBlogPost(Post post, IBrowserFile file);

    Task DeleteBlogPost(string postId);

    Task<IReadOnlyList<Comment>> GetComments(string postId);

    Task AddComment(Comment comment);

    Task<GeneratedStory> GeneratePost();

    Task<string> GenerateImage(string prompt);

    Task<bool> Moderate(string input);

    Task<string> Translate(Post post);

    Task<string> Summarize(Post post);
}
