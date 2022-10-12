using Microsoft.EntityFrameworkCore;

namespace Wallet.Modules.trade_module
{
    public class TradeRepository : ITradeRepository
    {
        private readonly DbSet<Trade> _context;

        public TradeRepository(DbSet<Trade> context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trade>> List()
        {
            return await _context.ToListAsync();
        }

        public async Task<IEnumerable<Trade>> List(string id)
        {
            return await _context.Where(a => a.Id == id).ToListAsync();
        }
    }
}
