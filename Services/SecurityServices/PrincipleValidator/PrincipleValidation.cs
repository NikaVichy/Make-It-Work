using MakeItWork.Services.ModelServices.UserAuthRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MakeItWork.Services.SecurityServices.PrincipleValidator
{
    public static class PrincipleValidation
    {
        private const string LastValidatedClaimType = "last-validated";
        private const string UserStampClaimType = "user-stamp";
        public static async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var principal = context.Principal;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                context.RejectPrincipal();
                return;
            }

            var lastValidatedClaim = principal.FindFirstValue(LastValidatedClaimType);
            if (IsValidByTime(lastValidatedClaim, TimeSpan.FromMinutes(2))) return;

            var userAuthRepository = context.HttpContext.RequestServices.GetService<IUserAuthRepository>();
            if (userAuthRepository == null)
            {
                context.RejectPrincipal();
                return;
            }

            var stringedId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(stringedId, out int userId))
            {
                context.RejectPrincipal();
                return;
            }

            if (!await IsValidStamp(principal, userId, userAuthRepository, context.HttpContext.RequestAborted))
            {
                context.RejectPrincipal();
                return;
            }

            var newPrincipal = GetNewPrincipal(principal);

            context.ReplacePrincipal(newPrincipal);
            context.ShouldRenew = true;
        }

        private static bool IsValidByTime(string? lastValidatedContext, TimeSpan maxTime)
        {
            if (DateTimeOffset.TryParse(lastValidatedContext, out var lastValidated))
            {
                if (lastValidated > DateTimeOffset.UtcNow) return false;
                if (DateTimeOffset.UtcNow - lastValidated < maxTime) return true;
            }

            return false;
        }

        private static async Task<bool> IsValidStamp(
            ClaimsPrincipal principal,
            int userId,
            IUserAuthRepository userAuthRepository,
            CancellationToken ct = default)
        {
            var stampInClaim = principal.FindFirst(UserStampClaimType)?.Value;
            if (string.IsNullOrWhiteSpace(stampInClaim)) return false;

            return await userAuthRepository.TryValidateUserStampAsync(userId, stampInClaim, ct);
        }

        private static ClaimsPrincipal GetNewPrincipal(ClaimsPrincipal oldPrincipal)
        {
            var claims = oldPrincipal.Claims.Where(c => c.Type != LastValidatedClaimType).ToList();
            claims.Add(new Claim(LastValidatedClaimType, DateTimeOffset.UtcNow.ToString("O")));

            var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(ci);
        }
    }
}
