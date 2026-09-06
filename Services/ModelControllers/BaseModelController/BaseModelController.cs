using MakeItWork.ApplicationDbContext;
using MakeItWork.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeItWork.Services.ModelControllers.BaseModelController
{
    public class BaseModelController<T> : IBaseModelController<T> where T : BaseModel
    {
        private protected readonly ApplicationDb _applicationDb = null!;
        private protected readonly DbSet<T> _dbSet = null!;

        public BaseModelController(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
            _dbSet = _applicationDb.Set<T>();
        }
        //public virtual async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        //{
        //    return await _dbSet.ToListAsync(ct);
        //}
        //public virtual async Task<T?> GetByIdAsync(int itemId, CancellationToken ct = default)
        //{
        //    return await _dbSet.FirstOrDefaultAsync(T => T.id == itemId, ct);
        //}
        public virtual async Task<T?> CreateModelAsync(T model, bool saveImmediately = true, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(model, ct);
                
            if (saveImmediately)
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
        //public virtual async Task DeleteModelAsync(int itemId, CancellationToken ct = default)
        //{
        //    await _dbSet
        //        .Where(m => m.id == itemId)
        //        .ExecuteDeleteAsync(ct);
        //}
    }
}
