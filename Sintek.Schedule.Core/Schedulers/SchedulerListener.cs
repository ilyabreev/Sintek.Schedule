namespace Sintek.Schedule.Core.Schedulers
{
    using System;

    using log4net;

    using Quartz;

    public class SchedulerListener : ISchedulerListener
    {
        public void JobScheduled(ITrigger trigger)
        {
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
        }

        public void TriggerFinalized(ITrigger trigger)
        {
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
        }

        public void TriggersPaused(string triggerGroup)
        {
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
        }

        public void TriggersResumed(string triggerGroup)
        {
        }

        public void JobAdded(IJobDetail jobDetail)
        {
        }

        public void JobDeleted(JobKey jobKey)
        {
        }

        public void JobPaused(JobKey jobKey)
        {
        }

        public void JobsPaused(string jobGroup)
        {
        }

        public void JobResumed(JobKey jobKey)
        {
        }

        public void JobsResumed(string jobGroup)
        {
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"CRITICAL STARTUP ERROR! {msg} Cause: {cause}");
            Console.WriteLine("");
            Console.WriteLine("Read carefully the error shown above to determine root cause of startup failure.");
            Console.WriteLine("Press any key...");
            Console.ReadLine();
        }

        public void SchedulerInStandbyMode()
        {
        }

        public void SchedulerStarted()
        {
        }

        public void SchedulerStarting()
        {
        }

        public void SchedulerShutdown()
        {
        }

        public void SchedulerShuttingdown()
        {
        }

        public void SchedulingDataCleared()
        {
        }
    }
}
