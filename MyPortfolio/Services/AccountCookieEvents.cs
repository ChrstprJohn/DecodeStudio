using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyPortfolio.Services;

public class AccountCookieEvents : CookieAuthenticationEvents
{
    private readonly AccountService _accountService;

    public AccountCookieEvents(AccountService accountService)
    {
        _accountService = accountService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var idClaim = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = int.TryParse(idClaim, out var id)
            ? await _accountService.GetActiveUserAsync(id)
            : null;

        // Recheck each request so deactivation and role changes invalidate old cookies.
        if (user is null || user.Role != context.Principal?.FindFirstValue(ClaimTypes.Role) ||
            AccountService.GetPasswordVersion(user) != context.Principal?.FindFirstValue("PasswordVersion"))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        else if (user.FirstName != context.Principal?.Identity?.Name)
        {
            var identity = (ClaimsIdentity)context.Principal!.Identity!;
            var oldName = identity.FindFirst(ClaimTypes.Name);
            if (oldName is not null) identity.RemoveClaim(oldName);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            context.ReplacePrincipal(new ClaimsPrincipal(identity));
            context.ShouldRenew = true;
        }
    }
}
