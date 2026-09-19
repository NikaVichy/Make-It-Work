using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace MakeItWork.Services.ModelServices.UserRepository
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        public UserRepository(ApplicationDb applicationDb) : base(applicationDb) { }

        new public async Task<UserModel?> CreateModelAsync(
            UserModel model,
            bool saveImmediatelly = true,
            CancellationToken ct = default)
        {
            bool anyExists = await _GetUserByUsername(model.username).AnyAsync(ct);
            if (anyExists) return null;

            return await base.CreateModelAsync(model, saveImmediatelly, ct);
        }

        private IQueryable<UserModel> _GetUserByUsername(string username)
        {
            return _dbSet
                .Where(u => u.username == username);

        }

        /// <returns>Returns uId or 0 if user is not found.</returns>
        public async Task<int> GetUserIdByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _GetUserByUsername(username)
                .Select(u => u.id)
                .FirstOrDefaultAsync(ct);
        }
    }
}
