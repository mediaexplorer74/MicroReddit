﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RedditUWP
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime postDate = DateTime.Parse(value.ToString());

            var diffDates = ((DateTime.Now.Year - postDate.Year) * 12) +  DateTime.Now.Month - postDate.Month;

            return diffDates;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
