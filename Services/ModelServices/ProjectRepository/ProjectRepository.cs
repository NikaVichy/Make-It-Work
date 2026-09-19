using MakeItWork.ApplicationDbContext;
using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace MakeItWork.Services.ModelServices.ProjectRepository
{
    public class ProjectRepository : BaseRepository<ProjectModel>, IProjectRepository
    {
        public ProjectRepository(ApplicationDb applicationDb) : base(applicationDb) { }
        public async Task<List<ProjectDTO>> GetProjectsForUserAsync(int userId, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.ownerId == userId || p.users.Any(u => u.id == userId))
                .Select(p => new ProjectDTO
                {
                    id = p.id,
                    name = p.projectName
                })
                .ToListAsync(ct);

        }
        public async Task<ProjectModel?> GetProjectByIdAsync(int projectId, int userId, CancellationToken ct)
        {
            return await _dbSet.Where(p => p.id == projectId && (p.ownerId == userId || p.users.Any(u => u.id == userId)))
                .FirstOrDefaultAsync(ct);
        }
        public async Task TransferOwnershipAsync(int projectId, int curOwner, int newOwner, CancellationToken ct)
        {
            if (curOwner == newOwner) return;

            var userInProject = await _dbSet.Where(p => p.id == projectId)
                .SelectMany(p => p.users)
                .AnyAsync(u => u.id == newOwner, ct);

            if (!userInProject) return;

            await _dbSet.Where(p => p.id == projectId && p.ownerId == curOwner).
                ExecuteUpdateAsync(m => m
                    .SetProperty(p => p.ownerId, newOwner), ct);
        }
        public async Task AddUserToProjectAsync(int projectId, int ownerId, int userId, CancellationToken ct)
        {
            var project = await _dbSet.Where(p => p.id == projectId && p.ownerId == ownerId)
                .Include(p => p.users)
                .FirstOrDefaultAsync(ct);
            if (project == null) return;
            if (project.users.Any(u => u.id == userId)) return;

            var userStub = new UserModel() { id = userId };
            _applicationDb.Entry(userStub).State = EntityState.Unchanged;

            project.users.Add(userStub);
            await _applicationDb.SaveChangesAsync(ct);
        }
        public async Task RemoveUserFromProjectAsync(int projectId, int ownerId, int userId, CancellationToken ct)
        {
            var project = await _dbSet.Where(p => p.id == projectId && p.ownerId == ownerId)
                .Include(p => p.users)
                .FirstOrDefaultAsync(ct);
            if (project == null) return;

            var userModel = project.users.FirstOrDefault(u => u.id == userId);
            if (userModel == null) return;

            project.users.Remove(userModel);
            await _applicationDb.SaveChangesAsync(ct);
        }
        public async Task AddTaskToProjectAsync(int projectId, int ownerId, int taskId, CancellationToken ct)
        {
            var taskInProject = await _dbSet
                .Where(p => p.id == projectId)
                .SelectMany(p => p.tasks)
                .AnyAsync(t => t.id == taskId);
            if (taskInProject) return;

            var project = await _dbSet.Where(p => p.id == projectId && p.ownerId == ownerId)
                .Include(p => p.tasks)
                .FirstOrDefaultAsync(ct);
            if (project == null) return;

            var taskStub = new TaskModel() { id = taskId };
            _applicationDb.Entry(taskStub).State = EntityState.Unchanged;

            project.tasks.Add(taskStub);
            await _applicationDb.SaveChangesAsync(ct);
        }
        public async Task DeleteProjectAsync(int projectId, int ownerId, CancellationToken ct)
        {
            await _dbSet.Where(p => p.id == projectId && p.ownerId == ownerId)
                .ExecuteDeleteAsync(ct);
        }
    }
}
