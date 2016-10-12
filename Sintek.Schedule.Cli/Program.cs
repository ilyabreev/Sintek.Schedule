namespace Sintek.Schedule.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var scheduler = new TestScheduler();
            scheduler.Start(args);
            return 0;
        }
    }
}
