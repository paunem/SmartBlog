using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartBlog.Shared;
using SmartBlog.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartBlog.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfigurationSection jwtSettings;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        jwtSettings = configuration.GetSection("Jwt");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginInfo userForAuthentication)
    {
        var user = await userManager.FindByEmailAsync(userForAuthentication.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            return Unauthorized(new AuthResponse { ErrorMessage = "Invalid Authentication" });

        var token = await GetToken(user);

        return Ok(new AuthResponse { IsAuthSuccessful = true, Token = token });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(LoginInfo userForRegistration)
    {
        if (userForRegistration is null || !ModelState.IsValid)
            return BadRequest();

        var user = new IdentityUser { UserName = userForRegistration.Email, Email = userForRegistration.Email };

        var result = await userManager.CreateAsync(user, userForRegistration.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            return BadRequest(new RegistrationResponse { Errors = errors });
        }

        await userManager.AddToRoleAsync(user, Constants.User);

        var token = await GetToken(user);

        return Ok(new RegistrationResponse { IsSuccessful = true, Token = token });
    }

    private async Task<string> GetToken(IdentityUser user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email)
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["ValidIssuer"],
            audience: jwtSettings["ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddDays(int.Parse(jwtSettings["ExpiryInDays"])),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }
}
