using System;

namespace GeneticRoute
{
    public static class Time
    {
        private const int FrequeOfReq = 15;
        
        public static DateTime RoundToNearestConstMinutes(this DateTime dateTime)
        {
            return dateTime.AddMinutes(
                ((int)Math.Ceiling((double) dateTime.Minute / FrequeOfReq)) * FrequeOfReq - dateTime.Minute);
        }

        public static DateTime ToDropMinutes(this DateTime dateTime)
        {
            return dateTime.AddMinutes(-dateTime.Minute);
        }
    }
}