namespace SmartBlog.Client;

using Blazored.LocalStorage;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService localStorage;

    public AuthHeaderHandler(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await localStorage.GetItemAsync<string>("authToken", cancellationToken);

        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}
