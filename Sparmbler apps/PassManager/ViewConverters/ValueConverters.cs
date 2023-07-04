using PassManager.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{

    internal abstract class UniversalPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetProperty();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        protected abstract string GetProperty();
    }
    internal class NamePropertyConverter : UniversalPropertyConverter
    {
        protected override string GetProperty()
        {
            return Resources.NameProperty;
        }
    }

    internal class PathPropertyConverter : UniversalPropertyConverter
    {
        protected override string GetProperty()
        {
            return Resources.PathProperty;
        }
    }

}
