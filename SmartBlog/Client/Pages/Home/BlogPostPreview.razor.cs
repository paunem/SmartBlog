using Microsoft.AspNetCore.Components;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.Home;

public class BlogPostPreviewModel : ComponentBase
{
    [Parameter] public Post Post { get; set; }

    [Parameter] public bool AlignRight { get; set; }
}
