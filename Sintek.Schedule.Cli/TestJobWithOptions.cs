using System;
using Sintek.Schedule.Core;

namespace Sintek.Schedule.Cli
{
    public class TestJobWithOptions : JobWithOptions<TestOptions>
    {
        protected override void Run(TestOptions options)
        {
            Console.WriteLine(options == null ? $"Scheduled start." : $"Manual start. Arg value: {options.TestArg}");
        }
    }
}
