using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.validation_dictionary;

namespace Wallet.Modules.asset_module
{
    public class AssetHistoricalDataService : GenericService<AssetHistoricalData>, IAssetHistoricalDataService
    {
        #region Vars
        private Context _context;
        #endregion

        #region Construtor
        public AssetHistoricalDataService(Context context)
        {
            _context = context;
        }
        #endregion

        public async Task AddHistoricalDataAsync(List<HistoricalDataDTO> assetHistoricalDataDTOList, string assetId)
        {
            foreach (var assetHistoricalDataDTO in assetHistoricalDataDTOList)
            {
                var assetHistoricalData = new AssetHistoricalData
                {
                    AssetId = assetId,
                    Date = assetHistoricalDataDTO.Date.ToUniversalTime(), // Garantir que a data seja tratada como Utc
                    Open = assetHistoricalDataDTO.Open,
                    High = assetHistoricalDataDTO.High,
                    Low = assetHistoricalDataDTO.Low,
                    Close = assetHistoricalDataDTO.Close,
                    AdjustedClose = assetHistoricalDataDTO.AdjustedClose,
                    Volume = assetHistoricalDataDTO.Volume,
                    DividendAmount = assetHistoricalDataDTO.DividendAmount,
                    SplitCoefficient = assetHistoricalDataDTO.SplitCoefficient
                };

                // Evite duplicação de consultas no banco de dados usando AnyAsync() com AsNoTracking()
                var dataExists = await _context.AssetHistoricalData.AsNoTracking()
                    .AnyAsync(a => a.AssetId == assetHistoricalData.AssetId && a.Date == assetHistoricalData.Date);

                if (!dataExists)
                {
                    await InsertAsync(assetHistoricalData, _context);
                }
            }
        }

    }
}
