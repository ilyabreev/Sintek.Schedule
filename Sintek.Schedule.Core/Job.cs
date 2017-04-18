using Quartz;

namespace Sintek.Schedule.Core
{
    public abstract class Job : BaseJob
    {
        protected override void ExecuteJob(IJobExecutionContext context)
        {
            Run();
        }

        protected abstract void Run();
    }
}