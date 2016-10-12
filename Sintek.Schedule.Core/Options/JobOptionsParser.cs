using System;
using Quartz;

namespace Sintek.Schedule.Core.Options
{
    public class JobOptionsParser
    {
        public T Parse<T>(JobDataMap dataMap)
        {
            var options = Activator.CreateInstance<T>();
            var optionsType = typeof(T);
            foreach (var key in dataMap.Keys)
            {
                optionsType.GetProperty(key).SetValue(options, dataMap[key], null);
            }

            return options;
        }
    }
}
