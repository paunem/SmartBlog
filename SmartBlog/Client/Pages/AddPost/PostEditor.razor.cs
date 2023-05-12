using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SmartBlog.Client.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.AddPost;

public class AddPostModel : ComponentBase
{
    [Inject] private IPostService PostService { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Parameter] public string PostId { get; set; }

    private const string WaitingMessage = "Waiting...";

    protected Post Post { get; set; } = new Post();

    protected FileUpload.FileUpload? FileUploadRef { get; set; }

    protected bool IsEdit => !string.IsNullOrEmpty(PostId);

    protected bool ShowValidationError { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(PostId))
        {
            await LoadPost();
        }
    }

    public async Task SavePost()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        if (!FileUploadRef.IsGenerated.HasValue)
        {
            ShowValidationError = true;
            return;
        }

        var IsImageGenerated = FileUploadRef.IsGenerated.Value;

        Post.Author = authState.User.Identity.Name;
        Post.ImageUrl = IsImageGenerated ? FileUploadRef.ImageSource : null;

        var postId = IsImageGenerated
            ? await PostService.AddBlogPost(Post)
            : await PostService.AddBlogPost(Post, FileUploadRef.BrowserFile);

        NavigationManager.NavigateTo($"post/{postId}");
    }

    public async Task UpdatePost()
    {
        if (FileUploadRef.IsGenerated.HasValue)
        {
            if (FileUploadRef.IsGenerated.Value)
            {
                Post.ImageUrl = FileUploadRef.ImageSource;
                await PostService.UpdateBlogPost(Post);
            }
            else
            {
                await PostService.UpdateBlogPost(Post, FileUploadRef.BrowserFile);
            }
        }
        else
        {
            await PostService.UpdateBlogPost(Post);
        }

        NavigationManager.NavigateTo($"post/{PostId}");
    }

    public async Task DeletePost()
    {
        await PostService.DeleteBlogPost(Post.Id);

        NavigationManager.NavigateTo("/");
    }

    private async Task LoadPost()
    {
        Post = await PostService.GetPostById(PostId);
    }

    public async Task GenerateStory()
    {
        Post.Title = WaitingMessage;
        Post.Content = WaitingMessage;

        var generatedStory = await PostService.GeneratePost();

        Post.Title = generatedStory.Title;
        Post.Summary = generatedStory.Summary;
        Post.Content = generatedStory.Content;
    }

    public async Task Summarize()
    {

        Post.Summary = WaitingMessage;

        var summary = await PostService.Summarize(Post);

        Post.Summary = summary;
    }
}