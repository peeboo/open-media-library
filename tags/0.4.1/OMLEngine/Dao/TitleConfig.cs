using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine
{    
    public static class TitleConfig
    {
        private const string DATE_TODAY = "Today";
        private const string DATE_YESTERDAY = "Yesterday";
        private const string DATE_LAST_WEEK = "Within Last week";
        private const string DATE_TWO_WEEKS = "Within Last 2 Weeks";
        private const string DATE_LAST_MONTH = "Within Last Month";
        private const string DATE_THREE_MONTHS = "Within Last 3 Months";
        private const string DATE_SIX_MONTHS = "Within Last 6 Months";
        private const string DATE_LAST_YEAR = "Within Last Year";
        private const string DATE_OVER_YEAR = "More Than 1 Year";
        
        private const string LENGTH_ZERO = "Unknown duration";
        private const string LENGTH_THIRTY = "30 minutes or less";
        private const string LENGTH_HOUR = "1 hour or less";
        private const string LENGTH_HOUR_HALF = "1.5 hours or less";
        private const string LENGTH_TWO_HOURS = "2 hours or less";
        private const string LENGTH_TWO_HOURS_HALF = "2.5 hours or less";
        private const string LENGTH_THREE_HOURS = "3 hours or less";
        private const string LENGTH_OVER = "Over 3 hours";

        public const int MAX_RUNTIME = 180;
        public const int MAX_DATE_ADDED = 365;

        public readonly static int[] RUNTIME_FILTER_LENGTHS = new int[] { 0, 30, 60, 90, 120, 150, 180, int.MaxValue };
        public readonly static IEnumerable<NumericRange> RUNTIME_RANGE = new List<NumericRange>()
        {
            new NumericRange() {Start = 0, End = 30}
            , new NumericRange() {Start = 0, End = 60}
            , new NumericRange() {Start = 0, End = 90}
            , new NumericRange() {Start = 0, End = 120}
            , new NumericRange() {Start = 0, End = 150}
            , new NumericRange() {Start = 0, End = 180}
            , new NumericRange() {Start = 181, End = int.MaxValue}
        };
        
        public readonly static int[] ADDED_FILTER_DATE = new int[] { 0, 1, 7, 14, 31, 92, 184, 365, int.MaxValue };
        public readonly static IEnumerable<NumericRange> DATE_ADDED_RANGE = new List<NumericRange>()
        {
            new NumericRange() {Start = 0, End = 1}
            , new NumericRange() {Start = 1, End = 2}
            , new NumericRange() {Start = 2, End = 7}
            , new NumericRange() {Start = 7, End = 14}
            , new NumericRange() {Start = 14, End = 31}
            , new NumericRange() {Start = 31, End = 92}
            , new NumericRange() {Start = 92, End = 184}
            , new NumericRange() {Start = 184, End = 365}
            , new NumericRange() {Start = 365, End = int.MaxValue}
        };

        public struct NumericRange
        {
            public int Start;
            public int End;
        }

        /// <summary>
        /// Turns a timespan into a filter string
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static string DaysToFilterString(int days)
        {
            if (days <= 1)
                return DATE_TODAY;
            else if (days <= 2)
                return DATE_YESTERDAY;
            else if (days <= 7)
                return DATE_LAST_WEEK;
            else if (days <= 14)
                return DATE_TWO_WEEKS;
            else if (days <= 31)
                return DATE_LAST_MONTH;
            else if (days <= 92)
                return DATE_THREE_MONTHS;
            else if (days <= 184)
                return DATE_SIX_MONTHS;
            else if (days <= 365)
                return DATE_LAST_YEAR;
            else
                return DATE_OVER_YEAR;
        }

        /// <summary>
        /// Returns a timespan in days for the given filter string
        /// </summary>
        /// <param name="dateAddedAgo"></param>
        /// <returns></returns>
        public static int DateAddedFilterStringToInt(string dateAddedAgo)
        {
            switch (dateAddedAgo)
            {
                case DATE_TODAY:
                    return 0;

                case DATE_YESTERDAY:
                    return 1;

                case DATE_LAST_WEEK:
                    return 7;

                case DATE_TWO_WEEKS:
                    return 14;

                case DATE_LAST_MONTH:
                    return 31;

                case DATE_THREE_MONTHS:
                    return 92;

                case DATE_SIX_MONTHS:
                    return 184;

                case DATE_LAST_YEAR:
                    return 365;
            }

            return -1;
        }

        /// <summary>
        /// Returns a runtime filter string given a time chunk
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public static string RuntimeToFilterString(int runtime)
        {
            if ( runtime <= 0 )
                return LENGTH_ZERO;
            else if ( runtime <= 30)
                return LENGTH_THIRTY;
            else if ( runtime <= 60)
                return LENGTH_HOUR;
            else if ( runtime <= 90 )
                return LENGTH_HOUR_HALF;
            else if ( runtime <= 120)
                return LENGTH_TWO_HOURS;
            else if ( runtime <= 150)
                return LENGTH_TWO_HOURS_HALF;
            else if ( runtime <= 180)
                return LENGTH_THREE_HOURS;
            else
                return LENGTH_OVER;            
        }        

        /// <summary>
        /// todo : solomon : i don't love the dao layer being aware of this - but it makes a lot of things much simplier
        /// maybe when it's moved into a config it'll be more elegant
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public static int RuntimeFilterStringToInt(string runtime)
        {
            switch (runtime)
            {
                case LENGTH_ZERO:
                    return 0;

                case LENGTH_THIRTY:
                    return 30;

                case LENGTH_HOUR:
                    return 60;

                case LENGTH_HOUR_HALF:
                    return 90;

                case LENGTH_TWO_HOURS:
                    return 120;

                case LENGTH_TWO_HOURS_HALF:
                    return 150;

                case LENGTH_THREE_HOURS:
                    return 180;
            }

            return -1;
        }
    }
}
