using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseService;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.UnitOfWork;

namespace MakeItWork.Services.ModelServices.ProjectService
{
    public class ProjectService : BaseService<ProjectModel, IProjectRepository>, IProjectService
    {
        private readonly IUnitOfWork _unitOfWork = null!;
        public ProjectService(
            IUnitOfWork unitOfWork,
            IProjectRepository projectRepository) : base(projectRepository)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectModel> TryCreateNewProjectAsync(
            int userId, 
            string projectName, 
            string projectDescription, 
            CancellationToken ct = default)
        {
            var projectModel = new ProjectModel
            {
                projectName = projectName,
                projectDescription = projectDescription,
                ownerId = userId,
            };
            var dbProjectModel = await _repository.CreateModelAsync(projectModel, saveImmediatelly: false, ct: ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return dbProjectModel;
        }
    }
}
