using MakeItWork.DTOModels;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.TaskRepository;
using MakeItWork.Services.ModelServices.TaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MakeItWork.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<TaskDTO> taskList = [];
        public List<ProjectDTO> projectList = [];

        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskService _taskService;

        public IndexModel(
            IProjectRepository projectRepository,
            ITaskRepository taskRepository, 
            ITaskService taskService)
        {
            _projectRepository = projectRepository;

            _taskRepository = taskRepository;
            _taskService = taskService;
        }

        public async Task<IActionResult> OnGet()
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var ct = HttpContext.RequestAborted;

            taskList = await _taskRepository.GetTasksForUserAsync(userId, ct);
            projectList = await _projectRepository.GetProjectsForUserAsync(userId, ct);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string taskName, int projectId)
        {
            var userId = GetUserId();
            if(userId == -1) return Unauthorized();

            var ct = HttpContext.RequestAborted;

            await _taskService.TryCreateTaskAsync(taskName, "", userId, projectId, null, null, ct: ct);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            CancellationToken ct = HttpContext.RequestAborted;

            await _taskService.ChangeTaskCompletionAsync(
                taskId,
                userId,
                ct);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            CancellationToken ct = HttpContext.RequestAborted;

            await _taskService.ChangeTaskVisibilityAsync(
                taskId,
                userId,
                ct);

            return RedirectToPage();
        }

        // Not sure if ill leave it or change or something else

        //public async Task<IActionResult> OnGetTaskDetailsAsync(int taskId)
        //{
        //    var userId = GetUserId();
        //    if (userId == -1) return Unauthorized();

        //    Console.WriteLine(userId);

        //    var dtoModel = await _taskRepository.GetDetailedTaskByIdAsync(
        //        taskId,
        //        userId,
        //        HttpContext.RequestAborted);

        //    return new JsonResult(dtoModel);
        //}


        // Same here, prlly use for filters?

        //public async Task<IActionResult> OnGetUserProjectsAsync([FromServices] IProjectRepository projectRepository)
        //{
        //    var userId = GetUserId();
        //    if (userId == -1) return Unauthorized();

        //    var dtoProjectModels = await projectRepository
        //        .GetProjectsForUserAsync(userId, HttpContext.RequestAborted);

        //    return new JsonResult(dtoProjectModels);
        //}

        private int GetUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId)
                ? userId
                : -1;
        }
    }
}
