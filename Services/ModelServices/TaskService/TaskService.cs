using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseService;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.TaskRepository;
using MakeItWork.Services.ModelServices.UnitOfWork;

namespace MakeItWork.Services.ModelServices.TaskService
{
    public class TaskService : BaseService<TaskModel, ITaskRepository>, ITaskService
    {
        private readonly IProjectRepository _projectRepository = null!;
        private readonly IUnitOfWork _saverUnit = null!;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUnitOfWork saverUnit) : base(taskRepository)
        {
            _projectRepository = projectRepository;
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
            var userIdsWithAccess = (await _projectRepository.GetUserIdsWithAccessAsync(taskProjectId, ct)).ToHashSet();

            if (!userIdsWithAccess.Contains(taskAuthorId)) return;

            assignedUsers ??= [];
            var authorizedIds = assignedUsers
                .Where(u => userIdsWithAccess.Contains(u))
                .ToList();

            var taskModel = new TaskModel
            {
                taskName = taskName,
                taskDescription = taskDescription ?? "",
                taskAuthorId = taskAuthorId,
                taskProjectId = taskProjectId,
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

            var userIdsWithAccess = (await _projectRepository.GetUserIdsWithAccessAsync(
                dbTaskModel.taskProjectId,
                ct))
                    .ToHashSet();

            assignedUsers ??= [];

            var authorizedIds = assignedUsers
                .Where(u => userIdsWithAccess.Contains(u))
                .ToList();

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
            if (dbTaskModel == null) return;

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

            if (dbTaskModel.isHidden) return;

            dbTaskModel.isHidden = true;
            await _saverUnit.SaveChangesAsync(ct);
        }
    }
}
