using Microsoft.EntityFrameworkCore;

namespace Wallet.Modules.asset_module
{
    public class AssetRepository : IAssetRepository
    {
        private readonly DbSet<Asset> _context;

        public AssetRepository(DbSet<Asset> context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asset>> List()
        {
            return await _context.ToListAsync();
        }

        public async Task<IEnumerable<Asset>> List(string id)
        {
            return await _context.Where(a => a.Id == id).ToListAsync();
        }
    }
}
