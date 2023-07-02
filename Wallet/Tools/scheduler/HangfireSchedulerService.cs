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
        private readonly IAlphaVantageService _alphaVantageService;

        public HangfireSchedulerService(Context context, IBackgroundJobClient backgroundJobClient, IAlphaVantageService alphaVantageService)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
            _alphaVantageService = alphaVantageService;
        }

        public async Task ScheduleJobs()
        {
            //Os schedulers todos tem dias da semana - pode ser '?' se forem todos, ou um dia específico 'MON' ou intervalo 'MON-FRI'
            //Os horários também seguiram isso - pode ser '?' ou intervalo definido ou até mesmo específico '0 */5 9-18 * ? MON-FRI *' nesse caso ele executa
            //a cada 5 minutos desde que dentro do horário de 9h até 18h de segunda a sexta.
            //Horários quebrados são mais complexos, mas vou tentar criar algo para calcular

            //Criar uma tabela com os schedulers ID, descrição, tipo, periodo, ativo?  $"0 */5 * * * ?"

            var reloadQuotes = new SchedulerDTO() { Type = eSchedulerType.Minute, TypeValue = "5", WeekDayType = eSchedulerWeekDayType.Interval, WeekDayTypeValue = "MON-FRI", HourType = eSchedulerHourType.Interval, HourTypeValue = "8-18" };
            var reloadQuotes2 = new SchedulerDTO() { Type = eSchedulerType.Minute, TypeValue = "2", WeekDayType = eSchedulerWeekDayType.Interval, WeekDayTypeValue = "MON-SUN", HourType = eSchedulerHourType.Interval, HourTypeValue = "8-22" };
            try
            {
                RecurringJob.AddOrUpdate("ER", () => ReloadQuotesScheduler(), GetCronExpression(reloadQuotes2));
                RecurringJob.AddOrUpdate("JJJ", () => Teste2(), GetCronExpression(reloadQuotes2));
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
                await _alphaVantageService.ReloadStockQuotes();
                await Task.CompletedTask;
            }
            catch (Exception)
            {

                await Task.CompletedTask;
            }

        }

        public async Task Teste2()
        {
            await Task.CompletedTask;
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


        //public string GetCronExpression2(eSchedulerType schedulerType, string value)
        //{
        //    switch (schedulerType)
        //    {
        //        case eSchedulerType.Day:
        //            return $"0 0 0 */{value} * *";

        //        case eSchedulerType.Hour:
        //            return $"0 0 */{value} * * *";

        //        case eSchedulerType.Minute:
        //            return $"0 */{value} * * * *";

        //        case eSchedulerType.Second:
        //            return $"*/{value} * * * * *";

        //        case eSchedulerType.Fixed:
        //            int interval;
        //            if (int.TryParse(value, out interval))
        //            {
        //                return $"0 0/{interval} * * * ?";
        //            }
        //            else
        //            {
        //                return null;
        //            }

        //        case eSchedulerType.CronExpression:
        //            return value;

        //        default:
        //            return null;
        //    }
        //}

    }

}
