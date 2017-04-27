using System;

namespace Sintek.Schedule.Core.Misc
{
    public class FailDetails
    {
        public int Count { get; private set; }

        public DateTime FirstFail { get; private set; } = DateTime.Now;

        public static FailDetails operator ++(FailDetails f)
        {
            ++f.Count;
            return f;
        }
    }
}
