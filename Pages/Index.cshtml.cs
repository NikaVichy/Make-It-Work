using MakeItWork.DTOModels;
using MakeItWork.Services.ModelControllers.ProjectController;
using MakeItWork.Services.ModelControllers.TaskControllerService;
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

        private readonly ITaskController _taskControllerService;

        public IndexModel(ITaskController taskControllerService)
        {
            _taskControllerService = taskControllerService;
        }

        public async Task<IActionResult> OnGet()
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            taskList = await _taskControllerService.GetTasksForUserAsync(userId, HttpContext.RequestAborted);

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync(
            string taskName, 
            string? taskDescription,
            int projectId,
            List<int> assignedUsers,
            DateTimeOffset? dueTo)
        {
            if (string.IsNullOrWhiteSpace(taskName)) 
                return RedirectToPage();

            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            CancellationToken ct = HttpContext.RequestAborted;

            try
            {
                var task = await _taskControllerService.CreateModelAsync(
                    taskName: taskName,
                    taskDescription: taskDescription,
                    taskAuthorId: userId,
                    taskProjectId: projectId,
                    dueTo: dueTo,
                    assignedUsers: assignedUsers,
                    ct: ct);
            }
            catch
            {
                // TODO: do smth
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            await _taskControllerService.ChangeTaskCompletionAsync(
                taskId, 
                userId, 
                HttpContext.RequestAborted);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUpdateAsync(int taskId, string? taskName, string? taskDescription, DateTimeOffset? dueTo)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            await _taskControllerService.UpdateTaskValues(
                taskId, 
                userId, 
                taskName,
                taskDescription,
                dueTo, 
                HttpContext.RequestAborted);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            await _taskControllerService.ChangeTaskVisibilityAsync(
                taskId, 
                userId, 
                HttpContext.RequestAborted);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTaskDetailsAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var dtoModel = await _taskControllerService.GetDetailedTaskByIdAsync(
                taskId, 
                userId, 
                HttpContext.RequestAborted);

            return new JsonResult(dtoModel);
        }

        public async Task<IActionResult> OnGetUserProjectsAsync([FromServices] IProjectController projectController)
        {
            var userId = GetUserId();
            if(userId == -1) return Unauthorized();

            var dtoProjectModels = await projectController.GetProjectsForUserAsync(userId, HttpContext.RequestAborted);

            return new JsonResult(dtoProjectModels);
        }

        private int GetUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId)
                ? userId
                : -1;
        }
    }
}
