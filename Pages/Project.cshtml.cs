using MakeItWork.DTOModels;
using MakeItWork.Services.ModelServices.ProjectRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MakeItWork.Pages
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;

        public List<ProjectDTO> projectModels = [];

        public ProjectModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            projectModels = await _projectRepository.GetProjectsForUserAsync(userId, HttpContext.RequestAborted);
        }
        private int GetUserId()
        {
            if (int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int uId)) return uId;
            return -1;
        }
    }
}
