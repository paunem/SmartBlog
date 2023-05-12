namespace SmartBlog.Infrastructure.Options;

public class JwtOptions
{
    public string SecurityKey { get; set; } = string.Empty;

    public string ValidIssuer { get; set; } = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;

    public string ExpiryInDays { get; set; } = string.Empty;
}
