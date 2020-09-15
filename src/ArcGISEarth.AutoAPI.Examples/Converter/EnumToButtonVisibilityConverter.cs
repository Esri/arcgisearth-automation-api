using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static ArcGISEarth.AutoAPI.Examples.MainWindowViewModel;

namespace ArcGISEarth.AutoAPI.Examples.Converter
{
    public class EnumToButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sendButtonType = (SendButtonType)value;
            var visibility = Visibility.Visible;
            switch (sendButtonType)
            {
                case SendButtonType.GetCamera:
                case SendButtonType.GetWorkspace:
                case SendButtonType.ClearWorkspace:
                    visibility = Visibility.Visible;
                    break;
                case SendButtonType.SetCamera:                    
                case SendButtonType.SetFlight:                    
                case SendButtonType.AddLayer:
                case SendButtonType.GetLayer:
                case SendButtonType.RemoveLayer:
                case SendButtonType.ClearLayers:
                case SendButtonType.ImportWorkspace:
                case SendButtonType.TakeSnapshot:
                    visibility = Visibility.Collapsed;
                    break;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToTextBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sendButtonType = (SendButtonType)value;
            var visibility = Visibility.Collapsed;
            switch (sendButtonType)
            {
                case SendButtonType.GetCamera:
                case SendButtonType.GetWorkspace:
                case SendButtonType.ClearWorkspace:
                    visibility = Visibility.Collapsed;
                    break;
                case SendButtonType.SetCamera:
                case SendButtonType.SetFlight:
                case SendButtonType.AddLayer:
                case SendButtonType.GetLayer:
                case SendButtonType.RemoveLayer:
                case SendButtonType.ClearLayers:
                case SendButtonType.ImportWorkspace:
                case SendButtonType.TakeSnapshot:
                    visibility = Visibility.Visible;
                    break;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
