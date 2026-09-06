using MakeItWork.Models;

namespace MakeItWork.Services.SecurityServices.Authenticator
{
    public interface IAuthenticatorService
    {
        Task<UserAuthModel?> TryValidateAuthModelAsync(string username, char[] password, CancellationToken ct = default);
    }
}
