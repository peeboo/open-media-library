using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine
{
    public static class TitleConfig
    {
        private const string LENGTH_ZERO = "Unknown duration";
        private const string LENGTH_THIRTY = "30 minutes or less";
        private const string LENGTH_HOUR = "1 hour or less";
        private const string LENGTH_HOUR_HALF = "1.5 hours or less";
        private const string LENGTH_TWO_HOURS = "2 hours or less";
        private const string LENGTH_TWO_HOURS_HALF = "2.5 hours or less";
        private const string LENGTH_THREE_HOURS = "3 hours or less";
        private const string LENGTH_OVER = "Over 3 hours";

        public const int MAX_RUNTIME = 180;

        public readonly static int[] RUNTIME_FILTER_LENGTHS = new int[]{ 0, 30, 60, 90, 120, 150, 180, int.MaxValue };

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
