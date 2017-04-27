using System;
using Quartz;

namespace Sintek.Schedule.Core
{
    public abstract class JobWithOptions<TOptions> : BaseJob where TOptions : class
    {
        protected override void ExecuteJob(IJobExecutionContext context)
        {
            var options = ParseOptions(context.JobDetail.JobDataMap);
            Run(options);
        }

        protected abstract void Run(TOptions options);

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
    }
}
