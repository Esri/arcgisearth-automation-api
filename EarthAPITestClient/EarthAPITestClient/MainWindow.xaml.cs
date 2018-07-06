// Copyright 2017 Esri
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EarthAPITestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel v = new ViewModel();
            this.DataContext = v;
        }

        private void GetCameraButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
             v.ExecuteFuction(FunctionType.GetCamera);
        }

        private void SetCameraButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.SetCamera);
        }

        private void FlyToButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.FlyTo);
        }

        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.AddLayer);
        }

        private void ClearLayersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.ClearLayers);
        }

        private void GetSnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.GetSnapshot);
        }

        private void ConnectEarth_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.Connect);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.Help);
        }

        private void ClearTextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.ClearTextBox);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.CloseConnect);
        }

        private void AddLayerSync_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.AddLayerSync);
        }

        private void GetLayerInfo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.GetLayerInfo);
        }

        private void GetLayersInfo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.GetLayersInfo);
        }

        private void ImportWorkspace_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.ImportWorkspace);
        }

        private void Removelayer_Click(object sender, RoutedEventArgs e)
        {
            ViewModel v = this.DataContext as ViewModel;
            v.ExecuteFuction(FunctionType.Removelayer);
        }
    }
}
