namespace MakeItWork.Services.ModelServices.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
