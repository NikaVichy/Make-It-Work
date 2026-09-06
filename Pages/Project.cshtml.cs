using MakeItWork.DTOModels;
using MakeItWork.Services.ModelControllers.ProjectController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MakeItWork.Pages
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        private readonly IProjectController _projectController;

        public List<ProjectDTO> projectModels = [];

        public ProjectModel(IProjectController projectController)
        {
            _projectController = projectController;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            projectModels = await _projectController.GetProjectsForUserAsync(userId, HttpContext.RequestAborted);
        }
        private int GetUserId()
        {
            if (int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int uId)) return uId;
            return -1;
        }
    }
}
