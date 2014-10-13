using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractal.Common
{
    public class Clock
    {
        private static DateTime frozenTime;
        private static bool isFrozen;

        public static DateTime Now
        {
            get
            {
                if (isFrozen)
                {
                    return frozenTime;
                }
                return DateTime.Now;
            }
        }

        public static DateTime Today
        {
            get
            {
                if (isFrozen)
                {
                    return frozenTime.Date;
                }
                return DateTime.Today;
            }
        }

        public static DateTime Never
        {
            get
            {
                return new DateTime(2079, 1, 1);
            }
        }

        public static void Freeze()
        {
            Freeze(DateTime.Now);
        }

        public static void Freeze(DateTime timeToFreeze)
        {
            frozenTime = timeToFreeze;
            isFrozen = true;
        }

        public static DateTime MaxDateTime
        {
            get { return DateTime.MaxValue; }
        }

        public static DateTime MinDateTime
        {
            get { return new DateTime(1753, 1, 1, 12, 0, 0); }
        }

        public static DateTime HighDateTime
        {
            get { return new DateTime(2079, 1, 1, 12, 0, 0); }
        }


        public static void Thaw()
        {
            isFrozen = false;
        }

        public static bool MoveForwardOneMinute()
        {
            if (isFrozen)
            {
                frozenTime = frozenTime.AddMinutes(1);
                return true;
            }
            return false;
        }

        public static bool MoveForward(TimeSpan interval)
        {
            if (interval < TimeSpan.FromMilliseconds(0)) throw new ArgumentException("You cannot move backwards with this method!");

            if (isFrozen)
            {
                frozenTime = frozenTime.Add(interval);
                return true;
            }
            return false;
        }

        public static DateTime NowAsStandardTime
        {
            get
            {
                DateTime unadjusted = Now;
                if (unadjusted.IsDaylightSavingTime())
                {
                    return unadjusted.AddHours(-1);
                }
                return unadjusted;
            }
        }
    }

}
