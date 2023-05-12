using SmartBlog.Application.Clients;
using SmartBlog.Application.Models;
using System.Text;
using System.Text.Json;

namespace SmartBlog.Infrastructure.Clients;

public class ScriptClient : IScriptClient
{
    private readonly HttpClient client;

    public ScriptClient(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("script");
    }

    public async Task<string> AnswerQuestion(string question, string text)
    {
        var requestData = new AnswerQuestionRequest
        {
            Question = question,
            Text = text
        };

        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(string.Empty, content);

        return await response.Content.ReadAsStringAsync();
    }
}
