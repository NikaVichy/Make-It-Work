using MakeItWork.Models;

namespace MakeItWork.Services.UserRegistration
{
    public interface IUserRegistrationService
    {
        public Task<UserModel?> RegisterUserAsync(string username, char[] password, CancellationToken ct = default);
    }
}
