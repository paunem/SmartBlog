using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SmartBlog.Shared.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartBlog.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions options;
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly ILocalStorageService localStorage;

    public AuthService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
    {
        client = httpClientFactory.CreateClient("ServerPublic");
        options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        this.authStateProvider = authStateProvider;
        this.localStorage = localStorage;
    }

    public async Task<AuthResponse> Login(LoginInfo userForAuthentication)
    {
        var content = JsonSerializer.Serialize(userForAuthentication);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        var authResult = await client.PostAsync("api/auth/login", bodyContent);
        var authContent = await authResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponse>(authContent, options);

        if (!authResult.IsSuccessStatusCode)
            return result;

        await localStorage.SetItemAsync("authToken", result.Token);
        ((AuthStateProvider)authStateProvider).NotifyUserAuthentication(result.Token);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
        return new AuthResponse { IsAuthSuccessful = true };
    }

    public async Task<RegistrationResponse> RegisterUser(LoginInfo userForRegistration)
    {
        var content = JsonSerializer.Serialize(userForRegistration);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        var registrationResult = await client.PostAsync("api/auth/register", bodyContent);
        var registrationContent = await registrationResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RegistrationResponse>(registrationContent, options);

        if (!registrationResult.IsSuccessStatusCode)
            return result;

        await localStorage.SetItemAsync("authToken", result.Token);
        ((AuthStateProvider)authStateProvider).NotifyUserAuthentication(result.Token);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

        return new RegistrationResponse { IsSuccessful = true };
    }

    public async Task Logout()
    {
        await localStorage.RemoveItemAsync("authToken");
        ((AuthStateProvider)authStateProvider).NotifyUserLogout();
        client.DefaultRequestHeaders.Authorization = null;
    }
}
