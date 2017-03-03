[![build status](http://gitlab.local/sintek/schedule/badges/master/build.svg)](http://sintek.pages.local/schedule/TestResult.html)
[![NuGet](https://img.shields.io/nuget/v/Sintek.Schedule.svg)](https://www.nuget.org/packages/Sintek.Schedule/)
# Sintek.Schedule
Compact Quartz.net based library for quick integration of task scheduling into you app

## How to use?

### Create a job class
```
public class TestJob : Job
{
    protected override void Run()
    {
    }
}
```

If you need to pass command line args to the job then create job class and options class.

```
public class TestJob : Job<TestOptions>
{
    protected override void Run(TestOptions options)
    {
        // use your options here
    }
}

public class TestOptions
{
    [Option('a', "arg", Required = false)]
    public string TestArg
    {
        get; set;
    }
}
```

### Create a scheduler class
```
public class TestScheduler : Scheduler
{
    public TestScheduler()
    {
        Jobs = new[]
        {
            CreateDailyTriggeredJob<TestJob>(10, 18)  // TestJob will run at 10:18.
        };
    }

    protected override ScheduledJob[] Jobs { get; }
}
```

### Use your scheduler!
```
public static int Main(string[] args)
{
    var scheduler = new TestScheduler();
    scheduler.Start(args);
    return 0;
}
```
Each your job can be run manually by passing `-j` command line arg with name of the job.

## How to install?

### NuGet

`Install-Package Sintek.Schedule`
