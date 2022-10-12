using Wallet.Tools.database;
using Wallet.Tools.generic_module;

namespace Wallet.Tools.entity_framework
{
    public interface IEntityFrameworkService<TModelo> where TModelo : class, IGenericModel
    {
        Task AlterarAsync(List<TModelo> listObjetos);
        Task AlterarAsync(TModelo objeto);
        IQueryable<TModelo> AsQueryable();
        void Detach();
        Task ExcluirAsync(List<string> ids);
        Task ExcluirAsync(List<TModelo> objetos);
        Task ExcluirAsync(string id);
        Task ExcluirAsync(TModelo objeto);
        Task IncluirAsync(TModelo objeto);
        Task IncluirAsync(TModelo[] objeto);
        void SetContext(Context context);
    }
}
