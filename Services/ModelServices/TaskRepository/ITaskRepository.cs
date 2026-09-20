using MakeItWork.DTOModels;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;

namespace MakeItWork.Services.ModelServices.TaskRepository
{
    public interface ITaskRepository : IBaseRepository<TaskModel>
    {
        public Task<List<TaskDTO>> GetTasksForUserAsync(int userId, CancellationToken ct = default);
        public Task<TaskDTO?> GetDetailedTaskByIdAsync(int taskId, int userId, CancellationToken ct = default);
        public Task<TaskModel?> GetForUpdateAsync(int taskid, int userId, CancellationToken ct = default);
        public void ReplaceUsersByIds(
            TaskModel dbTaskModel, 
            List<int> authorizedUsers);
    }
}
