using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseService;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.UnitOfWork;

namespace MakeItWork.Services.ModelServices.ProjectService
{
    public class ProjectService : BaseService<ProjectModel, IProjectRepository>, IProjectService
    {
        private readonly IUnitOfWork _unitOfWork = null!;
        public ProjectService(IProjectRepository projectRepository) : base(projectRepository) { }

        public async Task TryCreateProjectAsync(
            int userId, 
            string projectName, 
            string projectDescription, 
            CancellationToken ct = default)
        {
            var newProject = new ProjectModel
            {
                projectName = projectName,
                projectDescription = projectDescription,
                ownerId = userId,
            };

            await _repository.CreateModelAsync(newProject, ct: ct);
        }
    }
}
