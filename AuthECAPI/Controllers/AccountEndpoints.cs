using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthECAPI.Controllers;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/UserProfile", GetUserProfile);
        return app;
    }
    [Authorize]
    private static async Task<IResult> GetUserProfile(ClaimsPrincipal user, UserManager<AppUser> userManager)
    {
        string userId = user.Claims.First(x => x.Type == "UserID").Value;
        AppUser? userDetials = await userManager.FindByIdAsync(userId);
        return Results.Ok(new
        {
            FullName = userDetials?.FullName,
            Email = userDetials?.Email
        });
    }
}
