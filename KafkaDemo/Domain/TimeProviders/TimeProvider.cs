using Domain.Contracts;
using System;

namespace Domain.TimeProviders
{
    public class TimeProvider : ITimeProvider
    {
        public int GetMilisecondsForMinutes(int minutes)
        {
            if (minutes < 0)
            {
                throw new ArgumentException("Minutes must be a positive integer number.");
            }

            return minutes * GetMilisecondsForSeconds(60);
        }

        public int GetMilisecondsForSeconds(int seconds)
        {
            if (seconds < 0)
            {
                throw new ArgumentException("Minutes must be a positive integer number.");
            }

            return seconds * 1000;
        }
    }
}
