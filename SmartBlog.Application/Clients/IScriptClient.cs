namespace SmartBlog.Application.Clients;

public interface IScriptClient
{
    Task<string> AnswerQuestion(string question, string text);
}
