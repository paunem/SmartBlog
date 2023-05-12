using Microsoft.AspNetCore.Components;
using SmartBlog.Client.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.ViewPost;

public class ViewPostModel : ComponentBase
{
    [Inject] private IPostService PostService { get; set; }

    [Parameter] public string PostId { get; set; }

    protected Post Post { get; set; } = new Post();

    protected override async Task OnInitializedAsync()
    {
        await LoadBlogPost();
    }

    private async Task LoadBlogPost()
    {
        Post = await PostService.GetPostById(PostId);
    }

    public async Task TranslatePost()
    {
        var translatedContent = await PostService.Translate(Post);

        Post.Content = translatedContent;
    }
}