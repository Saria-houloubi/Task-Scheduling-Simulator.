﻿using System;
using System.Globalization;
using System.Windows;

namespace TishreenUniversity.ParallelPro.IValueConverters
{
    /// <summary>
    /// Returns the page that that we want
    /// </summary>
    public class StringToVisiblityValueConverter : BaseValueConverters<StringToVisiblityValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
