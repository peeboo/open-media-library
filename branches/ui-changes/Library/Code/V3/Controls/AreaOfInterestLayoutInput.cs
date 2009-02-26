using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Library.Code.V3
{
    public class AreaOfInterestLayoutInput : ILayoutInput, ILayoutData
    {
        // Fields
        private string m_stId;
        private static readonly DataCookie s_propData = DataCookie.ReserveSlot();

        // Methods
        public AreaOfInterestLayoutInput()
        {
        }

        public AreaOfInterestLayoutInput(string stId)
        {
            this.m_stId = stId;
        }

        public override string ToString()
        {
            return InvariantString.Format("{0}({1})", new object[] { base.GetType().Name, this.m_stId });
        }

        // Properties
        public static DataCookie Data
        {
            get
            {
                return s_propData;
            }
        }

        public string Id
        {
            get
            {
                return this.m_stId;
            }
            set
            {
                this.m_stId = value;
            }
        }

        DataCookie ILayoutData.Data
        {
            get
            {
                return Data;
            }
        }
    }

    [MarkupVisible]
    public interface ILayoutInput : ILayoutData
    {
    }

    public interface ILayoutData
    {
        // Properties
        DataCookie Data { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DataCookie
    {
        public static readonly DataCookie NULL;
        private uint m_value;
        private DataCookie(uint value)
        {
            this.m_value = value;
        }

        public static bool operator ==(DataCookie hl, DataCookie hr)
        {
            return (hl.m_value == hr.m_value);
        }

        public static bool operator !=(DataCookie hl, DataCookie hr)
        {
            return (hl.m_value != hr.m_value);
        }

        public override bool Equals(object oCompare)
        {
            uint num;
            if (oCompare is uint)
            {
                num = (uint)oCompare;
            }
            else if (oCompare is DataCookie)
            {
                num = ((DataCookie)oCompare).m_value;
            }
            else
            {
                return false;
            }
            return (this.m_value == num);
        }

        public override int GetHashCode()
        {
            return (int)this.m_value;
        }

        internal static DataCookie FromUInt32(uint value)
        {
            return new DataCookie(value);
        }

        internal static uint ToUInt32(DataCookie handle)
        {
            return handle.m_value;
        }

        public static DataCookie ReserveSlot()
        {
            return FromUInt32(KeyAllocator.ReserveSlot());
        }

        static DataCookie()
        {
            NULL = new DataCookie();
        }
    }

    internal class KeyAllocator
    {
        // Fields
        private static int s_idxKeyGen;

        // Methods
        internal static uint ReserveSlot()
        {
            return (uint)Interlocked.Increment(ref s_idxKeyGen);
        }
    }

}
