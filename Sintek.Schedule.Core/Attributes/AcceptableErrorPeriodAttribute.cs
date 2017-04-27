using System;

namespace Sintek.Schedule.Core.Attributes
{
    public enum IntervalType
    {
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AcceptableErrorPeriodAttribute : Attribute
    {
        public AcceptableErrorPeriodAttribute(int value, IntervalType valueType)
        {
        }
    }
}
