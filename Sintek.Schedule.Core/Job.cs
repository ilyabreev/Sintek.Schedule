using System;
using System.Collections.Generic;
using Quartz;

namespace Sintek.Schedule.Core
{
    public abstract class BaseJob : IJob
    {
        public abstract void Execute(IJobExecutionContext context);
    }

    public abstract class Job<TOptions> : BaseJob where TOptions : class
    {
        public override void Execute(IJobExecutionContext context)
        {
            var options = ParseOptions(context.JobDetail.JobDataMap);
            Run(options);
        }

        private TOptions ParseOptions(JobDataMap dataMap)
        {
            if (dataMap.Count == 0)
            {
                return null;
            }

            var options = Activator.CreateInstance<TOptions>();
            var optionsType = typeof(TOptions);
            foreach (var key in dataMap.Keys)
            {
                optionsType.GetProperty(key).SetValue(options, dataMap[key], null);
            }

            return options;
        }

        protected abstract void Run(TOptions options);
    }

    public abstract class Job : BaseJob
    {
        public override void Execute(IJobExecutionContext context)
        {
            Run();
        }

        protected abstract void Run();
    }
}
