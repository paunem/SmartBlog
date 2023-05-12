using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SmartBlog.Client.Services;
using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Pages.Login;

public class LoginModel : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IAuthService AuthService { get; set; }

    protected LoginInfo UserDetails { get; set; } = new LoginInfo();

    protected IEnumerable<string> RegisterErrors { get; set; } = Enumerable.Empty<string>();

    protected bool ShowLoginFailed { get; set; }

    protected bool ShowRegisterFailed { get; set; }


    public async Task ExecuteLogin()
    {
        ShowLoginFailed = false;
        ShowRegisterFailed = false;

        var result = await AuthService.Login(UserDetails);

        if (!result.IsAuthSuccessful)
        {
            ShowLoginFailed = true;
        }
        else
        {
            NavigationManager.NavigateTo("/");
        }
    }

    public async Task ExecuteRegister()
    {
        ShowLoginFailed = false;
        ShowRegisterFailed = false;

        var result = await AuthService.RegisterUser(UserDetails);

        if (!result.IsSuccessful)
        {
            RegisterErrors = result.Errors;
            ShowRegisterFailed = true;
        }
        else
        {
            NavigationManager.NavigateTo("/");
        }
    }

    public async void ExecuteLogout()
    {
        await AuthService.Logout();
        NavigationManager.NavigateToLogout("/");
    }
}
