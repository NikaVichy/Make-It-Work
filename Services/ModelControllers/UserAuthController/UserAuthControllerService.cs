using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MakeItWork.Services.ModelControllers.UserAuthController
{
    public class UserAuthControllerService : IUserAuthController
    {
        private readonly ApplicationDb _applicationDb = null!; 
        private readonly DbSet<UserAuthModel> _dbSet = null!;

        public UserAuthControllerService(ApplicationDb applicationDb) 
        { 
            _applicationDb = applicationDb;
            _dbSet = applicationDb.userAuth; 
        }

        public async Task<UserAuthModel?> TryGetUserAuthModelAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.id == id, ct); 
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
            bool saveImmediately=false, 
            CancellationToken ct = default)
        {
            await _dbSet.AddAsync(userAuthModel, ct);

            if (saveImmediately)
                await _applicationDb.SaveChangesAsync(ct);

            return userAuthModel;
        }
        public async Task<UserAuthModel?> UpdateUserAuthModelAsync(
            UserAuthModel userAuthModel, 
            CancellationToken ct = default)
        {
            var dbUserAuth = await TryGetUserAuthModelAsync(userAuthModel.id, ct);

            if (dbUserAuth == null) return null;

            dbUserAuth.UpdateModel(userAuthModel);
            await _applicationDb.SaveChangesAsync(ct);

            return dbUserAuth;
        }
    }
}
