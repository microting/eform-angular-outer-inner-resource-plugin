using System;

namespace MachineArea.Pn.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
        {
            int offset = dayOfWeek - date.DayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime LastDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime ldowDate = FirstDayOfWeek(date, dayOfWeek).AddDays(6);
            return ldowDate;
        }

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return date.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }
    }
}
