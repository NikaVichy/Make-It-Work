using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.UserAuthRepository;
using MakeItWork.Services.ModelServices.UserRepository;
using MakeItWork.Services.SecurityServices.PasswordHasher;

namespace MakeItWork.Services.UserRegistration
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ApplicationDb _applicationDb;
        private readonly IUserRepository _userRepository;
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IProjectRepository _projectRepository;

        private readonly IPasswordHasherService _passwordHasher;

        public UserRegistrationService(ApplicationDb applicationDb,
            IUserRepository userRepository,
            IUserAuthRepository userAuthRepository,
            IPasswordHasherService passwordHasher,
            IProjectRepository projectRepository)
        {
            _applicationDb = applicationDb;

            _userRepository = userRepository;
            _userAuthRepository = userAuthRepository;

            _passwordHasher = passwordHasher;

            _projectRepository = projectRepository;
        }

        public async Task<UserModel?> RegisterUserAsync(string username, char[] password, CancellationToken ct = default)
        {
            if (password.Length < 8) return null;

            await using var transaction = await _applicationDb.Database.BeginTransactionAsync();
            try
            {
                var newUser = new UserModel() { username = username };

                var dbUserModel = await _userRepository.CreateModelAsync(newUser, saveImmediately: false, ct);
                if (dbUserModel == null) return null; // TODO: Make feedback to user

                var hashedPassword = await _passwordHasher.HashValidatedPasswordAsync(password);
                Array.Clear(password);

                var newAuthModel = new UserAuthModel() { hashedPassword = hashedPassword, userModel = dbUserModel, userId = dbUserModel.id };
                await _userAuthRepository.CreateUserAuthModelAsync(newAuthModel, saveImmediatelly: false, ct);

                await _applicationDb.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var newProject = new ProjectModel() { ownerId = dbUserModel.id, projectName = dbUserModel.username + "'s project" };
                var dbProjectModel = await _projectRepository.CreateModelAsync(newProject, saveImmediately: false, ct: ct);

                await _applicationDb.SaveChangesAsync();

                return dbUserModel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
