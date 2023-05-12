namespace SmartBlog.Shared.Models;

public class RegistrationResponse
{
    public bool IsSuccessful { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public string Token { get; set; }
}
