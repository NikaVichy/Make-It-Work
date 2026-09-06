using MakeItWork.Models;
using MakeItWork.Services.SecurityServices.Authorizer;
using MakeItWork.Services.SecurityServices.Totp.TotpAuthenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MakeItWork.Pages
{
    public class Login_TotpModel : PageModel
    {
        private readonly ITotpAuthenticatorService _userTotpAuthorizerService;
        private readonly IAuthorizerService _authenticatorService;

        [BindProperty]
        public int SecurityCode { get; set; }

        public Login_TotpModel(ITotpAuthenticatorService userTotpAuthorizerService,
            IAuthorizerService authenticatorService)
        {
            _userTotpAuthorizerService = userTotpAuthorizerService;
            _authenticatorService = authenticatorService;
        }

        public IActionResult OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true) { return Redirect("/"); }

            ViewData["ShowHeader"] = false;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User?.Identity?.IsAuthenticated == true) { return Redirect("/"); }

            var dateTimeObject = HttpContext.Items["auth-attempt"];
            if (!IsDateTimeValid(dateTimeObject)) return RedirectToPage("/Login");

            var userIdString = HttpContext.Items["preauth-id"]?.ToString();
            var username = HttpContext.Items["preauth-username"]?.ToString();

            if (!int.TryParse(userIdString, out int userId)) return RedirectToPage("/Login");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            UserAuthModel? dbUserAuthModel = await _userTotpAuthorizerService.TryAuthorizeTotpAsync(
                SecurityCode, 
                userId, 
                HttpContext.RequestAborted);
            if (dbUserAuthModel == null) return Unauthorized();

            await _authenticatorService.SignInAsync(HttpContext, userId, username, dbUserAuthModel);

            return Redirect("/");
        }

        private bool IsDateTimeValid(object? dateTimeObject)
        {
            if (dateTimeObject == null) return false;
            if (!DateTime.TryParse((string)dateTimeObject, out DateTime dateTime)) return false;
            if (DateTime.Compare(dateTime.AddMinutes(5), DateTime.UtcNow) < 0) return false;

            return true;

        }
    }
}
