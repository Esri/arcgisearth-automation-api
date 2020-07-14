// Copyright 2018 Esri
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

using ArcGISEarth.AutoAPI.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ArcGISEarth.AutoAPI.Examples
{
    internal enum FunctionType
    {
        // UI function type
        GetCamera,
        SetCamera,
        SetFlight,
        AddLayer,
        GetLayer,
        RemoveLayer,
        ClearLayers,
        GetWorkspace,
        SetWorkspace,
        ClearWorkspace,
        GetSnapshot,
        ClearInputputBox,
        ClearOutputBox,
        Help
    }


    internal class FunctionTypeCommand : ICommand
    {
        // Implement ICommand

        private readonly Action<object> _execute;

        public FunctionTypeCommand(Action<object> action)
        {
            _execute = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AutomationAPIHelper _helper;

        // InputBox text.
        private string inputString;

        public string InputString
        {
            get { return inputString; }
            set
            {
                inputString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputString"));
            }
        }

        // OutputBox text.
        private string outputString;

        public string OutputString
        {
            get { return outputString; }
            set
            {
                outputString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OutputString"));
            }
        }

        // Function command.
        public ICommand GetCameraCommand { get; private set; }

        public ICommand SetCameraCommand { get; private set; }

        public ICommand SetFlightCommand { get; private set; }

        public ICommand AddLayerCommand { get; private set; }

        public ICommand GetLayerCommand { get; private set; }

        public ICommand RemoveLayerCommand { get; private set; }

        public ICommand ClearLayersCommand { get; private set; }

        public ICommand GetWorkspaceCommand { get; private set; }

        public ICommand SetWorkspaceCommand { get; private set; }

        public ICommand ClearWorkspaceCommand { get; private set; }

        public ICommand GetSnapshotCommand { get; private set; }

        public ICommand ClearInputBoxCommand { get; private set; }

        public ICommand ClearOutputBoxCommand { get; private set; }

        public ICommand HelpCommand { get; private set; }

        public MainWindowViewModel()
        {
            // Initialize variable and property.            
            _helper = new AutomationAPIHelper();
            InputString = "";
            OutputString = "";
            GetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetCamera));
            SetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetCamera));
            SetFlightCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetFlight));
            AddLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.AddLayer));
            GetLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetLayer));
            RemoveLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.RemoveLayer));
            ClearLayersCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearLayers));
            GetWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetWorkspace));
            SetWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetWorkspace));
            ClearWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearWorkspace));
            GetSnapshotCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetSnapshot));
            ClearInputBoxCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearInputputBox));
            ClearOutputBoxCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearOutputBox));
            HelpCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.Help));
        }

        private void ExecuteFuction(FunctionType functionType)
        {
            switch (functionType)
            {
                // More about input string syntax, please refer to "examples.txt".

                case FunctionType.SetCamera:
                    {
                        OutputString = _helper.SetCamera(InputString);
                        break;
                    }
                case FunctionType.GetCamera:
                    {
                        OutputString = _helper.GetCamera();
                        break;
                    }
                case FunctionType.SetFlight:
                    {
                        OutputString = _helper.SetFlight(InputString);
                        break;
                    }
                case FunctionType.AddLayer:
                    {
                        OutputString = _helper.AddLayer(InputString);
                        break;
                    }
                case FunctionType.GetLayer:
                    {

                        OutputString = _helper.GetLayer(InputString);
                        break;
                    }
                case FunctionType.RemoveLayer:
                    {
                        OutputString = _helper.RemoveLayer(InputString);
                        break;
                    }
                case FunctionType.ClearLayers:
                    {
                        OutputString = _helper.ClearLayers(InputString);
                        break;
                    }
                case FunctionType.GetWorkspace:
                    {
                        OutputString = _helper.GetWorkspace();
                        break;
                    }
                case FunctionType.SetWorkspace:
                    {
                        OutputString = _helper.SetWorkspace(InputString);
                        break;
                    }
                case FunctionType.ClearWorkspace:
                    {
                        OutputString = _helper.ClearWorkspace();
                        break;
                    }
                case FunctionType.GetSnapshot:
                    {
                        OutputString = _helper.GetSnapshot(InputString);
                        break;
                    }

                case FunctionType.ClearInputputBox:
                    {
                        InputString = "";
                        break;
                    }
                case FunctionType.ClearOutputBox:
                    {
                        OutputString = "";
                        break;
                    }
                case FunctionType.Help:
                    {
                        OutputString = Properties.Resources.examples;
                        break;
                    }
            }
        }
    }
}
