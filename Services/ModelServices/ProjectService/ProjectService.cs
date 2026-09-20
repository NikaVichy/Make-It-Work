using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseService;
using MakeItWork.Services.ModelServices.ProjectRepository;

namespace MakeItWork.Services.ModelServices.ProjectService
{
    public class ProjectService : BaseService<ProjectModel, IProjectRepository>, IProjectService
    {
        public ProjectService(IProjectRepository projectRepository) : base(projectRepository) { }
    }
}
