using MakeItWork.DTOModels;

namespace MakeItWork.Services.ModelServices.TaskService
{
    public interface ITaskService
    {
        public Task TryCreateTaskAsync(
            string taskName,
            string? taskDescription,
            int taskAuthorId,
            int taskProjectId,
            DateTimeOffset? dueTo,
            List<int>? assignedUsers,
            CancellationToken ct = default);
        public Task UpdateTaskAsync(
            int taskId,
            string taskName,
            string? taskDescription,
            int userId,
            DateTimeOffset? dueTo,
            List<int> assignedUsers,
            CancellationToken ct = default);

        public Task ChangeTaskCompletionAsync(
            int taskId, 
            int userId, 
            CancellationToken ct = default);

        public Task ChangeTaskVisibilityAsync(
            int taskId,
            int userId,
            CancellationToken ct = default);
    }
}
