using SmartBlog.Shared.Models;

namespace SmartBlog.Client.Services;

public interface IAuthService
{
    Task<RegistrationResponse> RegisterUser(LoginInfo userForRegistration);

    Task<AuthResponse> Login(LoginInfo userForAuthentication);

    Task Logout();
}
