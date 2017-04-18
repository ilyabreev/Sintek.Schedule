using System;
using System.Threading;
using log4net;
using Quartz;

namespace Sintek.Schedule.Core
{
    public abstract class BaseJob : IJob
    {
        /// <summary>
        /// Запускает выполнение джоба.
        /// </summary>
        /// <param name="context">Объект контекста джоба <see cref="IJobExecutionContext"/>.</param>
        public void Execute(IJobExecutionContext context)
        {
            using (LogicalThreadContext.Stacks["NDC"].Push(Guid.NewGuid().ToString()))
            {
                ExecuteJob(context);
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