using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeItWork.Services.ModelServices.BaseRepository
{
    public class BaseRepository<T> : IUnitOfWork, IBaseRepository<T> where T : BaseModel
    {
        private protected readonly ApplicationDb _applicationDb = null!;
        private protected readonly DbSet<T> _dbSet = null!;

        public BaseRepository(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
            _dbSet = _applicationDb.Set<T>();
        }
        public virtual async Task<T> CreateModelAsync(T model, bool saveImmediatelly = true, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(model, ct);

            if (saveImmediatelly)
                await _applicationDb.SaveChangesAsync(ct);

            return model;
        }
        public virtual async Task<T?> UpdateModelAsync(T model, CancellationToken ct = default)
        {
            var dbModel = await _dbSet.FindAsync(model.id, ct);
            if (dbModel == null) return null;

            _applicationDb.Entry(dbModel).CurrentValues.SetValues(model);
            await _applicationDb.SaveChangesAsync(ct);

            return dbModel;
        }
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _applicationDb.SaveChangesAsync(ct);
        }
    }
}
