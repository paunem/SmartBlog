using SmartBlog.Shared.Models;

namespace SmartBlog.Application.Clients;

public interface ICommentClient
{
    Task<IEnumerable<Comment>> GetPostComments(string postId);

    Task AddComment(Comment comment);
}
