using MakeItWork.Services.ModelServices.UserAuthRepository;
using MakeItWork.Services.ModelServices.UserRepository;
using MakeItWork.Services.SecurityServices.PasswordHasher;

namespace MakeItWork.Services.SecurityServices.Authenticator
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly IUserRepository _userRepository = null!;
        private readonly IUserAuthRepository _userAuthRepository = null!;

        public AuthenticatorService(IUserRepository userRepository,
            IUserAuthRepository userAuthRepository,
            IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _userAuthRepository = userAuthRepository;
        }

        public async Task<bool> TryValidateAuthModelAsync(string username, char[] password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || password == null || password.Length < 8)
                return false;

            try
            {
                var userId = await _userRepository.GetUserIdByUsernameAsync(username);
                if (userId == 0)
                {
                    Array.Clear(password);
                    return false;
                }

                bool isAuthenticated = await _userAuthRepository
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
