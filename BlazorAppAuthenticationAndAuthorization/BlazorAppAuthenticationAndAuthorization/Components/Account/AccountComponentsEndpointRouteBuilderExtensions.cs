using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazorAppAuthenticationAndAuthorization.Components.Account;

internal static class AccountComponentsEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapAdditionalAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/Logout", async (
             ClaimsPrincipal user,
             HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return TypedResults.LocalRedirect("/login");
        }).RequireAuthorization();

        return endpoints;
    }
}
