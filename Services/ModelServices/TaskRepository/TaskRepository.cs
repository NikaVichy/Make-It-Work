using MakeItWork.ApplicationDbContext;
using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MakeItWork.Services.ModelServices.TaskRepository
{
    public class TaskRepository : BaseRepository<TaskModel>, ITaskRepository
    {
        public TaskRepository(ApplicationDb applicationDb) : base(applicationDb) { }
        private IQueryable<TaskModel> _GetTasksAccessibleForUser(int userId)
        {
            return _dbSet
                .Where(t => !t.isHidden)
                .Where(t => t.taskProject.ownerId == userId || t.taskProject.users.Any(u => u.id == userId));
        }
        private IQueryable<TaskModel> _GetAccessibleTasksForAssignee(int userId)
        {
            return _GetTasksAccessibleForUser(userId)
                // If (no users assigned AND user if author) OR user is assigned
                .Where(t => t.assignedUsers.Count == 0 && t.taskAuthorId == userId ||
                    t.assignedUsers.Any(u => u.id == userId));
        }
        private IQueryable<TaskModel> _GetAccessibleTaskById(int taskId, int userId)
        {
            return _GetTasksAccessibleForUser(userId)
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

        public async Task<List<TaskDTO>> GetTasksForUserAsync(int userId, CancellationToken ct = default)
        {
            return await _GetAccessibleTasksForAssignee(userId)
                .Where(t => !t.isCompleted)
                .Select(_taskDtoCreation)
                .OrderBy(t => t.dueTo == null)
                .ThenBy(t => t.dueTo)
                .ToListAsync(ct);
        }

        public async Task<TaskDTO?> GetDetailedTaskByIdAsync(int taskId, int userId, CancellationToken ct = default)
        {
            return await _GetAccessibleTaskById(taskId, userId)
                .Select(_taskDtoCreation)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TaskModel?> GetForUpdateAsync(int taskId, int userId, CancellationToken ct = default)
        {
            return await _GetAccessibleTaskById(taskId, userId)
                .FirstOrDefaultAsync(ct);
        }

        public void ReplaceUsersByIds(TaskModel dbTaskModel, List<int> authorizedUsers)
        {
            var currentIds = dbTaskModel
                .assignedUsers
                .Select(u => u.id)
                .ToHashSet();

            var targetIds = authorizedUsers
                .Distinct()
                .ToHashSet();

            var usersToRemove = dbTaskModel
                .assignedUsers
                .Where(u => !targetIds.Contains(u.id))
                .ToList();

            var usersToAdd = targetIds
                .Except(currentIds)
                .Select(id => new UserModel { id = id })
                .ToList();

            foreach(var user in usersToRemove)
                dbTaskModel.assignedUsers.Remove(user);

            _applicationDb.AttachRange(usersToAdd);
            dbTaskModel.assignedUsers.AddRange(usersToAdd);
        }
    }
}
