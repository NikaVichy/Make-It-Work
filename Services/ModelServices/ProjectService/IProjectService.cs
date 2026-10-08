using MakeItWork.Models;

namespace MakeItWork.Services.ModelServices.ProjectService
{
    public interface IProjectService
    {
        public Task<ProjectModel> TryCreateNewProjectAsync(
            int userId, 
            string projectName, 
            string projectDescription, 
            CancellationToken ct = default);
    }
}
