using MakeItWork.DTOModels;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.ProjectService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MakeItWork.Pages
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        private readonly IProjectRepository _projectRepository = null!;
        private readonly IProjectService _projectService = null!;

        public List<ProjectDTO> projectModels = [];

        public ProjectModel(
            IProjectRepository projectRepository,
            IProjectService projectService)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            projectModels = await _projectRepository.GetProjectsForUserAsync(userId, HttpContext.RequestAborted);
        }

        public async Task<IActionResult> OnPostAsync(string? projectName, string? projectDescription)
        {
            if (string.IsNullOrWhiteSpace(projectName)) return BadRequest();

            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var dbProjectModel = await _projectService.TryCreateNewProjectAsync(
                userId, projectName, projectDescription ?? "", HttpContext.RequestAborted);

            return Redirect($"~/Project/{dbProjectModel.id}");
        }

        private int GetUserId()
        {
            if (int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int uId)) return uId;
            return -1;
        }
    }
}
