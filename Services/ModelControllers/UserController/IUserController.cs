using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.BaseModelController;

namespace MakeItWork.Services.ModelControllers.UserController
{
    public interface IUserController : IBaseModelController<UserModel>
    {
        Task<UserModel?> GetUserModelByUsernameAsync(string? username, CancellationToken ct = default);
    }
}
