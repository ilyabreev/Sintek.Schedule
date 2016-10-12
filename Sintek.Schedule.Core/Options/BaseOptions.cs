using CommandLine;

namespace Sintek.Schedule.Core.Options
{
    public class BaseOptions
    {
        [Option('j', "job", Required = false,
        HelpText = "Job name to start immediately.")]
        public string JobName { get; set; }
    }
}
