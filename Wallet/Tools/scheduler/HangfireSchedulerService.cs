using Hangfire;
using Newtonsoft.Json.Linq;
using Wallet.Modules.asset_module;
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

        public HangfireSchedulerService(Context context, IBackgroundJobClient backgroundJobClient, IAssetService assetService)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
            _assetService = assetService;
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
            }
            catch (Exception)
            {

                throw;
            }

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

        public string GetCronExpression(SchedulerDTO schedulerDTO)
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
    }
}
