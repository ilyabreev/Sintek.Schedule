﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandLine;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using Sintek.Schedule.Core.Options;

namespace Sintek.Schedule.Core
{
    /// <summary>
    /// Task scheduler abstract class
    /// </summary>
    public abstract class Scheduler
    {
        private readonly BaseOptions _baseOptions;

        private readonly Parser _parser;

        protected Scheduler()
        {
            _baseOptions = new BaseOptions();
            _parser = new Parser(s =>
            {
                s.IgnoreUnknownArguments = true;
            });
        }
        
        public delegate void WrongBaseArguments(string[] args);

        public delegate void ManualJobStart(string jobName);

        public delegate void JobArgumentsParsing(Type argsType);

        public delegate void WrongJobArguments(Type argsType, string[] args);

        public delegate void StartError(Exception e);

        /// <summary>
        /// Raised when wrong command line arguments are passed. Only arguments from <see cref="BaseOptions">BaseOptions</see> are affected. 
        /// </summary>
        public event WrongBaseArguments OnWrongBaseArguments = delegate(string[] args) { };

        /// <summary>
        /// Raised before single job starting with "-j" switch.
        /// </summary>
        public event ManualJobStart OnManualJobStart = delegate(string jobName) { };

        /// <summary>
        /// Raised before specific job arguments are parsed from command line arguments
        /// </summary>
        public event JobArgumentsParsing OnJobArgumentsParsing = delegate(Type argsType) { };

        /// <summary>
        /// Raised when wrong command line arguments are passed. Only job-specific arguments are affected.
        /// </summary>
        public event WrongJobArguments OnWrongJobArguments = delegate(Type type, string[] args) { };

        /// <summary>
        /// Raised when exception throwed in <see cref="Start">Start</see> method.
        /// </summary>
        public event StartError OnStartError = delegate(Exception exception) { };

        /// <summary>
        /// Start task scheduler with command line arguments.
        /// </summary>
        /// <param name="args">Command line args</param>
        public void Start(string[] args)
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            try
            {
                if (!_parser.ParseArguments(args, _baseOptions))
                {
                    OnWrongBaseArguments(args);
                    return;
                }

                if (!String.IsNullOrWhiteSpace(_baseOptions.JobName))
                {
                    OnManualJobStart(_baseOptions.JobName);
                    object customOptions = null;
                    var baseJobType = FindBaseJobType(Assembly.GetExecutingAssembly()) ??
                                      FindBaseJobType(Assembly.GetCallingAssembly());
                    if (baseJobType != null && baseJobType.IsGenericType)
                    {
                        var customOptionsType = baseJobType.GenericTypeArguments.FirstOrDefault();
                        if (customOptionsType != null)
                        {
                            OnJobArgumentsParsing(customOptionsType);
                            customOptions = Activator.CreateInstance(customOptionsType);
                            if (!_parser.ParseArguments(args, customOptions))
                            {
                                OnWrongJobArguments(customOptionsType, args);
                                return;
                            }
                        }
                    }

                    ScheduleImmediateJob(scheduler, _baseOptions.JobName, customOptions);
                }
                else
                {
                    ScheduleRegularJobs(scheduler);
                }

                scheduler.Start();
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                OnStartError(e);
                if (!String.IsNullOrWhiteSpace(_baseOptions.JobName))
                {
                    scheduler.Shutdown();
                }
            }

            if (!String.IsNullOrWhiteSpace(_baseOptions.JobName))
            {
                scheduler.Shutdown(true);
            }
        }

        /// <summary>
        /// Set of scheduled items for the current scheduler. Each ScheduleItem has its own job and trigger.
        /// </summary>
        protected abstract ScheduledJob[] Jobs { get; }

        /// <summary>
        /// Creates job with trigger that runs right now.
        /// </summary>
        protected ScheduledJob CreateNowTriggeredJob<T>() where T : IJob
        {
            return CreateNowTriggeredJob(typeof(T));
        }

        /// <summary>
        /// Creates job with trigger that runs right now.
        /// </summary>
        /// <param name="jobType">Type of the job</param>
        protected ScheduledJob CreateNowTriggeredJob(Type jobType)
        {
            var jobName = GetJobName(jobType);
            var job = CreateJob(jobType);
            var trigger = CreateNowTrigger(jobName);
            return new ScheduledJob(job, trigger);
        }

        private ITrigger CreateNowTrigger(string jobName)
        {
            return TriggerBuilder.Create()
                .ForJob(jobName)
                .StartNow()
                .Build();
        }

        /// <summary>
        /// Creates job with trigger that runs on daily basis on fixed time of day
        /// </summary>
        /// <typeparam name="T">Job type</typeparam>
        /// <param name="hours">Hours in (from 0 to 23)</param>
        /// <param name="minutes">Minutes (from 0 to 59)</param>
        protected ScheduledJob CreateDailyTriggeredJob<T>(int hours, int minutes) where T : IJob
        {
            return CreateDailyTriggeredJob(typeof(T), hours, minutes);
        }

        /// <summary>
        /// Creates job with trigger that runs on daily basis on fixed time of day
        /// </summary>
        /// <param name="jobType">Job type</param>
        /// <param name="hours">Hours in (from 0 to 24)</param>
        /// <param name="minutes">Minutes (from 0 to 60)</param>
        protected ScheduledJob CreateDailyTriggeredJob(Type jobType, int hours, int minutes)
        {
            if (hours < 0 || hours > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hours));
            }

            if (minutes < 0 || minutes > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            var jobName = GetJobName(jobType);
            var job = CreateJob(jobType);
            var trigger = CreateDailyTrigger(jobName, hours, minutes);
            return new ScheduledJob(job, trigger);
        }

        private ITrigger CreateDailyTrigger(string jobName, int hours, int minutes)
        {
            return TriggerBuilder.Create()
                .WithIdentity($"{jobName}_Daily_{hours}_{minutes}")
                .ForJob(jobName)
                .WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(24).StartingDailyAt(new TimeOfDay(hours, minutes)))
                .Build();
        }

        private string GetJobName(Type jobType)
        {
            return jobType.Name;
        }

        private IJobDetail CreateJob<T>() where T : IJob
        {
            return CreateJob(typeof(T));
        }

        private IJobDetail CreateJob(Type jobType)
        {
            return JobBuilder.Create(jobType).WithIdentity(GetJobName(jobType)).Build();
        }

        private void ScheduleRegularJobs(IScheduler scheduler)
        {
            foreach (var item in Jobs)
            {
                scheduler.ScheduleJob(item.JobDetail, item.Trigger);
            }
        }

        private void ScheduleImmediateJob(IScheduler scheduler, string jobName, object options = null)
        {
            var immediateJob = Jobs.Single(job => job.JobDetail.Key.Name == jobName).JobDetail.DeepClone();
            if (options != null)
            {
                var optionProperties = options.GetType().GetProperties();
                foreach (var optionProperty in optionProperties)
                {
                    immediateJob.JobDataMap[optionProperty.Name] = optionProperty.GetValue(options, null);
                }
            }

            var item = CreateNowTriggeredJob(immediateJob.JobType);
            scheduler.ScheduleJob(item.JobDetail, item.Trigger);
        }

        private Type FindBaseJobType(Assembly assembly)
        {
            return assembly.GetTypes().SingleOrDefault(t => t.Name == _baseOptions.JobName)?.BaseType;
        }
    }
}