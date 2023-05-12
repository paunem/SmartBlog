using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SmartBlog.Client.Services;

namespace SmartBlog.Client.Pages.FileUpload;

public class FileUploadModel : ComponentBase
{
    [Inject] private IPostService PostService { get; set; }

    [Parameter] public string ImageSource { get; set; }

    [Parameter] public string Title { get; set; }

    public IBrowserFile BrowserFile { get; set; }

    protected string HoverClass { get; set; }

    protected bool ShowTitleEmpty { get; set; }

    protected void OnDragEnter() => HoverClass = "hover";

    protected void OnDragLeave() => HoverClass = string.Empty;

    public bool? IsGenerated { get; set; }

    protected async Task OnChange(InputFileChangeEventArgs e)
    {
        BrowserFile = e.File;
        using var stream = e.File.OpenReadStream(5000000);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ImageSource = $"data:{e.File.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

        IsGenerated = false;
    }

    protected async Task GenerateImage()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ShowTitleEmpty = true;
            return;
        }

        ShowTitleEmpty = false;
        ImageSource = await PostService.GenerateImage(Title);

        IsGenerated = true;
    }
}
