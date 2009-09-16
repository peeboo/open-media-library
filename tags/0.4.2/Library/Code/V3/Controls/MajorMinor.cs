using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Library.Code.V3
{
    [StructLayout(LayoutKind.Sequential), MarkupVisible]
    public struct MajorMinor
    {
        private int major;
        private int minor;
        public static readonly MajorMinor Zero;
        [MarkupVisible]
        public MajorMinor(int major, int minor)
        {
            this.major = major;
            this.minor = minor;
        }

        public MajorMinor(Size size, Orientation o)
        {
            switch (o)
            {
                case Orientation.Horizontal:
                    this.major = size.Width;
                    this.minor = size.Height;
                    return;
            }
            this.major = size.Height;
            this.minor = size.Width;
        }

        public Size ToSize(Orientation o)
        {
            switch (o)
            {
                case Orientation.Horizontal:
                    return new Size(this.major, this.minor);
            }
            return new Size(this.minor, this.major);
        }

        public Point ToPoint(Orientation o)
        {
            return this.ToSize(o).ToPoint();
        }

        public static MajorMinor Min(MajorMinor a, MajorMinor b)
        {
            return new MajorMinor(Math.Min(a.Major, b.Major), Math.Min(a.Minor, b.Minor));
        }

        public static MajorMinor Max(MajorMinor a, MajorMinor b)
        {
            return new MajorMinor(Math.Max(a.Major, b.Major), Math.Max(a.Minor, b.Minor));
        }

        public static MajorMinor operator +(MajorMinor left, MajorMinor right)
        {
            return new MajorMinor(left.Major + right.Major, left.Minor + right.Minor);
        }

        public static MajorMinor operator -(MajorMinor left, MajorMinor right)
        {
            return new MajorMinor(left.Major - right.Major, left.Minor - right.Minor);
        }

        public static MajorMinor operator *(MajorMinor left, MajorMinor right)
        {
            return new MajorMinor(left.Major * right.Major, left.Minor * right.Minor);
        }

        public static MajorMinor operator /(MajorMinor left, MajorMinor right)
        {
            return new MajorMinor(left.Major / right.Major, left.Minor / right.Minor);
        }

        public static bool operator ==(MajorMinor left, MajorMinor right)
        {
            if (left.Major == right.Major)
            {
                return (left.Minor == right.Minor);
            }
            return false;
        }

        public static bool operator !=(MajorMinor left, MajorMinor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MajorMinor))
            {
                return false;
            }
            MajorMinor minor = (MajorMinor)obj;
            return (minor == this);
        }

        public override int GetHashCode()
        {
            return (this.major ^ this.minor);
        }

        public MajorMinor Swap()
        {
            return new MajorMinor(this.Minor, this.Major);
        }

        [MarkupVisible]
        public int Major
        {
            get
            {
                return this.major;
            }
            set
            {
                this.major = value;
            }
        }
        [MarkupVisible]
        public int Minor
        {
            get
            {
                return this.minor;
            }
            set
            {
                this.minor = value;
            }
        }
        public bool IsEmpty
        {
            get
            {
                if (this.Major != 0)
                {
                    return (this.Minor == 0);
                }
                return true;
            }
        }
        public override string ToString()
        {
            return InvariantString.Format("(Major={0}, Minor={1})", new object[] { this.Major, this.Minor });
        }

        static MajorMinor()
        {
            Zero = new MajorMinor(0, 0);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        private int m_width;
        private int m_height;
        internal static readonly Size Empty;
        internal static readonly object EmptyObject;
        public static readonly Size Zero;
        public Size(int width, int height)
        {
            this.m_width = width;
            this.m_height = height;
        }

        internal Size(Point point)
        {
            this.m_width = point.X;
            this.m_height = point.Y;
        }

        public int Width
        {
            get
            {
                return this.m_width;
            }
            set
            {
                this.m_width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.m_height;
            }
            set
            {
                this.m_height = value;
            }
        }
        public static Size operator +(Size left, Size right)
        {
            return new Size(left.Width + right.Width, left.Height + right.Height);
        }

        public static Size Add(Size left, Size right)
        {
            return (left + right);
        }

        public static Size operator -(Size left, Size right)
        {
            return new Size(left.Width - right.Width, left.Height - right.Height);
        }

        public static Size Subtract(Size left, Size right)
        {
            return (left - right);
        }

        public static bool operator ==(Size left, Size right)
        {
            if (left.Width == right.Width)
            {
                return (left.Height == right.Height);
            }
            return false;
        }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                Size size = (Size)obj;
                if (size.Width == this.Width)
                {
                    return (size.Height == this.Height);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.Width ^ this.Height);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("(Width=");
            builder.Append(this.Width);
            builder.Append(", Height=");
            builder.Append(this.Height);
            builder.Append(")");
            return builder.ToString();
        }

        internal SizeF ToSizeF()
        {
            return new SizeF((float)this.Width, (float)this.Height);
        }

        internal SIZE ToSIZE()
        {
            return new SIZE(this.Width, this.Height);
        }

        internal Point ToPoint()
        {
            return new Point(this.Width, this.Height);
        }

        internal static Size Min(Size sz1, Size sz2)
        {
            return new Size(Math.Min(sz1.Width, sz2.Width), Math.Min(sz1.Height, sz2.Height));
        }

        internal static Size Max(Size sz1, Size sz2)
        {
            return new Size(Math.Max(sz1.Width, sz2.Width), Math.Max(sz1.Height, sz2.Height));
        }

        internal bool IsEmpty
        {
            get
            {
                return this.Equals(Zero);
            }
        }
        internal bool IsEmptyArea
        {
            get
            {
                if (this.Width != 0)
                {
                    return (this.Height == 0);
                }
                return true;
            }
        }
        internal bool IsZero
        {
            get
            {
                return this.IsEmpty;
            }
        }
        internal static Size Truncate(SizeF value)
        {
            return new Size((int)value.Width, (int)value.Height);
        }

        internal void Scale(float flScale)
        {
            this.Width = (int)(this.Width * flScale);
            this.Height = (int)(this.Height * flScale);
        }

        internal static Size Scale(Size size, float flScale)
        {
            Size size2 = size;
            size2.Scale(flScale);
            return size2;
        }

        internal static Size Parse(string str)
        {
            float[] numArray = VectorSpec.Parse(str, "size", 2);
            int width = (int)numArray[0];
            return new Size(width, (int)numArray[1]);
        }

        static Size()
        {
            Empty = Zero;
            EmptyObject = new Size();
            Zero = new Size(0, 0);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct SizeF
    {
        private float width;
        private float height;
        public static readonly SizeF Empty;
        public static readonly SizeF Zero;
        public SizeF(SizeF size)
        {
            this.width = size.width;
            this.height = size.height;
        }

        public SizeF(PointF pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            if (sz1.Width == sz2.Width)
            {
                return (sz1.Height == sz2.Height);
            }
            return false;
        }

        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            return !(sz1 == sz2);
        }

        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }

        public static explicit operator Size(SizeF size)
        {
            return Size.Truncate(size);
        }

        public bool IsEmpty
        {
            get
            {
                return this.Equals(Zero);
            }
        }
        internal bool IsZero
        {
            get
            {
                return this.IsEmpty;
            }
        }
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }
        public void Scale(float flScale)
        {
            this.Width *= flScale;
            this.Height *= flScale;
        }

        public static SizeF Scale(SizeF size, float flScale)
        {
            SizeF ef = size;
            ef.Scale(flScale);
            return ef;
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeF)
            {
                SizeF ef = (SizeF)obj;
                if (ef.Width == this.Width)
                {
                    return (ef.Height == this.Height);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.width.GetHashCode() ^ this.height.GetHashCode());
        }

        public PointF ToPointF()
        {
            return (PointF)this;
        }

        public Size ToSize()
        {
            return (Size)this;
        }

        public static SizeF Min(SizeF sz1, SizeF sz2)
        {
            return new SizeF(Math.Min(sz1.Width, sz2.Width), Math.Min(sz1.Height, sz2.Height));
        }

        public static SizeF Max(SizeF sz1, SizeF sz2)
        {
            return new SizeF(Math.Max(sz1.Width, sz2.Width), Math.Max(sz1.Height, sz2.Height));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("(Width=");
            builder.Append(this.Width.ToString(NumberFormatInfo.InvariantInfo));
            builder.Append(", Height=");
            builder.Append(this.Height.ToString(NumberFormatInfo.InvariantInfo));
            builder.Append(")");
            return builder.ToString();
        }

        public static SizeF Parse(string str)
        {
            float[] numArray = VectorSpec.Parse(str, "size", 2);
            float width = numArray[0];
            return new SizeF(width, numArray[1]);
        }

        static SizeF()
        {
            Empty = Zero;
            Zero = new SizeF(0f, 0f);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PointF
    {
        private float x;
        private float y;
        public static readonly PointF Empty;
        public static readonly PointF Zero;
        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsEmpty
        {
            get
            {
                return this.Equals(Zero);
            }
        }
        internal bool IsZero
        {
            get
            {
                return this.IsEmpty;
            }
        }
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }
        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        public static PointF operator +(PointF pt, Size sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static PointF operator +(PointF pt, SizeF sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static PointF operator -(PointF pt, Size sz)
        {
            return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public static PointF operator -(PointF pt, SizeF sz)
        {
            return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public static SizeF operator -(PointF pt1, PointF pt2)
        {
            return new SizeF(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            if (left.X == right.X)
            {
                return (left.Y == right.Y);
            }
            return false;
        }

        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public Point ToPoint()
        {
            return new Point((int)this.x, (int)this.y);
        }

        public override bool Equals(object obj)
        {
            if (obj is PointF)
            {
                PointF tf = (PointF)obj;
                if ((tf.X == this.X) && (tf.Y == this.Y))
                {
                    return tf.GetType().Equals(base.GetType());
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.x.GetHashCode() ^ this.y.GetHashCode());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("(X=");
            builder.Append(this.X.ToString(NumberFormatInfo.InvariantInfo));
            builder.Append(", Y=");
            builder.Append(this.Y.ToString(NumberFormatInfo.InvariantInfo));
            builder.Append(")");
            return builder.ToString();
        }

        public static PointF Parse(string str)
        {
            float[] numArray = VectorSpec.Parse(str, "point", 2);
            float x = numArray[0];
            return new PointF(x, numArray[1]);
        }

        static PointF()
        {
            Empty = Zero;
            Zero = new PointF(0f, 0f);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        private int m_x;
        private int m_y;
        internal static readonly Point Empty;
        public static readonly Point Zero;
        public Point(int x, int y)
        {
            this.m_x = x;
            this.m_y = y;
        }

        internal Point(Size size)
        {
            this.m_x = size.Width;
            this.m_y = size.Height;
        }

        public int X
        {
            get
            {
                return this.m_x;
            }
            set
            {
                this.m_x = value;
            }
        }
        public int Y
        {
            get
            {
                return this.m_y;
            }
            set
            {
                this.m_y = value;
            }
        }
        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point Add(Point left, Point right)
        {
            return (left + right);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point Subtract(Point left, Point right)
        {
            return (left - right);
        }

        public static Point Offset(Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point point = (Point)obj;
                if (point.X == this.X)
                {
                    return (point.Y == this.Y);
                }
            }
            return false;
        }

        public static bool operator ==(Point left, Point right)
        {
            if (left.X == right.X)
            {
                return (left.Y == right.Y);
            }
            return false;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (this.X ^ this.Y);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("(X=");
            builder.Append(this.X);
            builder.Append(", Y=");
            builder.Append(this.Y);
            builder.Append(")");
            return builder.ToString();
        }

        internal bool IsEmpty
        {
            get
            {
                return this.Equals(Zero);
            }
        }
        internal bool IsZero
        {
            get
            {
                return this.IsEmpty;
            }
        }
        internal void Offset(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        internal PointF ToPointF()
        {
            return new PointF((float)this.X, (float)this.Y);
        }

        internal POINT ToPOINT()
        {
            return new POINT(this.X, this.Y);
        }

        internal Size ToSize()
        {
            return new Size(this.X, this.Y);
        }

        internal static Point Negate(Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        internal static Point Parse(string str)
        {
            float[] numArray = VectorSpec.Parse(str, "point", 2);
            int x = (int)numArray[0];
            return new Point(x, (int)numArray[1]);
        }

        static Point()
        {
            Empty = Zero;
            Zero = new Point(0, 0);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;
        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [MarkupVisible]
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    internal class VectorSpec
    {
        // Fields
        private static char[] s_chDelimiters = new char[] { ',' };

        // Methods
        public static float[] Parse(string str)
        {
            int startIndex = str.IndexOf('(') + 1;
            int index = str.IndexOf(')');
            string[] textArray = str.Substring(startIndex, index - startIndex).Split(s_chDelimiters);
            int length = textArray.Length;
            float[] numArray = new float[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = (float)double.Parse(textArray[i], NumberFormatInfo.InvariantInfo);
            }
            return numArray;
        }

        public static float[] Parse(string str, string strPrefix, int cDigits)
        {
            str = str.ToLower(CultureInfo.InvariantCulture);
            if (str.StartsWith(strPrefix, StringComparison.InvariantCulture))
            {
                float[] numArray = Parse(str);
                return numArray;
            }
            return null;
        }
    }
}
