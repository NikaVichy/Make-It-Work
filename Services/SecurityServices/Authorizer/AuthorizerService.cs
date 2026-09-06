using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.UserAuthController;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MakeItWork.Services.SecurityServices.Authorizer
{
    public class AuthorizerService : IAuthorizerService
    {
        public async Task SignInAsync(HttpContext context, int userId, string username)
        {
            IUserAuthController userAuthController = context.RequestServices.GetRequiredService<IUserAuthController>();
            var dbUserAuthModel = await userAuthController.TryGetUserAuthModelAsync(userId, context.RequestAborted);
            if (dbUserAuthModel == null) return;

            await SignInAsync(context, userId, username, dbUserAuthModel);

        }

        public async Task SignInAsync(HttpContext context, int userId, string username, UserAuthModel userAuthModel)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("last-validated", DateTimeOffset.UtcNow.ToString("O")),
                new Claim("user-stamp", userAuthModel.SecurityStamp)
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
