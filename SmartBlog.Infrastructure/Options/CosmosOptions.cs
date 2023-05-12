namespace SmartBlog.Infrastructure.Options;

public class CosmosOptions
{
    public string Account { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;
}
