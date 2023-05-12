using System.Text.Json.Serialization;

namespace SmartBlog.Application.Models;

public class AnswerQuestionRequest
{
    [JsonPropertyName("question")]
    public string Question { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
