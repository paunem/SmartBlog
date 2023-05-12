using Microsoft.Azure.Cosmos;

namespace SmartBlog.Infrastructure.Cosmos;

public class CosmosDbClient
{
	public CosmosDbClient(CosmosClient cosmosClient)
	{
		CosmosClient = cosmosClient;
	}

	public CosmosClient CosmosClient { get; }
}
