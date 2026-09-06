using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.UserAuthController;
using MakeItWork.Services.SecurityServices.Totp.TotpEncryptor;
using MakeItWork.Services.SecurityServices.Totp.TotpGenerator;
using System.Security.Cryptography;

namespace MakeItWork.Services.SecurityServices.Totp.TotpAuthenticator
{
    public class TotpAuthenticatorService : ITotpAuthenticatorService
    {
        private readonly IUserAuthController _userAuthController;

        private readonly TotpEncryptorService _totpCryptoService;
        private readonly ITotpGenerator _totpService;

        public TotpAuthenticatorService(IUserAuthController userAuthController, 
            TotpEncryptorService totpCryptoService, 
            ITotpGenerator totpService)
        {
            _userAuthController = userAuthController;
            _totpCryptoService = totpCryptoService;
            _totpService = totpService;
        }

        public async Task<UserAuthModel?> TryAuthorizeTotpAsync(
            int totpCode, 
            int preauthUserId, 
            CancellationToken ct = default)
        {
            try
            {
                var dbUserAuthModel = await _userAuthController.TryGetUserAuthModelAsync(
                    preauthUserId, 
                    ct);
                if (dbUserAuthModel == null || dbUserAuthModel.EncryptedTotpSecret == null) return null;

                if (dbUserAuthModel.IsLocked())
                    return null;

                var decryptedTotpSecret = _totpCryptoService.Decrypt(dbUserAuthModel.EncryptedTotpSecret);

                bool isValid = _totpService.VerifyTotp(decryptedTotpSecret, GetCharedCode(totpCode));
                CryptographicOperations.ZeroMemory(decryptedTotpSecret);

                if (!isValid)
                {
                    dbUserAuthModel.AddAuthAttempt();
                    await _userAuthController.UpdateUserAuthModelAsync(dbUserAuthModel, ct);

                    return null;
                }

                if (dbUserAuthModel.TryClearAuthAttempts())
                    await _userAuthController.UpdateUserAuthModelAsync(dbUserAuthModel, ct);

                return dbUserAuthModel;
            }
            catch
            {
                return null;
                throw;
            }
        }

        private char[] GetCharedCode(int code)
        {
            char[] charredCode = new char[6];
            for (byte i = 5; i >= 0; i--)
            {
                charredCode[i] = (char)(code % 10);
                code /= 10;
            }
            return charredCode;
        }
    }
}
