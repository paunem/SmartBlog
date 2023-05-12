using AspNetCore.Identity.Stores;
using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartBlog.Application.Clients;
using SmartBlog.Application.Services;
using SmartBlog.Infrastructure.Clients;
using SmartBlog.Infrastructure.Cosmos;
using SmartBlog.Infrastructure.Cosmos.Clients;
using SmartBlog.Infrastructure.Options;
using System.Net.Http.Headers;
using System.Text;

namespace SmartBlog.Infrastructure.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCosmosClient(configuration);
        services.AddBlobStorage(configuration);
        services.AddOpenAIClient(configuration);
        services.AddScriptClient(configuration);

        services.AddAuthentication(configuration);

        services.AddTransient<IPostService, PostService>();
    }

    private static void AddCosmosClient(this IServiceCollection services, IConfiguration configuration)
    {
        var cosmosOptions = configuration.GetSection("CosmosDb").Get<CosmosOptions>();

        services.AddOptions<CosmosOptions>().Configure<IConfiguration>((options, configuration) =>
        {
            configuration.GetSection("CosmosDb").Bind(options);
        });

        var client = new CosmosClientBuilder(cosmosOptions.Account, cosmosOptions.Key)
            .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
            .Build();

        ValidateDb(client, cosmosOptions).Wait();

        services.AddSingleton(new CosmosDbClient(client));

        services.AddTransient<ICosmosDb, CosmosDb>();

        services.AddTransient<IPostClient, PostClient>();
        services.AddTransient<ICommentClient, CommentClient>();
    }

    private static async Task ValidateDb(CosmosClient client, CosmosOptions cosmosOptions)
    {
        var database = await client.CreateDatabaseIfNotExistsAsync(cosmosOptions.DatabaseName);

        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(Containers.Posts, "/partitionKey"));
        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(Containers.Comments, "/postId"));
    }

    private static void AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobConnectionString = configuration.GetConnectionString("BlobConnectionString");

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(blobConnectionString);
        });


        services.AddTransient<IBlobClient, BlobClient>();
    }

    private static void AddOpenAIClient(this IServiceCollection services, IConfiguration configuration)
    {
        var openAiOptions = configuration.GetSection("OpenAI").Get<OpenAiOptions>();

        services.AddHttpClient("openAI", client =>
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiOptions.Key);
            client.BaseAddress = new Uri(openAiOptions.Url);
        });

        services.AddTransient<IOpenAIClient, OpenAIClient>();
    }

    private static void AddScriptClient(this IServiceCollection services, IConfiguration configuration)
    {
        var scriptOptions = configuration.GetSection("Script").Get<ScriptOptions>();

        services.AddHttpClient("script", client =>
        {
            client.BaseAddress = new Uri(scriptOptions.Url);
        });

        services.AddTransient<IScriptClient, ScriptClient>();
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var cosmosOptions = configuration.GetSection("CosmosDb").Get<CosmosOptions>();

        services.Configure<IdentityStoresOptions>(options => options
                .UseAzureCosmosDB(cosmosOptions.ConnectionString, cosmosOptions.DatabaseName));

        services.AddIdentity<IdentityUser, IdentityRole>().AddAzureCosmosDbStores();

        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.ValidIssuer,
                ValidAudience = jwtOptions.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
            };
        });
    }
}
