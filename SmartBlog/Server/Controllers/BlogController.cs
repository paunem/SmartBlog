using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBlog.Application.Models;
using SmartBlog.Application.Services;
using SmartBlog.Server.Binders;
using SmartBlog.Shared;
using SmartBlog.Shared.Models;

namespace SmartBlog.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly IPostService postService;

    public BlogController(IPostService postService)
    {
        this.postService = postService;
    }

    [AllowAnonymous]
    [HttpGet("GetPagedPosts")]
    public async Task<IActionResult> GetPagedPosts()
    {
        var continuationToken = Request.Headers["X-Continuation-Token"];
        var posts = await postService.GetPagedPosts(continuationToken);

        return Ok(posts);
    }

    [AllowAnonymous]
    [HttpGet("GetAllPosts")]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await postService.GetAllPosts();

        return Ok(posts);
    }

    [AllowAnonymous]
    [HttpGet("GetPost")]
    public async Task<IActionResult> GetBlogPostById(string id)
    {
        var blogPost = await postService.GetPostById(id);

        return blogPost is null ? NotFound() : Ok(blogPost);
    }

    [Authorize(Roles = Constants.Owner)]
    [HttpPost("AddPost")]
    public async Task<IActionResult> AddBlogPost(Post newBlogPost)
    {
        var postId = await postService.AddBlogPost(newBlogPost);

        return Created($"api/Blog/GetPost?={postId}", postId);
    }

    [Authorize(Roles = Constants.Owner)]
    [HttpPost("AddPostImage")]
    public async Task<IActionResult> AddBlogPost(
        [ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "Post")] AddPostModel postData)
    {
        var postId = await postService.AddBlogPost(postData);

        return Created($"api/Blog/GetPost?={postId}", postId);
    }

    [Authorize(Roles = Constants.Owner)]
    [HttpPatch("UpdatePostImage")]
    public async Task<IActionResult> UpdateBlogPost(
        [ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "Post")] AddPostModel postData)
    {
        await postService.UpdateBlogPost(postData);

        return Ok();
    }

    [Authorize(Roles = Constants.Owner)]
    [HttpPatch("UpdatePost")]
    public async Task<IActionResult> UpdateBlogPost(Post updatedBlogPost)
    {
        await postService.UpdateBlogPost(updatedBlogPost);

        return Ok();
    }

    [Authorize(Roles = Constants.Owner)]
    [HttpDelete("DeletePost")]
    public async Task<IActionResult> DeleteBlogPost(string id)
    {
        await postService.DeleteBlogPost(id);

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("GetComments")]
    public async Task<IActionResult> GetComments(string postId)
    {
        var comments = await postService.GetComments(postId);

        return Ok(comments);
    }

    [Authorize]
    [HttpPost("AddComment")]
    public async Task<IActionResult> AddComment(Comment comment)
    {
        await postService.AddComment(comment);

        return Ok();
    }
}
