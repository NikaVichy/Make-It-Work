using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;

namespace MakeItWork.Services.ModelServices.ProjectRepository
{
    public interface IProjectRepository : IBaseRepository<ProjectModel>
    {
        Task<ProjectModel?> GetModelForUpdateAsync(int projectId, int userId, CancellationToken ct = default);
        Task<List<int>> GetUserIdsWithAccessAsync(int projectId, CancellationToken ct = default);
        Task<List<ProjectDTO>> GetProjectsForUserAsync(int userId, CancellationToken ct);
        //Task<ProjectModel?> GetProjectByIdAsync(int projectId, int userId, CancellationToken ct);
        //Task TransferOwnershipAsync(int projectId, int ownerId, int userId, CancellationToken ct);
        //Task AddUserToProjectAsync(int projectId, int ownerId, int userId, CancellationToken ct);
        //Task RemoveUserFromProjectAsync(int projectId, int ownerId, int userId, CancellationToken ct);
        //Task AddTaskToProjectAsync(int projectId, int ownerId, int taskId, CancellationToken ct);
        //Task DeleteProjectAsync(int projectId, int ownerId, CancellationToken ct);
    }
}
