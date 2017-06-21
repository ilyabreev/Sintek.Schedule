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
        /// <summary>
        /// Был ли отменен джоб
        /// </summary>
        protected bool IsCancelled { get; set; }

        /// <summary>
        /// Был ли джоб запущен вручную
        /// </summary>
        protected bool IsManual { get; private set; }

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
                IsManual = Convert.ToBoolean(context.JobDetail.JobDataMap["manual"]);
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
        /// Вызывается, когда джоб, запущенный по расписанию, требуется прервать
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Вызывается планировщиком, когда планировщик останавливается
        /// </summary>
        public void Interrupt()
        {
            if (!IsManual)
            {
                IsCancelled = true;
                Cancelled?.Invoke(this, EventArgs.Empty);
            }
        }

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