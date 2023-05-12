using Microsoft.Azure.Cosmos;

namespace SmartBlog.Infrastructure.Cosmos;

public interface ICosmosDb
{
    Container Posts { get; }

    Container Comments { get; }
}
