using SmartBlog.Application.Models;
using SmartBlog.Application.Clients;
using System.Net.Http.Json;
using System.Text.Json;
using SmartBlog.Application.Common;
using SmartBlog.Shared.Models;

namespace SmartBlog.Infrastructure.Clients;

public class OpenAIClient : IOpenAIClient
{
    private readonly HttpClient client;

    public OpenAIClient(IHttpClientFactory httpClientFactory)
    {
        client = httpClientFactory.CreateClient("openAI");
    }

    //public async Task<TitleStory> GenerateText()
    //{
    //    var requestData = new OpenAIRequest
    //    {
    //        Model = "text-davinci-003",
    //        Prompt = Constants.CreateTitleStory,
    //        MaxTokens = 500
    //    };

    //    var response = await client.PostAsJsonAsync("completions", requestData);

    //    var content = await response.Content.ReadAsStringAsync();

    //    var jsonDoc = JsonDocument.Parse(content);
    //    var generatedResponse = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("text").ToString().TrimStart().TrimEnd();

    //    var titleAndStory = ExtractTitleAndStory(generatedResponse);

    //    return titleAndStory;
    //}

    public async Task<GeneratedStory> GenerateText()
    {
        var requestData = new OpenAIRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = Constants.CreateTitleStory
                }
            },
            MaxTokens = 1000
        };

        var response = await client.PostAsJsonAsync("chat/completions", requestData);

        var content = await response.Content.ReadAsStringAsync();

        var jsonDoc = JsonDocument.Parse(content);
        var generatedResponse = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").ToString().TrimStart().TrimEnd();

        var titleAndStory = ExtractTitleAndStory(generatedResponse);

        return titleAndStory;
    }

    public async Task<string> GenerateImage(string prompt)
    {
        var requestData = new OpenAIRequest
        {
            Prompt = prompt,
            Size = "512x512"
        };

        var response = await client.PostAsJsonAsync("images/generations", requestData);

        var content = await response.Content.ReadAsStringAsync();

        var jsonDoc = JsonDocument.Parse(content);
        var imageUrl = jsonDoc.RootElement.GetProperty("data")[0].GetProperty("url").ToString();

        return imageUrl;
    }

    public async Task<bool> Moderate(string input)
    {
        var requestData = new OpenAIRequest
        {
            Input = input,
        };

        var response = await client.PostAsJsonAsync("moderations", requestData);

        var content = await response.Content.ReadAsStringAsync();

        var jsonDoc = JsonDocument.Parse(content);
        return jsonDoc.RootElement.GetProperty("results")[0].GetProperty("flagged").GetBoolean();
    }

    public async Task<string> Translate(string content)
    {
        var requestData = new OpenAIRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = Constants.Translate + content
                }
            },
            MaxTokens = 1000
        };

        var response = await client.PostAsJsonAsync("chat/completions", requestData);

        var contentString = await response.Content.ReadAsStringAsync();

        var jsonDoc = JsonDocument.Parse(contentString);
        return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").ToString().TrimStart().TrimEnd();
    }

    public async Task<string> Summarize(string content)
    {
        var requestData = new OpenAIRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = Constants.Summarize + content
                }
            },
            MaxTokens = 1000
        };

        var response = await client.PostAsJsonAsync("chat/completions", requestData);

        var contentString = await response.Content.ReadAsStringAsync();

        var jsonDoc = JsonDocument.Parse(contentString);
        return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").ToString().TrimStart().TrimEnd();
    }

    private static GeneratedStory ExtractTitleAndStory(string inputString)
    {
        var titleEnd = inputString.IndexOf("\n\n");
        var title = inputString[7..titleEnd];
        var story = inputString[(titleEnd + 2)..];

        return new GeneratedStory
        {
            Title = title,
            Content = story
        };
    }
}
