using Wallet.Tools.generic_module;

namespace Wallet.Modules.position_module
{
    public interface IPositionService : IGenericService<Position>
    {
        Task<List<Position>> Read(string? id);
    }
}