using System;
using System.Linq;
using System.Reflection;
using Sintek.Schedule.Core.Attributes;

namespace Sintek.Schedule.Core.Misc
{
    public static class RefireConditionsChecker
    {
        public static bool CheckConditions(Type jobType, FailDetails details)
        {
            var countAttr = GetArgument(jobType, typeof(AcceptableErrorCountAttribute));
            var periodAttr = GetArgument(jobType, typeof(AcceptableErrorPeriodAttribute));
            if (countAttr == null && periodAttr == null)
            {
                return false;
            }

            var refireCount = (countAttr?.ConstructorArguments[0].Value as int?) ?? 1;
            TimeSpan refirePeriod = TimeSpan.MaxValue;
            if (periodAttr != null)
            {
                var argValue = (int)periodAttr.ConstructorArguments[0].Value;
                switch ((IntervalType)periodAttr.ConstructorArguments[1].Value)
                {
                    case IntervalType.Days:
                        refirePeriod = TimeSpan.FromDays(argValue);
                        break;
                    case IntervalType.Hours:
                        refirePeriod = TimeSpan.FromHours(argValue);
                        break;
                    case IntervalType.Minutes:
                        refirePeriod = TimeSpan.FromMinutes(argValue);
                        break;
                    case IntervalType.Seconds:
                        refirePeriod = TimeSpan.FromSeconds(argValue);
                        break;
                    case IntervalType.Milliseconds:
                        refirePeriod = TimeSpan.FromMilliseconds(argValue);
                        break;
                    default:
                        throw new ArgumentException("Missing parameter");
                }
            }

            if (DateTime.Now - details.FirstFail < refirePeriod)
            {
                if (details.Count < refireCount)
                {
                    return true;
                }
                else
                {
                    details = new FailDetails();

                    return false;
                }
            }
            else
            {
                details = new FailDetails();

                return true;
            }
        }

        private static CustomAttributeData GetArgument(Type jobType, Type attrType)
        {
            return jobType.IsDefined(attrType, true) ?
                jobType.CustomAttributes.Where(x => x.AttributeType == attrType).Single() : null;
        }
    }
}
