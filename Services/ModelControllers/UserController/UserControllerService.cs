using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using Microsoft.EntityFrameworkCore;
using MakeItWork.Services.ModelControllers.BaseModelController;

namespace MakeItWork.Services.ModelControllers.UserController
{
    public class UserControllerService : BaseModelController<UserModel>, IUserController
    {
        public UserControllerService(ApplicationDb applicationDb) : base(applicationDb) { }

        new public async Task<UserModel?> CreateModelAsync(
            UserModel model, 
            bool saveImmediatelly=true, 
            CancellationToken ct = default)
        {
            var dbUserModel = await GetUserModelByUsernameAsync(model.username, ct);
            if (dbUserModel != null) return null;

            return await base.CreateModelAsync(model, saveImmediatelly, ct);
        }

        public async Task<UserModel?> GetUserModelByUsernameAsync(string? username, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return await _dbSet.FirstOrDefaultAsync(u => u.username == username, ct); // TODO: replace with .where().select();
        }
    }
}
