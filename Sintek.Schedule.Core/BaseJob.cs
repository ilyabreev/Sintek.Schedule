using System;
using System.Collections.Concurrent;
using System.Threading;
using log4net;
using Quartz;
using Sintek.Schedule.Core.Misc;

namespace Sintek.Schedule.Core
{
    public abstract class BaseJob : IInterruptableJob
    {
        private readonly ManualResetEvent _manualResetEvent;

        protected BaseJob()
        {
            _manualResetEvent = new ManualResetEvent(false);
        }
        
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
                    throw new JobExecutionException(ex, refire);
                }
            }
        }

        /// <summary>
        /// Даёт сигнал о том, что джоб попросили завершиться
        /// </summary>
        public abstract void Interrupt();

        /// <summary>
        /// Блокирует текущий поток джоба.
        /// </summary>
        public void Wait()
        {
            _manualResetEvent.WaitOne();
        }

        public void Release()
        {
            _manualResetEvent.Set();
        }
        
        protected abstract void ExecuteJob(IJobExecutionContext context);

        private static ConcurrentDictionary<string, FailDetails> FailedJobs { get; } = new ConcurrentDictionary<string, FailDetails>();
    }
}