using CommandLine;

namespace Sintek.Schedule.Cli
{
    public class TestOptions
    {
        [Option('a', "arg", Required = false)]
        public string TestArg
        {
            get; set;
        }
    }
}