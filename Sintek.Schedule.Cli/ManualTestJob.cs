namespace Sintek.Schedule.Cli
{
    using System;
    using System.Threading;

    using Core;

    public class ManualTestJob : Job
    {
        protected override void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("I am still here...");
                Thread.Sleep(1000);
            }
        }
    }
}
