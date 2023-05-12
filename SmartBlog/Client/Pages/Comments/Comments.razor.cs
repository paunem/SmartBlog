using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SmartBlog.Client.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.Comments;

public class CommentsModel : ComponentBase
{
    [Parameter] public string PostId { get; set; }

    [Inject] private IPostService PostService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected string NewComment { get; set; }

    protected List<Comment> Comments { get; set; } = new List<Comment>();

    protected bool IsFlagged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadComments();
    }

    private async Task LoadComments()
    {
        var comments = (await PostService.GetComments(PostId)).ToList();
        comments.Sort((x, y) => y.PostedOn.CompareTo(x.PostedOn));
        Comments = comments;
    }

    public async Task ModerateComment()
    {
        IsFlagged = await PostService.Moderate(NewComment);

        if (!IsFlagged)
        {
            await SaveComment();
        }
    }

    public async Task SaveComment()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var newComment = new Comment()
        {
            PostId = PostId,
            Author = authState.User.Identity.Name,
            Content = NewComment,
        };

        await PostService.AddComment(newComment);

        await LoadComments();

        NewComment = string.Empty;
    }
}
