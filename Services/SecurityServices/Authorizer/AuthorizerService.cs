using MakeItWork.Services.ModelServices.UserAuthRepository;
using MakeItWork.Services.ModelServices.UserRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MakeItWork.Services.SecurityServices.Authorizer
{
    public class AuthorizerService : IAuthorizerService
    {
        public async Task SignInAsync(HttpContext context, string username, CancellationToken ct = default)
        {
            IUserAuthRepository userAuthRepository = context.RequestServices.GetRequiredService<IUserAuthRepository>();
            IUserRepository userRepository = context.RequestServices.GetRequiredService<IUserRepository>();

            var userId = await userRepository.GetUserIdByUsernameAsync(username);
            if (userId == 0) return;

            var securityStamp = await userAuthRepository.TryGetSecurityStampAsync(userId, ct);
            if (String.IsNullOrWhiteSpace(securityStamp)) return;

            await SignInAsync(context, userId, username, securityStamp);
        }

        public async Task SignInAsync(HttpContext context, int userId, string username, string securityStamp)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("last-validated", DateTimeOffset.UtcNow.ToString("O")),
                new Claim("user-stamp", securityStamp)
            };

            AuthenticationProperties AuthProperties = new()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7),
                IssuedUtc = DateTime.UtcNow,
                AllowRefresh = true,
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), AuthProperties);
        }
        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
