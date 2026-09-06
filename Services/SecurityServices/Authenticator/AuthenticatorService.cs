using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.UserAuthController;
using MakeItWork.Services.ModelControllers.UserController;
using MakeItWork.Services.SecurityServices.PasswordHasher;

namespace MakeItWork.Services.SecurityServices.Authenticator
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly IUserController _userController = null!;
        private readonly IUserAuthController _userAuthController = null!;
        private readonly IPasswordHasherService _passwordHasherService = null!;

        public AuthenticatorService(IUserController userController, 
            IUserAuthController userAuthController, 
            IPasswordHasherService passwordHasherService)
        {
            _userController = userController;
            _userAuthController = userAuthController;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<UserAuthModel?> TryValidateAuthModelAsync(string username, char[] password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || password == null || password.Length < 8)
                return null;

            try
            {
                var dbUserModel = await _userController.GetUserModelByUsernameAsync(username, ct);
                if (dbUserModel == null)
                {
                    Array.Clear(password);
                    return null;
                }

                var dbUserAuthModel = await _userAuthController.TryGetUserAuthModelAsync(dbUserModel.id, ct);
                if (dbUserAuthModel == null)
                {
                    Array.Clear(password);
                    return null;
                }

                if (dbUserAuthModel.IsLocked())
                {
                    Array.Clear(password);
                    return null;
                }

                var isAuthorized = await _passwordHasherService.VerifyPasswordAsync(password: password, encodedHash: dbUserAuthModel.hashedPassword);
                Array.Clear(password);

                if (!isAuthorized)
                {
                    dbUserAuthModel.AddAuthAttempt();
                    await _userAuthController.UpdateUserAuthModelAsync(dbUserAuthModel, ct);

                    return null;
                }

                if(dbUserAuthModel.TryClearAuthAttempts())
                    await _userAuthController.UpdateUserAuthModelAsync(dbUserAuthModel, ct);

                return dbUserAuthModel;
            }
            catch
            {
                Array.Clear(password);
                return null;
            }
        }
    }
}
