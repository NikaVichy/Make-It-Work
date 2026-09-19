namespace MakeItWork.Services.ModelServices.ProjectService
{
    public interface IProjectService
    {
        public Task<bool> UserHasAccessAsync(
            int taskAuthorId, 
            int taskProjectId, 
            CancellationToken ct = default);

        public Task<List<int>> UsersHaveAccessAsync(
            List<int> usersToCheck, 
            int taskProjectId, 
            CancellationToken ct = default);
    }
}
