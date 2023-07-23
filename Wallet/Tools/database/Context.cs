using Microsoft.EntityFrameworkCore;
using Wallet.Modules.asset_module;
using Wallet.Modules.position_module;
using Wallet.Modules.trade_module;
using Wallet.Modules.user_module;
using Wallet.Tools.session_control;

namespace Wallet.Tools.database
{
    public class Context : DbContext
    {
        #region Construtor
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        protected Context(DbContextOptions contextOptions)
        : base(contextOptions)
        {
        }
        #endregion

        #region DbSet
        public DbSet<Asset> Asset { get; set; }
        public DbSet<AssetHistoricalData> AssetHistoricalData { get; set; }
        public DbSet<Trade> Trade { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<SessionControl> SessionControl { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }



    }
}
