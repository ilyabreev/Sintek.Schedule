using System;
using System.Collections.Concurrent;
using System.Threading;
using log4net;
using Quartz;
using Sintek.Schedule.Core.Misc;

namespace Sintek.Schedule.Core
{
    public abstract class BaseJob : IJob
    {
        private static ConcurrentDictionary<string, FailDetails> FailedJobs { get; } = new ConcurrentDictionary<string, FailDetails>();

        /// <summary>
        /// Запускает выполнение джоба.
        /// </summary>
        /// <param name="context">Объект контекста джоба <see cref="IJobExecutionContext"/>.</param>
        public void Execute(IJobExecutionContext context)
        {
            using (LogicalThreadContext.Stacks["NDC"].Push(Guid.NewGuid().ToString()))
            {
                try
                {
                    ExecuteJob(context);
                }
                catch (Exception ex)
                {
                    var jobName = context.JobDetail.Key.ToString();
                    var fail = FailedJobs.GetOrAdd(jobName, new FailDetails());
                    var refire = RefireConditionsChecker.CheckConditions(context.JobInstance.GetType(), fail);
                    ++fail;
                    throw new JobExecutionException($"Critical exception in {jobName}. Job will {(refire ? "" : "NOT ")}be restarted", ex, refire);
                }
            }
        }

        /// <summary>
        /// Блокирует текущий поток джоба.
        /// </summary>
        public void Wait()
        {
            new ManualResetEvent(false).WaitOne();
        }

        protected abstract void ExecuteJob(IJobExecutionContext context);
    }
}