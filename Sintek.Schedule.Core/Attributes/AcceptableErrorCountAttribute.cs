using System;

namespace Sintek.Schedule.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AcceptableErrorCountAttribute : Attribute
    {
        public AcceptableErrorCountAttribute(int count)
        {
        }
    }
}
