using System;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace Library.Code.V3
{
    [Flags, MarkupVisible]
    public enum FontStyles
    {
        None,
        Bold,
        Italic
    }

    [MarkupVisible]
    public class FancyTextHelper
    {
        public Font Font { get; set; }
        private string fontString;
        public object FontString
        {
            get
            {
                return this.fontString;
            }
            set
            {
                //Type myType = value.GetType();
                //PrivateObjectWeasel weasel2 = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.Font, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { });
                //PropertyInfo inf = myType.GetProperty("Layout", BindingFlags.Public | BindingFlags.Instance);
                //inf.SetValue(Panel, (object)weasel2.Instance, null);
                this.fontString = value.ToString();
            }
        }

        public string FontName { get; set; }
        public float FontSize { get; set; }
        private FontStyles fontStyle;
        public object FontStyle
        {
            get
            {
                return this.fontString;
            }
            set
            {
                this.fontStyle = (FontStyles)value;
            }
        }

        public string Content { get; set; }
        public System.Single MaximumWidth { get; set; }
        public int MaximumWidthInt
        {
            get
            {
                return Convert.ToInt32(this.MaximumWidth);
            }
            set
            {
                this.MaximumWidth = Convert.ToSingle(value)-24;//TODO: margins are currently hard coded...;
            }
        }
        public bool AllowMarquee { get; set; }

        private bool shouldScroll = false;
        private bool checkedScroll = false;
        public bool ShouldScroll
        {
            get
            {
                if (!checkedScroll)
                {
                    checkedScroll = true;
                    Bitmap bmp = new Bitmap(1, 1);
                    Graphics graphics = Graphics.FromImage(bmp);
                    string test = "this is a test";
                    Font f = new Font(FontName, FontSize);
                    if (MeasureDisplayStringWidth(graphics, this.Content, f) > MaximumWidth && MaximumWidth>0)
                    {
                        this.shouldScroll = true;
                        return true;
                    }
                }
                return this.shouldScroll;
            }
        }

        static public System.Single GetSingle(int Integer)
        {
            return Convert.ToSingle(Integer);
        }
        static public int MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            System.Drawing.CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Right + 1.0f);
        }

        static public bool MeasureDisplayStringWidth2(string text, string font, int width)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            string test = "this is a test";
            Font f = new Font("Arial", 16);
            if (MeasureDisplayStringWidth(g, test, f) > width)
                return true;
            return false;
        }
    }
}
