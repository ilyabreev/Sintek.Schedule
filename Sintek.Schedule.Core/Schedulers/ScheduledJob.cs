namespace Sintek.Schedule.Core.Schedulers
{
    using Quartz;

    public class ScheduledJob
    {
        public ScheduledJob(IJobDetail jobDetail, ITrigger trigger)
        {
            JobDetail = jobDetail;
            Trigger = trigger;
        }

        public ITrigger Trigger { get; }

        public IJobDetail JobDetail { get; }
    }
}
