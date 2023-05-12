using SmartBlog.Shared.Models;

namespace SmartBlog.Application.Clients;

public interface IPostClient
{
    Task<PagedResponse<Post>> GetPagedPosts(string continuationToken);

    Task<IEnumerable<Post>> GetAllPosts();

    Task<Post?> GetPostById(string postId);

    Task<string> AddPost(Post post);

    Task UpdatePost(Post post);

    Task DeletePost(string postId);

}
