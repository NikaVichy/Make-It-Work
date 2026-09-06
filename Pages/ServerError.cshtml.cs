using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MakeItWork.Pages
{
    public class ServerErrorModel : PageModel
    {
        public void OnGet() { ViewData["ShowHeader"] = false; }
    }
}
