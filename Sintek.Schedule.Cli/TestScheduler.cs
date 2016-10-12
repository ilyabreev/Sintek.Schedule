using Sintek.Schedule.Core;

namespace Sintek.Schedule.Cli
{
    public class TestScheduler : Scheduler
    {
        public TestScheduler()
        {
            Jobs = new[]
            {
                CreateDailyTriggeredJob<TestJob>(10, 18)
            };
        }

        protected override ScheduledJob[] Jobs { get; }
    }
}
