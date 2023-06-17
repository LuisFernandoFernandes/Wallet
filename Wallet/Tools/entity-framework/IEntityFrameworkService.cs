using Wallet.Tools.database;
using Wallet.Tools.generic_module;

namespace Wallet.Tools.entity_framework
{
    public interface IEntityFrameworkService<T> where T : class, IGenericModel
    {
        Task UpdateAsync(List<T> obj, Context context);
        Task UpdateAsync(T obj, Context context);
        IQueryable<T> AsQueryable();
        void Detach();
        Task DeleteAsync(List<string> ids, Context context);
        Task DeleteAsync(List<T> objs, Context context);
        Task DeleteAsync(string id, Context context);
        Task DeleteAsync(T obj, Context context);
        Task InsertAsync(T obj, Context context);
        Task InsertAsync(T[] obj, Context context);
        void SetContext(Context context);
    }
}
