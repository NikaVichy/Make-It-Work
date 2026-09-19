using MakeItWork.Models;
using MakeItWork.Services.ModelServices.BaseRepository;

namespace MakeItWork.Services.ModelServices.BaseService
{
    public class BaseService<TModel, TRepository>
        where TModel : BaseModel
        where TRepository : IBaseRepository<TModel>
    {
        private protected readonly TRepository _repository;

        public BaseService(TRepository repository)
        {
            _repository = repository;
        }
    }
}
