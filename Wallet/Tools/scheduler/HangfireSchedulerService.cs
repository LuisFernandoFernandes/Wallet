using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wallet.Modules.asset_module;
using Wallet.Modules.asset_module.enums;
using Wallet.Modules.user_module;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;

namespace Wallet.Tools.scheduler
{
    public class HangfireSchedulerService : IHangfireSchedulerService
    {
        private readonly Context _context;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IAssetService _assetService;
        private readonly IUserService _userService;
        private readonly IAlphaVantageService _alphaVantageService;

        public HangfireSchedulerService(Context context, IBackgroundJobClient backgroundJobClient, IAssetService assetService, IUserService userService, IAlphaVantageService alphaVantageService)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
            _assetService = assetService;
            _userService = userService;
            _alphaVantageService = alphaVantageService;
        }

        public async Task ScheduleJobs()
        {
            //Os schedulers todos tem dias da semana - pode ser '?' se forem todos, ou um dia específico 'MON' ou intervalo 'MON-FRI'
            //Os horários também seguiram isso - pode ser '?' ou intervalo definido ou até mesmo específico '0 */5 9-18 * ? MON-FRI *' nesse caso ele executa
            //a cada 5 minutos desde que dentro do horário de 9h até 18h de segunda a sexta.
            //Horários quebrados são mais complexos, mas vou tentar criar algo para calcular

            //Criar uma tabela com os schedulers ID, descrição, tipo, periodo, ativo?  $"0 */5 * * * ?"

            //var reloadQuotes = new SchedulerDTO() { Type = eSchedulerType.Minute, TypeValue = "5", WeekDayType = eSchedulerWeekDayType.Interval, WeekDayTypeValue = "MON-FRI", HourType = eSchedulerHourType.Interval, HourTypeValue = "8-18" };

            var reloadQuotes = new SchedulerDTO() { Type = eSchedulerType.Minute, TypeValue = "2", WeekDayType = eSchedulerWeekDayType.Interval, WeekDayTypeValue = "MON-SUN", HourType = eSchedulerHourType.Interval, HourTypeValue = "8-22" };


            try
            {
                var count = 0;
                RecurringJob.AddOrUpdate(count++.ToString(), () => ReloadQuotesScheduler(), GetCronExpression(reloadQuotes));

                BackgroundJob.Enqueue(() => UserSeedData());
                BackgroundJob.Enqueue(() => USAssetSeedData());
                BackgroundJob.Enqueue(() => MyAssetSeedData());
            }
            catch (Exception)
            {

                throw;
            }

        }

        private string GetCronExpression(SchedulerDTO schedulerDTO)
        {
            var cronExpression = String.Empty;

            var isInterval = schedulerDTO.HourType == eSchedulerHourType.Interval ? true : false;

            switch (schedulerDTO.Type)
            {

                case eSchedulerType.None:
                    return cronExpression;
                case eSchedulerType.Second:
                    cronExpression = isInterval ? $"*/{schedulerDTO.TypeValue} * {schedulerDTO.HourTypeValue} * * *" : $"*/{schedulerDTO.TypeValue} * * * * *";
                    break;
                case eSchedulerType.Minute:
                    cronExpression = isInterval ? $"0 */{schedulerDTO.TypeValue} {schedulerDTO.HourTypeValue} * * *" : $"0 */{schedulerDTO.TypeValue} * * * *";
                    break;
                case eSchedulerType.Hour:
                    cronExpression = $"0 0 */{schedulerDTO.TypeValue} * * *";
                    break;
                case eSchedulerType.Day:
                    cronExpression = $"0 0 0 */{schedulerDTO.TypeValue} * *";
                    break;
                case eSchedulerType.Fixed:
                    cronExpression = $"0 {schedulerDTO.TypeValue} * * *";
                    break;
            }

            switch (schedulerDTO.WeekDayType)
            {
                case eSchedulerWeekDayType.All:
                    break;
                case eSchedulerWeekDayType.Fixed:
                case eSchedulerWeekDayType.Interval:
                    cronExpression = cronExpression.Substring(0, cronExpression.LastIndexOf('*')) + schedulerDTO.WeekDayTypeValue;
                    break;
            }

            return cronExpression;
        }


        public async Task ReloadQuotesScheduler()
        {
            try
            {
                await _assetService.ReloadStockQuotes();
                await Task.CompletedTask;
            }
            catch (Exception)
            {

                await Task.CompletedTask;
            }
        }

        public async Task UserSeedData()
        {
            var users = new List<UserDTO>()
            {
                new UserDTO
                {
                    UserName = "string",
                    Password = "string",
                    Name = "Admin",
                    Email = "admim@teste.com",
                    CPF = "11111111111",
                    Role = eRole.Admin
                },
                // Adicione outros usuários à lista
            };
            foreach (var user in users)
            {
                if (await _context.User.AsQueryable().AnyAsync(a => a.UserName == user.UserName || a.Email == user.Email || a.CPF == user.CPF)) continue;
                await _userService.Create(user);
            }
        }

        public async Task USAssetSeedData()
        {
            try
            {
                var allUSAssetsDto = await _alphaVantageService.GetAllUSAssets();
                var tickerList = allUSAssetsDto.Select(a => a.symbol).ToList();

                var existingTickers = await _context.Asset
                    .Where(a => tickerList.Contains(a.Ticker))
                    .Select(a => a.Ticker)
                    .ToListAsync();

                var newTickers = tickerList.Except(existingTickers);
                var newAssetDtoList = allUSAssetsDto.Where(a => newTickers.Contains(a.symbol)).ToList();

                var assetList = new List<Asset>();

                foreach (var newAsset in newAssetDtoList)
                {
                    var asset = new Asset
                    {
                        Ticker = newAsset.symbol,
                        Class = newAsset.assetType == "Stock" ? eAssetClass.Stock : eAssetClass.EtfExterior,
                        Description = newAsset.name,
                    };

                    await _assetService.InsertAsync(asset, _context);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task MyAssetSeedData()
        {
            string apiEndpoint = "http://localhost:5000/tickers";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);
                    response.EnsureSuccessStatusCode(); // Verifica se a resposta foi bem-sucedida (status code 2xx)

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserializar o JSON para uma lista de GetMyAssetsDTO
                    var myAssets = JsonConvert.DeserializeObject<GetMyAssetsDTO[]>(responseBody);

                    // Exibir os dados
                    foreach (var myAsset in myAssets)
                    {

                        if (await _context.Asset.AnyAsync(a => a.Ticker == myAsset.AV)) continue;

                        var asset = new Asset();

                        switch (myAsset.LISTA.Trim().ToUpperInvariant())
                        {
                            case "AÇÃO":
                                asset.Class = eAssetClass.Acao; break;
                            case "ETF":
                                asset.Class = eAssetClass.EtfBrasil; break;
                            case "FII":
                                asset.Class = eAssetClass.Fii; break;
                            case "BDR":
                                asset.Class = eAssetClass.Bdr; break;
                            case "STOCKS":
                                asset.Class = eAssetClass.Stock; break;
                            case "ETFUS":
                                asset.Class = eAssetClass.EtfExterior; break;
                            case "REIT":
                                asset.Class = eAssetClass.Reits; break;
                            case "ADR":
                                asset.Class = eAssetClass.ADR; break;
                            default:
                                continue;
                        }

                        asset.Ticker = myAsset.AV;
                        asset.Description = myAsset.EMPRESA;

                        await _assetService.InsertAsync(asset, _context);
                    }
                }
                catch (HttpRequestException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}
