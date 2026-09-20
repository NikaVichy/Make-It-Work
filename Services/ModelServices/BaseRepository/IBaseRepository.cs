using MakeItWork.Models;

namespace MakeItWork.Services.ModelServices.BaseRepository
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        //Task<List<T>> GetAllAsync(CancellationToken ct = default);
        //Task<T?> GetByIdAsync(int itemId, CancellationToken ct = default);
        Task<T> CreateModelAsync(T item, bool saveImmediatelly = true, CancellationToken ct = default);
        Task<T?> UpdateModelAsync(T item, CancellationToken ct = default);
        //Task DeleteModelAsync(int itemId, CancellationToken ct = default);
    }
}
