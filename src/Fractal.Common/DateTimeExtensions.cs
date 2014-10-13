using System;

namespace Fractal.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddMilliseconds(-1);
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

    }
}