using Microsoft.EntityFrameworkCore;
using System.Web.Http.ModelBinding;
using Wallet.Modules.asset_module.enums;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.validation_dictionary;

namespace Wallet.Modules.asset_module
{
    public class AssetService : GenericService<Asset>, IAssetService
    {
        #region Variáveis
        private IValidationDictionary _validatonDictionary;
        private Context _context;
        private readonly IAlphaVantageService _alphaVantageService;
        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Construtor
        public AssetService(Context context, IAlphaVantageService alphaVantageService)
        {
            _context = context;
            _alphaVantageService = alphaVantageService;
        }

        #endregion

        public async Task<List<Asset>> Read(string? id = null, string? ticker = null)
        {
            var list = new List<Asset>();


            if (id is null && ticker is null)
            {
                list = await _context.Asset.AsQueryable().ToListAsync();
            }
            else
            {
                list = await _context.Asset.AsQueryable().Where(a => a.Id == id || a.Ticker == ticker).ToListAsync();
            }

            if (list.Count > 0)
            {
                return list;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public async Task<Asset> Creat(Asset asset)
        {
            asset.Ticker = GetFixedTicker(asset);
            if (!await TickerExists(asset.Ticker)) throw new ArgumentException("Ativo não foi encontrado.");
            if (await _context.Asset.AnyAsync(a => a.Ticker == asset.Ticker && a.Class == asset.Class)) throw new ArgumentNullException("Ativo já cadastrado.");
            await InsertOrUpdate(asset);
            return asset;
        }

        private async Task<bool> TickerExists(string ticker)
        {
            var searchList = await _alphaVantageService.SearchTicker(ticker);
            return searchList.Any(item => string.Equals(item.Symbol, ticker, StringComparison.OrdinalIgnoreCase));
        }


        public async Task<Asset> Update(string id, Asset asset)
        {
            var oldAsset = await _context.Asset.AsQueryable().Where(a => a.Id == id || a.Id == asset.Id).FirstOrDefaultAsync();

            if (oldAsset == null) throw new ArgumentNullException("Não existe registro deste ativo.");

            asset.Id = oldAsset.Id;
            asset.Ticker ??= oldAsset.Ticker;
            asset.Description ??= oldAsset.Description;
            asset.Class ??= oldAsset.Class;
            await InsertOrUpdate(asset);
            return asset;
        }

        public async Task<Asset> Delete(string id)
        {
            Asset asset = await _context.Asset.AsQueryable().Where(a => a.Id == id).FirstOrDefaultAsync();
            await Remove(asset);
            return asset;
        }


        private string GetFixedTicker(Asset asset)
        {
            var fixedTicker = asset.Ticker.ToUpper();

            if (asset.Class == eAssetClass.Acao || asset.Class == eAssetClass.Fii || asset.Class == eAssetClass.EtfBrasil || asset.Class == eAssetClass.Bdr)
            {
                fixedTicker = fixedTicker + ".SAO";
            }

            return fixedTicker;
        }

        private async Task InsertOrUpdate(Asset asset)
        {
            if (!modelState.IsValid)
            {
                throw new ArgumentException();
            }

            if (await _context.Asset.AsQueryable().Where(a => a.Id == asset.Id).FirstOrDefaultAsync() is not null)
            {
                _context.Update(asset);
            }
            else
            {
                _context.Add(asset);
            }
            await Save();
        }

        private async Task Remove(Asset asset)
        {
            if (!modelState.IsValid)
            {
                throw new ArgumentException();
            }
            _context.Remove(asset);
            await Save();
        }


        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ReloadStockQuotes()
        {
            var assets = await _context.Asset.AsQueryable().Join(_context.Position, asset => asset.Id, position => position.AssetId, (asset, position) => asset).Distinct().ToListAsync();
            foreach (var asset in assets)
            {
                try
                {
                    var stockQuote = await _alphaVantageService.GetStockQuote(asset.Ticker);
                    asset.Price = stockQuote;
                }
                catch (Exception)
                {

                    continue;
                }

            }

            await SetStockQuote(assets);
        }

        public async Task SetStockQuote(List<Asset> assets)
        {
            await UpdateAsync(assets, _context);
        }
    }
}