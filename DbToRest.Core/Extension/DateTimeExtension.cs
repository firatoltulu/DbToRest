﻿using System;

namespace DbToRest.Core
{
    public static class DateTimeExtension
    {
        public static string HumanFriendlyString(this DateTime datetime)
        {
            TimeSpan ts = DateTime.Now.Subtract(datetime);

            // The trick: make variable contain date and time representing the desired timespan,
            // having +1 in each date component.
            DateTime date = DateTime.MinValue + ts;

            return ProcessPeriod(date.Year - 1, date.Month - 1, "Yıl")
                   ?? ProcessPeriod(date.Month - 1, date.Day - 1, "Ay")
                   ?? ProcessPeriod(date.Day - 1, date.Hour, "Gün", "Dün")
                   ?? ProcessPeriod(date.Hour, date.Minute, "Saat")
                   ?? ProcessPeriod(date.Minute, date.Second, "Dakika")
                   ?? ProcessPeriod(date.Second, 0, "Saniye")
                   ?? "Şimdi";
        }

        private static string ProcessPeriod(int value, int subValue, string name, string singularName = null)
        {
            if (value == 0)
            {
                return null;
            }
            if (value == 1)
            {
                if (!String.IsNullOrEmpty(singularName))
                {
                    return singularName;
                }
                string articleSuffix = name[0] == 'h' ? "n" : String.Empty;
                return subValue == 0
                    ? String.Format("Bir{0} {1} Önce", articleSuffix, name)
                    : String.Format("bir{0} {1} önce", articleSuffix, name);
            }
            return subValue == 0
                ? String.Format("{0} {1} önce", value, name)
                : String.Format("{0} {1} önce", value, name);
        }

        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        public static int Age(this DateTime date)
        {
            var now = DateTime.Now;
            var age = now.Year - date.Year;
            if (now.Month > date.Month || (now.Month == date.Month && now.Day < date.Day))
                age -= 1;
            return age;
        }

        public static int AgeInDays(this DateTime date)
        {
            TimeSpan ts = DateTime.Now - date;
            return ts.Duration().Days;
        }

        public static double RemainingDaysInYear(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, 12, 31).Date - date.Date;
            return ts.Duration().TotalDays;
        }

        public static DateTime SetTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static double RemainingDaysInMonth(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Date - date.Date;
            return ts.Duration().TotalDays;
        }

        public static double RemainingHoursInYear(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, 12, 31, 23, 59, 59, 999) - date;
            return ts.Duration().TotalHours;
        }

        public static double RemainingHoursInMonth(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1) - date;
            return ts.Duration().TotalHours;
        }

        public static double RemainingHoursInDay(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999) - date;
            return ts.Duration().TotalHours;
        }

        public static double RemainingMinutesInYear(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, 12, 31, 23, 59, 59, 999) - date;
            return ts.Duration().TotalMinutes;
        }

        public static double RemainingMinutesInMonth(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, 1, 23, 59, 59, 999).AddMonths(1).AddDays(-1) - date;
            return ts.Duration().TotalMinutes;
        }

        public static double RemainingMinutesInDay(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999) - date;
            return ts.Duration().TotalMinutes;
        }

        public static double RemainingMinutesInHour(this DateTime date)
        {
            TimeSpan ts = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0).AddHours(1) - date;
            return ts.Duration().TotalMinutes;
        }

        public static bool Between(this DateTime date, DateTime first, DateTime last)
        {
            return (date.Date >= first.Date && date.Date <= last.Date) ? true : false;
        }

        public static bool IsBirthday(this DateTime date, DateTime dateToCheck)
        {
            if (dateToCheck.Year <= date.Year)
            {
                return false;
            }
            return dateToCheck.Month == date.Month && dateToCheck.Day == date.Day;
        }

        public static bool IsBirthday(this DateTime date)
        {
            if (DateTime.Now.Year <= date.Year)
            {
                return false;
            }
            return DateTime.Now.Month == date.Month && DateTime.Now.Day == date.Day;
        }

        public static bool IsFuture(this DateTime date)
        {
            return (date.Date > DateTime.Now) ? true : false;
        }

        public static bool IsPast(this DateTime date)
        {
            return (date.Date < DateTime.Now) ? true : false;
        }

        public static bool IsSunday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Sunday)) ? true : false;
        }

        public static bool IsMonday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Monday)) ? true : false;
        }

        public static bool IsTuesday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Tuesday)) ? true : false;
        }

        public static bool IsWednesday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Wednesday)) ? true : false;
        }

        public static bool IsThursday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Thursday)) ? true : false;
        }

        public static bool IsFriday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Friday)) ? true : false;
        }

        public static bool IsSaturday(this DateTime date)
        {
            return (date.DayOfWeek.Equals(DayOfWeek.Saturday)) ? true : false;
        }

        public static DateTime NextSunday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Sunday);
        }

        public static DateTime NextMonday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Monday);
        }

        public static DateTime NextTuesday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Tuesday);
        }

        public static DateTime NextWednesday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Wednesday);
        }

        public static DateTime NextThursday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Thursday);
        }

        public static DateTime NextFriday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Friday);
        }

        public static DateTime NextSaturday(this DateTime date)
        {
            return CheckNextDayOfWeek(date, DayOfWeek.Saturday);
        }

        private static DateTime CheckNextDayOfWeek(DateTime date, DayOfWeek returnDay)
        {
            int i = 0;
            while (!date.DayOfWeek.Equals(returnDay))
            {
                date = date.AddDays(1);
                i++;
            }

            return (i == 0) ? date.AddDays(7) : date;
        }

        public static DateTime LastSunday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Sunday);
        }

        public static DateTime LastMonday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Monday);
        }

        public static DateTime LastTuesday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Tuesday);
        }

        public static DateTime LastWednesday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Wednesday);
        }

        public static DateTime LastThursday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Thursday);
        }

        public static DateTime LastFriday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Friday);
        }

        public static DateTime LastSaturday(this DateTime date)
        {
            return CheckLastDayOfWeek(date, DayOfWeek.Saturday);
        }

        private static DateTime CheckLastDayOfWeek(DateTime date, DayOfWeek returnDay)
        {
            int i = 0;
            while (!date.DayOfWeek.Equals(returnDay))
            {
                date = date.AddDays(-1);
                i++;
            }

            return (i == 0) ? date.AddDays(-7) : date;
        }
    }
}