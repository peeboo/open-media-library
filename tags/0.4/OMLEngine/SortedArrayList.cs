using System;
using System.Collections;

namespace OMLEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class SortedArrayList : ArrayList
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddSorted(object item)
        {
            int position = this.BinarySearch(item);
            if (position < 0)
            {
                position = ~position;
            }

            this.Insert(position, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public void ModifySorted(object item, int index)
        {
            this.RemoveAt(index);

            int position = this.BinarySearch(item);
            if (position < 0)
            {
                position = ~position;
            }

            this.Insert(position, item);
        }
    }
}
