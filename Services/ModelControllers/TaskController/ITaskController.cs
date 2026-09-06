using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.BaseModelController;

namespace MakeItWork.Services.ModelControllers.TaskControllerService
{
    public interface ITaskController : IBaseModelController<TaskModel>
    {
        public Task<TaskModel?> CreateModelAsync(
            string taskName,
            int taskAuthorId,
            int taskProjectId,
            DateTimeOffset? dueTo,
            List<int>? assignedUsers,
            string? taskDescription,
            bool saveImmediatelly = true, 
            CancellationToken ct = default);
        public Task<List<TaskDTO>> GetTasksForUserAsync(int userId, CancellationToken ct = default);
        public Task UpdateTaskValues(int taskId, int userId, string taskName, string? taskDescription, DateTimeOffset? dueTo, CancellationToken ct = default);
        public Task ChangeTaskCompletionAsync(int taskId, int userId, CancellationToken ct = default);
        public Task ChangeTaskVisibilityAsync(int taskId, int userId, CancellationToken ct = default);
        public Task<TaskDTO?> GetDetailedTaskByIdAsync(int taskId, int userId, CancellationToken ct = default);
    }
}
