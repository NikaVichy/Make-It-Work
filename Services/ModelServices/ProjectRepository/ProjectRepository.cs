using MakeItWork.ApplicationDbContext;
using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MakeItWork.Services.ModelServices.ProjectRepository
{
    public class ProjectRepository : BaseRepository<ProjectModel>, IProjectRepository
    {
        public ProjectRepository(ApplicationDb applicationDb) : base(applicationDb) { }

        private IQueryable<ProjectModel> _GetProjectsAccessibleForUser(int userId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.ownerId == userId || p.users.Any(u => u.id == userId));
        }

        private static readonly Expression<Func<ProjectModel, ProjectDTO>> _projectDtoCreation = p => new ProjectDTO
        {
            id = p.id,
            name = p.projectName,
        };

        public async Task<ProjectModel?> GetModelForUpdateAsync(int projectId, int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(p => p.id == projectId && p.ownerId == userId)
                .FirstOrDefaultAsync(ct);

        }

        public async Task<List<int>> GetUserIdsWithAccessAsync(int projectId, CancellationToken ct = default)
        {
            var projectQuery = _dbSet
                .Where(p => p.id == projectId);

            var ownerId = projectQuery.Select(p => p.ownerId);

            var usersInProject = projectQuery.SelectMany(p => p.users.Select(u => u.id));

            return await ownerId.Union(usersInProject).ToListAsync(ct);
        }

        public async Task<List<ProjectDTO>> GetProjectsForUserAsync(int userId, CancellationToken ct)
        {
            return await _GetProjectsAccessibleForUser(userId)
                .Select(_projectDtoCreation)
                .ToListAsync(ct);
        }
    }
}
