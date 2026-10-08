using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;

namespace MakeItWork.Services.ModelServices.ProjectRepository
{
    public interface IProjectRepository : IBaseRepository<ProjectModel>
    {
        Task<bool> UserHasAccessAsync(int projectId, int userId, CancellationToken ct = default);
        Task<ProjectModel?> GetModelForUpdateAsync(int projectId, int userId, CancellationToken ct = default);
        Task<List<int>> GetUserIdsWithAccessAsync(int projectId, CancellationToken ct = default);
        Task<List<ProjectDTO>> GetProjectsForUserAsync(int userId, CancellationToken ct);
    }
}
