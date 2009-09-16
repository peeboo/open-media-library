using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class WrappedIndexTransformer : ITransformer
    {
        // Fields
        private int m_cItems;

        // Methods
        public object Transform(object value)
        {
            int idxData;
            int nGeneration;
            int idx = (int)value;
            ListUtility.GetWrappedIndex(idx, this.m_cItems, out idxData, out nGeneration);
            return idxData;
        }

        // Properties
        public int Count
        {
            get
            {
                return this.m_cItems;
            }
            set
            {
                this.m_cItems = value;
            }
        }
    }
}
