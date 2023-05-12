using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SmartBlog.Infrastructure.Options;

namespace SmartBlog.Infrastructure.Cosmos;

public class CosmosDb : ICosmosDb
{
	private readonly CosmosDbClient cosmosClient;
	private readonly CosmosOptions cosmosOptions;

	public CosmosDb(CosmosDbClient cosmosClient, IOptions<CosmosOptions> cosmosOptions)
	{
		this.cosmosClient = cosmosClient;
		this.cosmosOptions = cosmosOptions.Value;
    }

    public Container Posts => cosmosClient.CosmosClient.GetContainer(cosmosOptions.DatabaseName, Containers.Posts);

    public Container Comments => cosmosClient.CosmosClient.GetContainer(cosmosOptions.DatabaseName, Containers.Comments);
}
