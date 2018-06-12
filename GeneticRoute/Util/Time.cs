using System;

namespace GeneticRoute
{
    public static class Time
    {
        public static DateTime RoundToNearestConstMinutes(this DateTime dateTime, int frequeOfReq = 15)
        {
            return dateTime.AddMinutes(
                (int)Math.Ceiling((double) dateTime.Minute / frequeOfReq) * frequeOfReq - dateTime.Minute);
        }

        public static DateTime ToDropMinutes(this DateTime dateTime)
        {
            return dateTime.AddMinutes(-dateTime.Minute);
        }
    }
}