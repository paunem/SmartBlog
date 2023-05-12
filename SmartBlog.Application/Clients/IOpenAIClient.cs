using SmartBlog.Shared.Models;

namespace SmartBlog.Application.Clients;

public interface IOpenAIClient
{
    Task<GeneratedStory> GenerateText();

    Task<string> GenerateImage(string prompt);

    Task<bool> Moderate(string input);

    Task<string> Translate(string content);

    Task<string> Summarize(string content);
}