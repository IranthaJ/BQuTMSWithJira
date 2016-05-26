using System;
using System.Globalization;
using System.Windows.Data;

namespace BQuTMSWithJira
{
    //leave history table hilight "Rejected" and "Accepted" leave code
    class NumberToPolarValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sta = (string)System.Convert.ChangeType(value, typeof(string));

            if (sta.Equals("Rejected"))
                return -1;
            if (sta.Equals("Accepted"))
                return 0;
            return +1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported");
        }
    }
}
