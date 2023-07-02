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
            //Os horários também seguiram isso - pode ser '?' ou intervalo definido ou até mesmo específico '*/5 9:30-18 * ? MON-FRI *' nesse caso ele executa
            //a cada 5 minutos desde que dentro do horário de 9h30 até 18h de segunda a sexta.

            //Criar uma tabela com os schedulers ID, descrição, tipo, periodo, ativo
            RecurringJob.AddOrUpdate("w", () => Teste(), $"0 */5 * * * ?");
        }


        public async Task Teste()
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

        public string GetCronExpression(eSchedulerType schedulerType, string value)
        {
            switch (schedulerType)
            {
                case eSchedulerType.Day:
                    return $"0 0 0 */{value} * ?";

                case eSchedulerType.Hour:
                    return $"0 0 */{value} * * ?";

                case eSchedulerType.Minute:
                    return $"0 */{value} * * * ?";

                case eSchedulerType.Second:
                    return $"*/{value} * * * * ?";

                case eSchedulerType.Fixed:
                    int interval;
                    if (int.TryParse(value, out interval))
                    {
                        return $"0 0/{interval} * * * ?";
                    }
                    else
                    {
                        return null;
                    }

                case eSchedulerType.CronExpression:
                    return value;

                default:
                    return null;
            }
        }

    }

}
