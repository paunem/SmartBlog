using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartBlog.Application.Clients;
using SmartBlog.Application.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Functions;

public class GeneratePost
{
    private readonly ILogger logger;
    private readonly IOpenAIClient openAIClient;
    private readonly IPostService postService;

    public GeneratePost(ILoggerFactory loggerFactory, IOpenAIClient openAIClient, IPostService postService)
    {
        logger = loggerFactory.CreateLogger<GeneratePost>();
        this.openAIClient = openAIClient;
        this.postService = postService;
    }

    [Function("GeneratePost")]
    public async Task Run([TimerTrigger("%PostSchedule%")] TimerInfo myTimer)
    {
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

        var titleStory = await openAIClient.GenerateText();
        var imageUrl = await openAIClient.GenerateImage(titleStory.Title);
        var summary = await openAIClient.Summarize(titleStory.Content);

        var post = new Post
        {
            Title = titleStory.Title,
            Content = titleStory.Content,
            Summary = summary,
            Author = "GPT3",
            ImageUrl = imageUrl,
        };

        await postService.AddBlogPost(post);
    }
}
