namespace Wallet.Tools.scheduler
{
    public enum eSchedulerType
    {
        None,
        Second,
        Minute,
        Hour,
        Day,
        Fixed,
        //CronExpression
    }

    public enum eSchedulerWeekDayType
    {
        None,
        All,
        Fixed,
        Interval
    }

    public enum eSchedulerHourType
    {
        All,
        Fixed,
        Interval
    }
}
