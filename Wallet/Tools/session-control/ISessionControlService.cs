using Wallet.Modules.user_module;
using Wallet.Tools.generic_module;

namespace Wallet.Tools.session_control
{
    public interface ISessionControlService : IGenericService<SessionControl>
    {
        Task<SessionControl> RegisterLogin(User user);
    }
}
