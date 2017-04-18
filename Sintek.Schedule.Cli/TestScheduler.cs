using Sintek.Schedule.Core;

namespace Sintek.Schedule.Cli
{
    using System.Reflection;

    using Autofac;

    using Core.Schedulers;

    public class TestScheduler : Scheduler
    {
        public TestScheduler()
        {
            Jobs = new[]
            {
                CreateDailyTriggeredJob<TestJobWithOptions>(10, 18)
            };
        }

        protected override IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new JobsModule(Assembly.GetExecutingAssembly()));
            return builder.Build();
        }

        protected override ScheduledJob[] Jobs { get; }
    }
}
