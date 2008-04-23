using System;
using System.Collections;

namespace OMLEngine
{
    public class SortedArrayList : ArrayList
    {
        public void AddSorted(object item)
        {
            int position = this.BinarySearch(item);
            if (position < 0)
            {
                position = ~position;
            }

            this.Insert(position, item);
        }
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
