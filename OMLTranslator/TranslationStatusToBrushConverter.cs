using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace OMLTranslator
{
    [ValueConversion(typeof(TranslationStatus), typeof(Brush))]
    public class TranslationStatusToBrushConverter : IValueConverter
    {
        private static readonly Dictionary<TranslationStatus, Brush> brushes = new Dictionary<TranslationStatus, Brush>
        {
            { TranslationStatus.Ok, Brushes.White },
            { TranslationStatus.Warning, Brushes.LightYellow },
            { TranslationStatus.Error, Brushes.LightPink }
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return brushes[(TranslationStatus)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var q = from entry in brushes
                    where entry.Value.Equals(value)
                    select entry.Key;
            return q.First();
        }
    }
}
