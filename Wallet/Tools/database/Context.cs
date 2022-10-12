using Microsoft.EntityFrameworkCore;
using Wallet.Modules.asset_module;

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
        #endregion
    }
}
