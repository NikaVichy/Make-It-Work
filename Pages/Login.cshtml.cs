using MakeItWork.Services.SecurityServices.Authenticator;
using MakeItWork.Services.SecurityServices.Authorizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace MakeItWork.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthenticatorService _authenticationService;
        private readonly IAuthorizerService _authorizationService;

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IAuthenticatorService userAuthorizerService, IAuthorizerService authenticatorService)
        {
            _authenticationService = userAuthorizerService;
            _authorizationService = authenticatorService;
        }

        public IActionResult OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true) { return Redirect("/"); }

            ViewData["ShowHeader"] = false;

            return Page();
        }

        [EnableRateLimiting("auth")]
        public async Task<IActionResult> OnPostAsync()
        {
            if (User?.Identity?.IsAuthenticated == true) { return Redirect("/"); }

            Username = Username.Trim();
            Password = Password.Trim();

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return BadRequest("Username and/or password are invalid");

            char[] charedPassword = Password.ToArray();
            Password = "";

            try
            {
                var isAuthenticated = await _authenticationService.TryValidateAuthModelAsync(Username, charedPassword);
                Array.Clear(charedPassword);

                if (!isAuthenticated) return new UnauthorizedResult();

                await _authorizationService.SignInAsync(HttpContext, Username);
                return Redirect("/");
            }
            catch
            {
                Array.Clear(charedPassword);
                return new StatusCodeResult(500);
            }
        }
    }
}
