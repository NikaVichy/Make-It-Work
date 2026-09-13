using MakeItWork.Models;

namespace MakeItWork.Services.ModelControllers.UserAuthController
{
    public interface IUserAuthController
    {
        Task<bool> CheckIfAnyPresentAsync(int id, CancellationToken ct = default);
        Task<UserAuthModel?> TryGetUserAuthModelAsync(int id, CancellationToken ct = default);
        Task<bool> TryValidateUserStampAsync(int id, string userStamp, CancellationToken ct = default);
        Task<UserAuthModel?> CreateUserAuthModelAsync(
            UserAuthModel userAuthModel, 
            bool saveImmediatelly, 
            CancellationToken ct = default);
        Task<UserAuthModel?> UpdateUserAuthModelAsync(UserAuthModel userAuthModel, CancellationToken ct = default);

    }
}
