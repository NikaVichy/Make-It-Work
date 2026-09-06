using MakeItWork.ApplicationDbContext;
using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.BaseModelController;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MakeItWork.Services.ModelControllers.TaskControllerService
{
    public class TaskControllerService : BaseModelController<TaskModel>, ITaskController
    {
        public TaskControllerService(ApplicationDb applicationDb) : base(applicationDb) { }
        private IQueryable<TaskModel> _GetTasksAccessibleToUserQuery(int userId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(t => t.taskProject.ownerId == userId || t.taskProject.users.Any(u => u.id == userId));
        }
        private IQueryable<TaskModel> _GetTaskByIdQuery(int taskId, int userId)
        {
            return _GetTasksAccessibleToUserQuery(userId)
                .Where(t => t.id == taskId);
        }
        private IQueryable<TaskModel> _GetTaskForAuthorQuery(int taskId, int userId)
        {
            return _GetTaskByIdQuery(taskId, userId)
                .Where(t => t.taskAuthorId == userId);
        }
        private IQueryable<TaskModel> _GetTasksForAssigneeQuery(int userId)
        {
            return _GetTasksAccessibleToUserQuery(userId)
                .Where(t => (!t.assignedUsers.Any() && t.taskAuthorId == userId) || t.assignedUsers.Any(u => u.id == userId));
        }
        private IQueryable<TaskModel> _GetTaskForAssigneeQuery(int taskId, int userId)
        {
            return _GetTasksForAssigneeQuery(userId)
                .Where(t => t.id == taskId);
        }

        private static readonly Expression<Func<TaskModel, TaskDTO>> _taskDtoCreation = t => new TaskDTO
        {
            id = t.id,
            taskName = t.taskName,
            taskDescription = t.taskDescription,
            createdAt = t.createdAt,
            dueTo = t.dueTo,
            authorId = t.taskAuthorId,
            authorName = t.taskAuthor.username,
            projectId = t.taskProjectId,
            projectName = t.taskProject.projectName
        };

        public async Task<TaskModel?> CreateModelAsync(
            string taskName,
            int taskAuthorId,
            int taskProjectId,
            DateTimeOffset? dueTo,
            List<int>? assignedUsers,
            string? taskDescription,
            bool saveImmediatelly = true,
            CancellationToken ct = default)
        {
            var projectAccess = await _applicationDb.Set<ProjectModel>()
                .Where(p => p.id == taskProjectId)
                .Select(p => new
                {
                    p.ownerId,
                    userIds = p.users.Select(u => u.id)
                })
                .FirstOrDefaultAsync(ct);
            if (projectAccess == null) return null;

            var allowedUsersIds = projectAccess.userIds.ToHashSet();
            allowedUsersIds.Add(projectAccess.ownerId);

            if (!allowedUsersIds.Contains(taskAuthorId)) return null;

            assignedUsers ??= [];
            List<UserModel> authorizedAssignee = assignedUsers
                .Distinct()
                .Where(uId => allowedUsersIds.Contains(uId))
                .Select(uId =>
                {
                    var userStub = new UserModel { id = uId };
                    _applicationDb.Attach(userStub);
                    return userStub;
                })
                .ToList();

            var taskModel = new TaskModel()
            {
                taskName = taskName,
                taskDescription = taskDescription ?? "",
                taskAuthorId = taskAuthorId,
                taskProjectId = taskProjectId,
                dueTo = dueTo,
                assignedUsers = authorizedAssignee,
            };

            return await base.CreateModelAsync(taskModel, saveImmediatelly, ct);
        }
        public async Task<List<TaskDTO>> GetTasksForUserAsync(int userId, CancellationToken ct = default)
        {
            return await _GetTasksForAssigneeQuery(userId)
                .OrderBy(t => t.dueTo == null)
                .ThenBy(t => t.dueTo)
                .Select(_taskDtoCreation)
                .ToListAsync(ct);
        }
        
        public async Task UpdateTaskValues(
            int taskId, 
            int userId, 
            string taskName, 
            string? taskDescription, 
            DateTimeOffset? dueTo, 
            CancellationToken ct = default)
        {
            await _GetTaskForAuthorQuery(taskId, userId)
                .ExecuteUpdateAsync(m => m
                .SetProperty(t => t.taskName, taskName)
                .SetProperty(t => t.taskDescription, taskDescription ?? "")
                .SetProperty(t => t.dueTo, dueTo?.ToUniversalTime()), ct);
        }
        public async Task ChangeTaskCompletionAsync(int taskId, int userId, CancellationToken ct = default)
        {
            await _GetTaskByIdQuery(taskId, userId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(t => t.isCompleted, t => !t.isCompleted), ct);
        }

        public async Task ChangeTaskVisibilityAsync(int taskId, int userId, CancellationToken ct = default)
        {
            await _GetTaskByIdQuery(taskId, userId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(t => t.isHidden, true), ct);
        }

        public async Task<TaskDTO?> GetDetailedTaskByIdAsync(int taskId, int userId, CancellationToken ct = default)
        {
            return await _GetTaskByIdQuery(taskId, userId)
                .Select(_taskDtoCreation)
                .FirstOrDefaultAsync(ct);
        }   

        public async Task AssignUserToTask(int taskId, int userId, int assignedUserId, CancellationToken ct)
        {
            var taskInfo = await _GetTaskForAuthorQuery(taskId, userId)
                .Where(t => t.taskProject.users.Any(u => u.id == assignedUserId))
                .Where(t => !t.assignedUsers.Any(u => u.id == assignedUserId))
                .AnyAsync(ct);
            if (!taskInfo) return;

            var userStub = new UserModel { id = assignedUserId };
            _applicationDb.Attach(userStub);

            var taskStub = new TaskModel { id = taskId };
            _applicationDb.Attach(taskStub);

            taskStub.assignedUsers.Add(userStub);

            await _applicationDb.SaveChangesAsync(ct);
        }
    }
}
