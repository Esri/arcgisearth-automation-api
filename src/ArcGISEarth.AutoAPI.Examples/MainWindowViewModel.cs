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

using ArcGISEarth.AutoAPI.Utils;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        ImportWorkspace,
        ClearWorkspace,
        TakeSnapshot,
        ClearInputputBox,
        Send
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

        private string _apiUrl;

        public string APIUrl
        {
            get { return _apiUrl; }
            set
            {
                if (_apiUrl != value)
                {
                    _apiUrl = value;
                    if (_helper != null)
                    {
                        _helper.APIUrl = value;
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(APIUrl)));
                }
            }
        }

        private string _inputPlaceholderString;

        public string InputPlaceholderString
        {
            get { return _inputPlaceholderString; }
            set
            {
                if (_inputPlaceholderString != value)
                {
                    _inputPlaceholderString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputPlaceholderString)));
                }
            }
        }

        private string _inputString;

        public string InputString
        {
            get { return _inputString; }
            set
            {
                if (_inputString != value)
                {
                    _inputString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputString)));
                }
            }
        }

        private string _outputString;

        public string OutputString
        {
            get { return _outputString; }
            set
            {
                if (_outputString != value)
                {
                    _outputString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputString)));
                }
            }
        }

        public ICommand GetCameraCommand { get; private set; }

        public ICommand SetCameraCommand { get; private set; }

        public ICommand SetFlightCommand { get; private set; }

        public ICommand AddLayerCommand { get; private set; }

        public ICommand GetLayerCommand { get; private set; }

        public ICommand RemoveLayerCommand { get; private set; }

        public ICommand ClearLayersCommand { get; private set; }

        public ICommand GetWorkspaceCommand { get; private set; }

        public ICommand ImportWorkspaceCommand { get; private set; }

        public ICommand ClearWorkspaceCommand { get; private set; }

        public ICommand TakeSnapshotCommand { get; private set; }

        public ICommand ClearInputBoxCommand { get; private set; }

        public ICommand SendButtonCommand { get; private set; }

        private const string DEFAULT_API_URL = "http://localhost:8000/arcgisearth";


        public MainWindowViewModel()
        {
            // Initialize variable and property.            
            _helper = new AutomationAPIHelper();
            APIUrl = DEFAULT_API_URL;
            InputString = string.Empty;
            OutputString = string.Empty;
            GetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetCamera));
            SetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetCamera));
            SetFlightCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetFlight));
            AddLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.AddLayer));
            GetLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetLayer));
            RemoveLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.RemoveLayer));
            ClearLayersCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearLayers));
            GetWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetWorkspace));
            ImportWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ImportWorkspace));
            ClearWorkspaceCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearWorkspace));
            TakeSnapshotCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.TakeSnapshot));
            ClearInputBoxCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearInputputBox));
            SendButtonCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.Send));
        }

        public enum SendButtonType
        {
            GetCamera,
            SetCamera,
            SetFlight,
            AddLayer,
            GetLayer,
            RemoveLayer,
            ClearLayers,
            GetWorkspace,
            ImportWorkspace,
            ClearWorkspace,
            TakeSnapshot
        }

        private SendButtonType _sendButtonType;
        public SendButtonType SendButtontype 
        {
            get { return _sendButtonType; }
            set 
            {
                if (_sendButtonType != value)
                {
                    _sendButtonType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SendButtontype)));
                }
            }
        }

        private async void ExecuteFuction(FunctionType functionType)
        {            
            switch (functionType)
            {
                // More about input string syntax, please refer to "examples.txt".
                case FunctionType.GetCamera:
                    {
                        SendButtontype = SendButtonType.GetCamera;
                        InputPlaceholderString = "Testing InputPlaceholderString GetCamera";
                        OutputString = "";
                        break;
                    }
                case FunctionType.SetCamera:
                    {
                        SendButtontype = SendButtonType.SetCamera;
                        InputPlaceholderString = "Testing InputPlaceholderString SetCamera";
                        OutputString = "";
                        break;
                    }
                case FunctionType.SetFlight:
                    {
                        SendButtontype = SendButtonType.SetFlight;
                        OutputString = "";
                        break;
                    }
                case FunctionType.AddLayer:
                    {
                        SendButtontype = SendButtonType.AddLayer;
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetLayer:
                    {
                        SendButtontype = SendButtonType.GetLayer;
                        OutputString = "";
                        break;
                    }
                case FunctionType.RemoveLayer:
                    {
                        SendButtontype = SendButtonType.RemoveLayer;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearLayers:
                    {
                        SendButtontype = SendButtonType.ClearLayers;
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetWorkspace:
                    {
                        SendButtontype = SendButtonType.GetWorkspace;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ImportWorkspace:
                    {
                        SendButtontype = SendButtonType.ImportWorkspace;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearWorkspace:
                    {
                        SendButtontype = SendButtonType.ClearWorkspace;
                        OutputString = "";
                        break;
                    }
                case FunctionType.TakeSnapshot:
                    {
                        SendButtontype = SendButtonType.TakeSnapshot;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearInputputBox:
                    {
                        InputString = "";
                        break;
                    }
                case FunctionType.Send:
                    {
                        OutputString = await SendMessage(_helper, SendButtontype, InputString);
                        break;
                    }
            }            
        }

        private async Task<string> SendMessage(AutomationAPIHelper helper, SendButtonType sendType, string inputStr)
        {
            string outputStr = null;
            switch (sendType)
            {
                case SendButtonType.GetCamera:
                    outputStr = await helper.GetCamera();
                    break;
                case SendButtonType.SetCamera:
                    outputStr = await helper.SetCamera(inputStr);
                    break;
                case SendButtonType.SetFlight:
                    outputStr = await helper.SetFlight(inputStr);
                    break;
                case SendButtonType.AddLayer:
                    outputStr = await helper.AddLayer(inputStr);
                    break;
                case SendButtonType.GetLayer:
                    outputStr = await helper.GetLayer(inputStr);
                    break;
                case SendButtonType.RemoveLayer:
                    outputStr = await helper.RemoveLayer(inputStr);
                    break;
                case SendButtonType.ClearLayers:
                    outputStr = await helper.ClearLayers(inputStr);
                    break;
                case SendButtonType.GetWorkspace:
                    outputStr = await helper.GetWorkspace();
                    break;
                case SendButtonType.ImportWorkspace:
                    outputStr = await helper.ImportWorkspace(inputStr);
                    break;
                case SendButtonType.ClearWorkspace:
                    outputStr = await helper.ClearWorkspace();
                    break;
                case SendButtonType.TakeSnapshot:
                    outputStr = await helper.TakeSnapshot(inputStr);
                    break;
            }
            return outputStr;
        }
    }
}
