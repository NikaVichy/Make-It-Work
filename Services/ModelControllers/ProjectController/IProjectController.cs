using MakeItWork.Services.ModelControllers.BaseModelController;
using MakeItWork.Models;
using MakeItWork.DTOModels;

namespace MakeItWork.Services.ModelControllers.ProjectController
{
    public interface IProjectController : IBaseModelController<ProjectModel>
    {
        Task<List<ProjectDTO>> GetProjectsForUserAsync(int userId, CancellationToken ct);
        Task<ProjectModel?> GetProjectByIdAsync(int projectId, int userId, CancellationToken ct);
        Task TransferOwnershipAsync(int projectId, int ownerId, int userId, CancellationToken ct);
        Task AddUserToProjectAsync(int projectId, int ownerId, int userId   , CancellationToken ct);
        Task RemoveUserFromProjectAsync(int projectId, int ownerId, int userId, CancellationToken ct);
        Task AddTaskToProjectAsync(int projectId, int ownerId, int taskId, CancellationToken ct);
        Task DeleteProjectAsync(int projectId, int ownerId, CancellationToken ct);
    }
}
