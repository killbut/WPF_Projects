using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GameRoyak.Logic
{
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Tuple<object, object> tuple = new Tuple<object, object>(
            //    (object)values[0], (object)values[1]);
            List<object> param = new List<object>();
            foreach (var item in values)
            {
              param.Add(item);   
            }
            return (object)param;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
