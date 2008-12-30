using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace OMLTranslator
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class IsTranslatableToTextBrushConverter : IValueConverter
    {
        private static readonly Dictionary<TranslationStatus, Brush> brushes = new Dictionary<TranslationStatus, Brush>
        {
            { TranslationStatus.Ok, Brushes.White },
            { TranslationStatus.Warning, Brushes.LightYellow },
            { TranslationStatus.Error, Brushes.LightPink }
        };


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value) return Brushes.DarkGray;
            return Brushes.Black; // Not system color as the background brush is set to specific values.
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == Brushes.DarkGray) return true;
            return false;
        }

    }
}
