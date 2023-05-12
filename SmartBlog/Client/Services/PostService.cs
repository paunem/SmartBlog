using Microsoft.AspNetCore.Components.Forms;
using SmartBlog.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SmartBlog.Client.Services;

public class PostService : IPostService
{
    private readonly HttpClient clientAuth;
    private readonly HttpClient clientPublic;

    public PostService(IHttpClientFactory httpClientFactory, HttpClient client)
    {
        clientAuth = httpClientFactory.CreateClient("ServerAuth");
        clientPublic = httpClientFactory.CreateClient("ServerPublic");
    }

    public async Task<IReadOnlyList<Post>> GetAllPosts()
    {
        return await clientPublic.GetFromJsonAsync<IReadOnlyList<Post>>("api/Blog/GetAllPosts");
    }

    public async Task<Post?> GetPostById(string id)
    {
        return await clientPublic.GetFromJsonAsync<Post>($"api/Blog/GetPost?id={id}");
    }

    public async Task<string> AddBlogPost(Post post, IBrowserFile file)
    {
        var fileContent = new StreamContent(file.OpenReadStream(5000000));

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var postJson = JsonSerializer.Serialize(post);

        using var multipartData = new MultipartFormDataContent
        {
            { fileContent, "Image", file.Name },
            { new StringContent(postJson, Encoding.UTF8, "application/json"), "Post" }
        };

        var response = await clientAuth.PostAsync("api/Blog/AddPostImage", multipartData);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return content;
    }

    public async Task<string> AddBlogPost(Post post)
    {
        var response = await clientAuth.PostAsJsonAsync("api/Blog/AddPost", post);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return content;
    }

    public async Task UpdateBlogPost(Post post)
    {
        await clientAuth.PatchAsJsonAsync($"api/Blog/UpdatePost", post);
    }

    public async Task UpdateBlogPost(Post post, IBrowserFile file)
    {
        var fileContent = new StreamContent(file.OpenReadStream(5000000));

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var postJson = JsonSerializer.Serialize(post);

        using var multipartData = new MultipartFormDataContent
        {
            { fileContent, "Image", file.Name },
            { new StringContent(postJson, Encoding.UTF8, "application/json"), "Post" }
        };

        await clientAuth.PatchAsync("api/Blog/UpdatePostImage", multipartData);
    }

    public async Task DeleteBlogPost(string postId)
    {
        await clientAuth.DeleteAsync($"api/Blog/DeletePost?id={postId}");
    }

    public async Task<IReadOnlyList<Comment>> GetComments(string postId)
    {
        return await clientPublic.GetFromJsonAsync<IReadOnlyList<Comment>>($"api/Blog/GetComments?postId={postId}");
    }

    public async Task AddComment(Comment comment)
    {
        var response = await clientAuth.PostAsJsonAsync("api/Blog/AddComment", comment);

        response.EnsureSuccessStatusCode();
    }

    public async Task<GeneratedStory> GeneratePost()
    {
        return await clientAuth.GetFromJsonAsync<GeneratedStory>("api/Helper/GeneratePost");
    }

    public async Task<string> GenerateImage(string prompt)
    {
        return await clientAuth.GetStringAsync($"api/Helper/GenerateImage?prompt={prompt}");
    }

    public async Task<bool> Moderate(string input)
    {
        var isFlagged = await clientAuth.GetStringAsync($"api/Helper/Moderate?input={input}");

        return bool.Parse(isFlagged);
    }

    public async Task<string> Translate(Post post)
    {
        var response = await clientPublic.PostAsJsonAsync($"api/Helper/Translate", post);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return content;
    }

    public async Task<string> Summarize(Post post)
    {
        var response =  await clientAuth.PostAsJsonAsync($"api/Helper/Summarize", post.Content);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return content;
    }
}
