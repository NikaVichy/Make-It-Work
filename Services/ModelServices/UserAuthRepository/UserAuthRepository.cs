using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using MakeItWork.Services.SecurityServices.PasswordHasher;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MakeItWork.Services.ModelServices.UserAuthRepository
{
    public sealed class UserAuthRepository : IUserAuthRepository
    {
        private readonly ApplicationDb _applicationDb = null!;
        private readonly DbSet<UserAuthModel> _dbSet = null!;
        private readonly IPasswordHasherService _passwordHasherService = null!;

        public UserAuthRepository(ApplicationDb applicationDb,
            IPasswordHasherService passwordHasherService)
        {
            _applicationDb = applicationDb;
            _dbSet = applicationDb.userAuth;

            _passwordHasherService = passwordHasherService;
        }
        public async Task<bool> TryValidateUserStampAsync(int id, string userStamp, CancellationToken ct = default)
        {
            var secStampObj = await _dbSet
                .Where(u => u.id == id)
                .Select(u => new { u.SecurityStamp })
                .FirstOrDefaultAsync(ct);

            if (secStampObj == null) return false;
            return CryptographicOperations
                .FixedTimeEquals(
                    Encoding.UTF8.GetBytes(userStamp),
                    Encoding.UTF8.GetBytes(secStampObj.SecurityStamp));
        }
        public async Task<bool> CheckIfAnyPresentAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(u => u.id == id, ct);
        }
        public async Task<UserAuthModel?> CreateUserAuthModelAsync(
            UserAuthModel userAuthModel,
            bool saveImmediately = false,
            CancellationToken ct = default)
        {
            await _dbSet.AddAsync(userAuthModel, ct);

            if (saveImmediately)
                await _applicationDb.SaveChangesAsync(ct);

            return userAuthModel;
        }

        public async Task<bool> IsUserAuthenticatedAsync(
            int userId,
            char[] password,
            CancellationToken ct = default)
        {
            try
            {
                var dbUserAuthModel = await _dbSet
                    .Where(u => u.id == userId)
                    .FirstOrDefaultAsync(ct);

                if (dbUserAuthModel == null)
                {
                    Array.Clear(password);
                    return false;
                }

                if (IsLocked(dbUserAuthModel))
                {
                    Array.Clear(password);
                    return false;
                }

                var isAuthenticated = await _passwordHasherService.VerifyPasswordAsync(
                    password,
                    dbUserAuthModel.hashedPassword);
                Array.Clear(password);

                if (isAuthenticated)
                {
                    if (dbUserAuthModel.authAttempts != 0) await ClearAuthAttempts(userId, ct);
                    return true;
                }

                await AddLastAuthAttemt(
                    userId,
                    ++dbUserAuthModel.authAttempts,
                    DateTime.UtcNow,
                    ct);
                return false;

            }
            catch
            {
                Array.Clear(password);
                return false;
            }
        }

        public async Task<string> TryGetSecurityStampAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(u => u.id == userId)
                .Select(m => m.SecurityStamp)
                .FirstOrDefaultAsync(ct) ?? "";
        }
        private static bool IsLocked(UserAuthModel authModel)
        {
            if (authModel.authAttempts < 3) return false;

            int addedHours = authModel.authAttempts / 3;
            if (DateTime.Compare(authModel.lockedSince.AddHours(addedHours), DateTime.UtcNow) > 0)
                return true;

            return false;
        }

        private async Task ClearAuthAttempts(
            int userId,
            CancellationToken ct = default)
        {
            await _dbSet
                .Where(u => u.id == userId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(u => u.authAttempts, 0));
        }

        private async Task AddLastAuthAttemt(
            int userId,
            int authAttempt,
            DateTime lockedSince,
            CancellationToken ct = default)
        {
            await _dbSet
                .Where(u => u.id == userId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(u => u.authAttempts, authAttempt)
                    .SetProperty(u => u.lockedSince, lockedSince));

            // TODO: Rework database so it won't be confusing
            // Should be lastLoggedIn not lockedSince
        }
    }
}
