// Copyright 2020 Esri
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArcGISEarth.AutoAPI.Examples.Converter
{
    public class EnumToButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sendButtonType = (FunctionType)value;
            var visibility = Visibility.Visible;
            switch (sendButtonType)
            {
                case FunctionType.GetCamera:
                case FunctionType.GetWorkspace:
                case FunctionType.ClearWorkspace:
                case FunctionType.TakeSnapshot:
                    visibility = Visibility.Visible;
                    break;
                case FunctionType.SetCamera:                    
                case FunctionType.SetFlight:                    
                case FunctionType.AddLayer:
                case FunctionType.GetLayer:
                case FunctionType.RemoveLayer:
                case FunctionType.ClearLayers:
                case FunctionType.ImportWorkspace:
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
            var sendButtonType = (FunctionType)value;
            var visibility = Visibility.Collapsed;
            switch (sendButtonType)
            {
                case FunctionType.GetCamera:
                case FunctionType.GetWorkspace:
                case FunctionType.ClearWorkspace:
                case FunctionType.TakeSnapshot:
                    visibility = Visibility.Collapsed;
                    break;
                case FunctionType.SetCamera:
                case FunctionType.SetFlight:
                case FunctionType.AddLayer:
                case FunctionType.GetLayer:
                case FunctionType.RemoveLayer:
                case FunctionType.ClearLayers:
                case FunctionType.ImportWorkspace:
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

    public class EnumToImageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sendButtonType = (FunctionType)value;
            var visibility = Visibility.Collapsed;

            switch (sendButtonType)
            {
                case FunctionType.GetCamera:
                case FunctionType.GetWorkspace:
                case FunctionType.ClearWorkspace:
                case FunctionType.SetCamera:
                case FunctionType.SetFlight:
                case FunctionType.AddLayer:
                case FunctionType.GetLayer:
                case FunctionType.RemoveLayer:
                case FunctionType.ClearLayers:
                case FunctionType.ImportWorkspace:
                    visibility = Visibility.Collapsed;
                    break;
                case FunctionType.TakeSnapshot:
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
