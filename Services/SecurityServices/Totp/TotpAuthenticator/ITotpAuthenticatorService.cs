using MakeItWork.Models;

namespace MakeItWork.Services.SecurityServices.Totp.TotpAuthenticator
{
    public interface ITotpAuthenticatorService
    {
        public Task <UserAuthModel?> TryAuthorizeTotpAsync(
            int totpCode, 
            int preauthUserId, 
            CancellationToken ct = default);
    }
}
