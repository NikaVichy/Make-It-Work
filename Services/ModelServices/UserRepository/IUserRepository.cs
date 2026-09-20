using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;

namespace MakeItWork.Services.ModelServices.UserRepository
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<int> GetUserIdByUsernameAsync(string username, CancellationToken ct = default);
    }
}
