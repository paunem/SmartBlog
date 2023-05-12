using SmartBlog.Application.Models;
using SmartBlog.Shared.Models;

namespace SmartBlog.Application.Services;

public interface IPostService
{
    Task<PagedResponse<Post>> GetPagedPosts(string continuationToken);

    Task<IEnumerable<Post>> GetAllPosts();

    Task<Post?> GetPostById(string id);

    Task<string> AddBlogPost(AddPostModel postData);

    Task<string> AddBlogPost(Post newBlogPost);

    Task UpdateBlogPost(Post post);

    Task UpdateBlogPost(AddPostModel postData);

    Task DeleteBlogPost(string postId);

    Task<IEnumerable<Comment>> GetComments(string postId);

    Task AddComment(Comment comment);
}
