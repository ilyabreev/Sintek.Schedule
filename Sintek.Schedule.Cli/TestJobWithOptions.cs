using System;
using Sintek.Schedule.Core;

namespace Sintek.Schedule.Cli
{
    using System.Threading;

    public class TestJobWithOptions : JobWithOptions<TestOptions>
    {
        protected override void Run(TestOptions options)
        {
            Console.WriteLine(options == null ? $"Scheduled start." : $"Manual start. Arg value: {options.TestArg}");
            while (!Interrupted)
            {
                Thread.Sleep(1000);
                Console.WriteLine("I am still here");
            }

            Console.WriteLine("Shutdown requested!");
        }

        public bool Interrupted { get; set; }

        public override void Interrupt()
        {
            Interrupted = true;
        }
    }
}
