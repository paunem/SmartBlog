using Microsoft.AspNetCore.Components;
using SmartBlog.Client.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.Home;

public class HomeModel : ComponentBase
{
    [Inject] private IPostService PostService { get; set; }

    private const int PageSize = 5;

    protected IReadOnlyList<Post> AllPosts { get; set; } = new List<Post>();

    protected List<Post> VisiblePosts { get; set; } = new List<Post>();

    private int VisiblePostsCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadBlogPosts();
    }

    private async Task LoadBlogPosts()
    {
        var posts = (await PostService.GetAllPosts()).ToList();
        posts.Sort((x, y) => y.PostedOn.CompareTo(x.PostedOn));
        AllPosts = posts;
        VisiblePosts = posts.Take(PageSize).ToList();
        VisiblePostsCount = PageSize;
    }

    public void LoadMore()
    {
        var nextPagePosts = AllPosts.Skip(VisiblePostsCount).Take(PageSize).ToList();
        VisiblePosts.AddRange(nextPagePosts);
        VisiblePostsCount += PageSize;
    }
}
