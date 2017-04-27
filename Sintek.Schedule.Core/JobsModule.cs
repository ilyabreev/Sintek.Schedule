namespace Sintek.Schedule.Core
{
    using System.Reflection;
    using Autofac;
    using Autofac.Extras.Quartz;
    using Module = Autofac.Module;

    public class JobsModule : Module
    {
        private readonly Assembly _jobsAssembly;

        public JobsModule(Assembly jobsAssembly)
        {
            _jobsAssembly = jobsAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(
                new QuartzAutofacJobsModule(_jobsAssembly)
                {
                    AutoWireProperties = true
                });
        }
    }
}
