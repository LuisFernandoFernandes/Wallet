using Wallet.Modules.asset_module;
using Wallet.Tools.database;

namespace Wallet.Tools.scheduler
{
    public interface IHangfireSchedulerService
    {
        Task ScheduleJobs();
    }
}
