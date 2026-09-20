using MakeItWork.ApplicationDbContext;

namespace MakeItWork.Services.ModelServices.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDb _applicationDb;

        public EfUnitOfWork(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _applicationDb.SaveChangesAsync(ct);
        }
    }
}
