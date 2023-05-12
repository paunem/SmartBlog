using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SmartBlog.Client.Services;

namespace SmartBlog.Client.Shared;

public class TopNavModel : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    public async void ExecuteLogout()
    {
        await AuthService.Logout();
        NavigationManager.NavigateToLogout("/");
    }
}
