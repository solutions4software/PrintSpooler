using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSpooler.Utilities
{
    class TimeConverter
    {
        // Convert time as minutes past UTC midnight into human readable time in local time zone.
        internal static DateTime ConvertToLocalHumanReadableTime(Int32 timeInMinutesAfterUTCMidnight)
        {
            // Construct a UTC midnight object.
            // Must start with current date so that the local Daylight Savings system, if any, will be taken into account.
            DateTime utcNow = DateTime.UtcNow;
            DateTime utcMidnight = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);

            // Add the minutes passed into the method in order to get the intended UTC time.
            Double minutesAfterUTCMidnight = (Double)timeInMinutesAfterUTCMidnight;
            DateTime utcTime = utcMidnight.AddMinutes(minutesAfterUTCMidnight);

            // Convert to local time.
            DateTime localTime = utcTime.ToLocalTime();

            return localTime;

        }// end ConvertToLocalHumanReadableTime

    }//end TimeConverter class
}
