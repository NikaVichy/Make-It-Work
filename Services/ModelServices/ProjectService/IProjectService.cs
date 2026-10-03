namespace MakeItWork.Services.ModelServices.ProjectService
{
    public interface IProjectService
    {
        public Task TryCreateProjectAsync(
            int userId, 
            string projectName, 
            string projectDescription, 
            CancellationToken ct = default);
    }
}
