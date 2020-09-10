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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace ArcGISEarth.AutoAPI.Examples
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool vis = true;
        public bool Vis
        {
            get { return vis; }
            set
            {
                if (vis != value)
                {
                    vis = value;
                    OnPropertyChanged("Vis"); // To notify when the property is changed
                }
            }
        }

        private bool visTextbox = false;
        public bool VisTextBox
        {
            get { return visTextbox; }
            set
            {
                if (visTextbox != value)
                {
                    visTextbox = value;
                    OnPropertyChanged("VisTextBox"); // To notify when the property is changed
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            Vis = true;
            VisTextBox = false;
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Vis = true;
            VisTextBox = false;
        }

        private void TextBox_Click(object sender, RoutedEventArgs e)
        {
            Vis = false;
            VisTextBox = true;
        }
    }
}
