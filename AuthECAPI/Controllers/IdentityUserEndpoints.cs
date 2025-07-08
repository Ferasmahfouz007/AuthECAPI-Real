using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers;

public static class IdentityUserEndpoints
{
    public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/signup", CreateUser);
        app.MapPost("/siginin", SignIn);
        return app;
    }
    [AllowAnonymous]
    private static async Task<IResult> CreateUser(UserManager<AppUser> userManager, [FromBody] UserRegistrationModel userRegistrationModel)
    {
        AppUser user = new AppUser
        {
            UserName = userRegistrationModel.Email,
            Email = userRegistrationModel.Email,
            FullName = userRegistrationModel.FullName,
            Gender = userRegistrationModel.Gender,
            DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age)),
            LibraryId = userRegistrationModel.LibraryId
        };
        var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
        await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
        if (result.Succeeded)
            return Results.Ok(result);
        else
            return Results.BadRequest(result);
    }
    [AllowAnonymous]
    private static async Task<IResult> SignIn(UserManager<AppUser> userManager, [FromBody] LoginModel loginModel, IOptions<AppSettings> appSettings)
    {
        var user = await userManager.FindByEmailAsync(loginModel.Email);
        if (user == null || !(await userManager.CheckPasswordAsync(user, loginModel.Password)))
            return Results.BadRequest("Invalid credentials");

        var jwtSecret = appSettings.Value.JWTSecret;
        if (jwtSecret.Length != 32)
        {
            return Results.BadRequest("The JWT Secret key must be 32 bytes long.");
        }
        var roles = await userManager.GetRolesAsync(user);
        var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        //var signInKey = new SymmetricSecurityKey(
        //    Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));

        ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
        {
            new Claim("UserID", user.Id.ToString()),
            new Claim("Gender", user.Gender.ToString()),
            new Claim("Age",(DateTime.Now.Year - user.DOB.Year).ToString()),
            new Claim (ClaimTypes.Role,roles.First())
        });
        if (user.LibraryId != null)
        {
            claims.AddClaim(new Claim("LibraryId", user.LibraryId.ToString()!));
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            //Expires = DateTime.UtcNow.AddMinutes(10),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(
                signInKey,
                SecurityAlgorithms.HmacSha256Signature
                )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);// actuall token that we want to pass as a response to the post requst
        return Results.Ok(new { token });
    }
}

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public int? LibraryId { get; set; }
}
public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}