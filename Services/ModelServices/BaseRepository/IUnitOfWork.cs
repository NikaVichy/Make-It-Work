namespace MakeItWork.Services.ModelServices.BaseRepository
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
