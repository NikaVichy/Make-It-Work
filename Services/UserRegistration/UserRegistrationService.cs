using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using MakeItWork.Services.ModelControllers.ProjectController;
using MakeItWork.Services.ModelControllers.UserAuthController;
using MakeItWork.Services.ModelControllers.UserController;
using MakeItWork.Services.SecurityServices.PasswordHasher;
using System.Security.Cryptography;

namespace MakeItWork.Services.UserRegistration
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ApplicationDb _applicationDb;
        private readonly IUserController _userController;
        private readonly IUserAuthController _userAuthController;
        private readonly IProjectController _projectController;

        private readonly IPasswordHasherService _passwordHasher;

        public UserRegistrationService(ApplicationDb applicationDb,
            IUserController userController,
            IUserAuthController userAuthController,
            IPasswordHasherService passwordHasher,
            IProjectController projectController)
        {
            _applicationDb = applicationDb;

            _userController = userController;
            _userAuthController = userAuthController;

            _passwordHasher = passwordHasher;

            _projectController = projectController;
        }

        public async Task<UserModel?> RegisterUserAsync(string username, char[] password, CancellationToken ct = default)
        {
            if(password.Length < 8) return null;

            await using var transaction = await _applicationDb.Database.BeginTransactionAsync();
            try
            {
                var newUser = new UserModel() { username = username };

                var dbUserModel = await _userController.CreateModelAsync(newUser, saveImmediately: false, ct);
                if (dbUserModel == null) return null; // TODO: Make feedback to user

                var hashedPassword = await _passwordHasher.HashValidatedPasswordAsync(password);
                Array.Clear(password);

                var newAuthModel = new UserAuthModel() { hashedPassword = hashedPassword, userModel = dbUserModel, userId = dbUserModel.id };
                await _userAuthController.CreateUserAuthModelAsync(newAuthModel, saveImmediatelly: false, ct);

                await _applicationDb.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var newProject = new ProjectModel() { ownerId = dbUserModel.id, projectName = dbUserModel.username + "'s project" };
                var dbProjectModel = await _projectController.CreateModelAsync(newProject, saveImmediately: false, ct: ct);

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
