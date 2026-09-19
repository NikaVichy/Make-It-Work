using MakeItWork.Models;

namespace MakeItWork.Services.ModelServices.UserAuthRepository
{
    public interface IUserAuthRepository
    {
        Task<bool> CheckIfAnyPresentAsync(int id, CancellationToken ct = default);
        Task<bool> TryValidateUserStampAsync(int id, string userStamp, CancellationToken ct = default);
        Task<UserAuthModel?> CreateUserAuthModelAsync(
            UserAuthModel userAuthModel,
            bool saveImmediatelly,
            CancellationToken ct = default);
        Task<bool> IsUserAuthenticatedAsync(int userId, char[] password, CancellationToken ct = default);
        Task<string> TryGetSecurityStampAsync(int userId, CancellationToken ct = default);
    }
}
