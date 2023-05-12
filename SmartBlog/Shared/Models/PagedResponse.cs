namespace SmartBlog.Shared.Models;

public class PagedResponse<T>
{
    public string ContinuationToken { get; set;}

    public IEnumerable<T> Items { get; set;}
}
