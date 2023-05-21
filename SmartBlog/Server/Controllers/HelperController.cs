using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBlog.Application.Clients;
using SmartBlog.Shared.Models;

namespace SmartBlog.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HelperController : ControllerBase
{
    private readonly IOpenAIClient openAIClient;

    public HelperController(IOpenAIClient openAIService)
    {
        openAIClient = openAIService;
    }

    [HttpGet("GeneratePost")]
    public async Task<IActionResult> GeneratePost()
    {
        var story = await openAIClient.GenerateText();

        return Ok(story);
    }

    [HttpGet("GenerateImage")]
    public async Task<IActionResult> GenerateImage(string prompt)
    {
        var imageUrl = await openAIClient.GenerateImage(prompt);

        return Ok(imageUrl);
    }

    [HttpGet("Moderate")]
    public async Task<IActionResult> Moderate(string input)
    {
        var isFlagged = await openAIClient.Moderate(input);

        return Ok(isFlagged);
    }

    [AllowAnonymous]
    [HttpPost("Translate")]
    public async Task<IActionResult> Translate(Post post)
    {
        var result = await openAIClient.Translate(post.Content);

        return Ok(result);
    }

    [HttpPost("Summarize")]
    public async Task<IActionResult> Summarize([FromBody] string content)
    {
        var result = await openAIClient.Summarize(content);

        return Ok(result);
    }

    [HttpPost("Rephrase")]
    public async Task<IActionResult> Rephrase([FromBody] string content)
    {
        var result = await openAIClient.Rephrase(content);

        return Ok(result);
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> Edit([FromBody] string content)
    {
        var result = await openAIClient.Edit(content);

        return Ok(result);
    }
}
