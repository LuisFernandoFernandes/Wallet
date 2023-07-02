namespace Wallet.Tools.scheduler
{
    public class SchedulerDTO
    {
        public eSchedulerType Type { get; set; }
        public string TypeValue { get; set; }
        public eSchedulerWeekDayType WeekDayType { get; set; }
        public string WeekDayTypeValue { get; set; }
        public eSchedulerHourType HourType { get; set; }
        public string HourTypeValue { get; set; }
    }
}
