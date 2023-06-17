using Wallet.Tools.entity_framework;

namespace Wallet.Tools.generic_module
{
    public interface IGenericService<T> : IEntityFrameworkService<T>, IDisposable where T : class, IGenericModel
    { }

}