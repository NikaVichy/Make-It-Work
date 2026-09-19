using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;
using MakeItWork.Services.ModelServices.BaseService;
using MakeItWork.Services.ModelServices.ProjectService;
using MakeItWork.Services.ModelServices.TaskRepository;

namespace MakeItWork.Services.ModelServices.TaskService
{
    public class TaskService : BaseService<TaskModel, ITaskRepository>, ITaskService
    {
        private readonly IProjectService _projectService = null!;
        private readonly IUnitOfWork _saverUnit = null!;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectService projectService,
            IUnitOfWork saverUnit) : base(taskRepository)
        {
            _projectService = projectService;
            _saverUnit = saverUnit;
        }

        public async Task TryCreateTaskAsync(
            string taskName,
            string? taskDescription,
            int taskAuthorId,
            int taskProjectId,
            DateTimeOffset? dueTo,
            List<int>? assignedUsers,
            bool saveImmediatelly = true,
            CancellationToken ct = default)
        {
            if (!await _projectService.UserHasAccessAsync(taskAuthorId, taskProjectId, ct))
                return;

            assignedUsers ??= [];
            var authorizedIds = await _projectService
                .UsersHaveAccessAsync(assignedUsers, taskProjectId, ct);
            

            var taskModel = new TaskModel
            {
                taskName = taskName,
                taskDescription = taskDescription ?? "",
                taskAuthorId = taskAuthorId,
                dueTo = dueTo
            };

            var dbTaskModel = await _repository.CreateModelAsync(taskModel, saveImmediatelly: false, ct: ct);
            _repository.ReplaceUsersByIds(dbTaskModel, authorizedIds);

            await _saverUnit.SaveChangesAsync(ct);
        }

        public async Task UpdateTaskAsync(
            int taskId,
            string taskName,
            string? taskDescription,
            int userId,
            DateTimeOffset? dueTo,
            List<int>? assignedUsers,
            CancellationToken ct = default)
        {
            var dbTaskModel = await _repository.GetForUpdateAsync(taskId, userId, ct);
            if (dbTaskModel == null) return;

            assignedUsers ??= [];
            var authorizedIds = await _projectService
                .UsersHaveAccessAsync(assignedUsers, dbTaskModel.taskProjectId, ct);

            dbTaskModel.taskName = taskName;
            dbTaskModel.taskDescription = taskDescription ?? "";
            dbTaskModel.dueTo = dueTo;

            _repository.ReplaceUsersByIds(dbTaskModel, authorizedIds);

            await _saverUnit.SaveChangesAsync(ct);
        }

        public async Task ChangeTaskCompletionAsync(
            int taskId,
            int userId,
            CancellationToken ct = default)
        {
            var dbTaskModel = await _repository.GetForUpdateAsync(taskId, userId, ct);
            if(dbTaskModel == null) return;

            dbTaskModel.isCompleted = !dbTaskModel.isCompleted;
            await _saverUnit.SaveChangesAsync(ct);
        }

        public async Task ChangeTaskVisibilityAsync(
            int taskId,
            int userId,
            CancellationToken ct = default)
        {
            var dbTaskModel = await _repository.GetForUpdateAsync(taskId, userId, ct);
            if (dbTaskModel == null) return;

            dbTaskModel.isHidden = true;
            await _saverUnit.SaveChangesAsync(ct);
        }
    }
}
