using MakeItWork.Services.ModelControllers.UserAuthController;
using MakeItWork.Services.ModelControllers.UserController;
using MakeItWork.Services.SecurityServices.PasswordHasher;

namespace MakeItWork.Services.SecurityServices.Authenticator
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly IUserController _userController = null!;
        private readonly IUserAuthController _userAuthController = null!;

        public AuthenticatorService(IUserController userController,
            IUserAuthController userAuthController,
            IPasswordHasherService passwordHasherService)
        {
            _userController = userController;
            _userAuthController = userAuthController;
        }

        public async Task<bool> TryValidateAuthModelAsync(string username, char[] password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || password == null || password.Length < 8)
                return false;

            try
            {
                var userId = await _userController.GetUserIdByUsernameAsync(username);
                if (userId == 0)
                {
                    Array.Clear(password);
                    return false;
                }

                bool isAuthenticated = await _userAuthController
                    .IsUserAuthenticatedAsync(userId, password, ct);

                return isAuthenticated;
            }
            catch
            {
                Array.Clear(password);
                return false;
            }
        }
    }
}
