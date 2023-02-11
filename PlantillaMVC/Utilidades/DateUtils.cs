using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantillaMVC
{
    public class DateUtils
    {
        private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private DateTime fromDate;
        private DateTime toDate;
        private int year;
        private int month;
        private int day;
        private int increment;
        public string DateDifference(DateTime d1, DateTime d2)
        {
            if (d1 > d2)
            {
                this.fromDate = d2;
                this.toDate = d1;
            }
            else
            {
                this.fromDate = d1;
                this.toDate = d2;
            }
            increment = 0;
            if (this.fromDate.Day > this.toDate.Day)
            {
                increment = this.monthDay[this.fromDate.Month - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(this.fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (this.toDate.Day + increment) - this.fromDate.Day;
                increment = 1;
            }
            else
            {
                day = this.toDate.Day - this.fromDate.Day;
            }
            if ((this.fromDate.Month + increment) > this.toDate.Month)
            {
                this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                increment = 0;
            }
            this.year = this.toDate.Year - (this.fromDate.Year + increment);
            return this.year + " año(s), " + this.month + " mes(es), " + this.day + " días";
        }
    }
}