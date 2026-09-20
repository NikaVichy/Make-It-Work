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

        private readonly ITaskRepository _taskRepository;
        private readonly ITaskService _taskService;

        public IndexModel(ITaskRepository taskRepository, ITaskService taskService)
        {
            _taskRepository = taskRepository;
            _taskService = taskService;
        }

        public async Task<IActionResult> OnGet()
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            taskList = await _taskRepository.GetTasksForUserAsync(userId, HttpContext.RequestAborted);

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync(
            string taskName,
            string taskDescription,
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
                var taskAuthorId = GetUserId();
                await _taskService
                    .TryCreateTaskAsync(
                        taskName,
                        taskDescription,
                        taskAuthorId,
                        projectId,
                        dueTo,
                        assignedUsers,
                        saveImmediatelly: false,
                        ct);
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

            CancellationToken ct = HttpContext.RequestAborted;

            await _taskService.ChangeTaskCompletionAsync(
                taskId,
                userId,
                ct);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUpdateAsync(
            int taskId,

            string taskName,
            string taskDescription,
            DateTimeOffset? dueTo,
            List<int> assignedUsers)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var ct = HttpContext.RequestAborted;

            await _taskService
                .UpdateTaskAsync(
                    taskId,
                    taskName,
                    taskDescription,
                    userId,
                    dueTo,
                    assignedUsers,
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

        public async Task<IActionResult> OnGetTaskDetailsAsync(int taskId)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            Console.WriteLine(userId);

            var dtoModel = await _taskRepository.GetDetailedTaskByIdAsync(
                taskId,
                userId,
                HttpContext.RequestAborted);

            return new JsonResult(dtoModel);
        }

        public async Task<IActionResult> OnGetUserProjectsAsync([FromServices] IProjectRepository projectRepository)
        {
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var dtoProjectModels = await projectRepository
                .GetProjectsForUserAsync(userId, HttpContext.RequestAborted);

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
