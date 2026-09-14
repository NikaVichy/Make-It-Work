using MakeItWork.Services.SecurityServices.Authorizer;
using MakeItWork.Services.UserRegistration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MakeItWork.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthorizerService _authenticatorService;

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public RegisterModel(IUserRegistrationService userRegistrationService, 
            IAuthorizerService authenticatorService)
        { 
            _authenticatorService = authenticatorService;
            _userRegistrationService = userRegistrationService; 
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

            Username = Username.Trim();
            Password = Password.Trim();

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return BadRequest();

            char[] charedPass = Password.ToArray();
            Password = null;

            try
            {
                var dbUserModel = await _userRegistrationService.RegisterUserAsync(Username, charedPass, HttpContext.RequestAborted);
                if (dbUserModel == null) return Unauthorized();

                await _authenticatorService.SignInAsync(HttpContext, dbUserModel.username);
                // TODO: REWORK
                return Redirect("/");
            }
            catch
            {
                Array.Clear(charedPass);
                return new StatusCodeResult(500);
            }
        }
    }
}
