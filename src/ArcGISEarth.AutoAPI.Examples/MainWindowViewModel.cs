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

using ArcGISEarth.AutoAPI.Examples.Converter;
using ArcGISEarth.AutoAPI.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ArcGISEarth.AutoAPI.Examples
{
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
                _apiUrl = value;
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

        private ImageSource _outputImage;

        public ImageSource OutputImage
        {
            get { return _outputImage; }
            set
            {
                if (_outputImage != value)
                {
                    _outputImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputImage)));
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


        public MainWindowViewModel()
        {
            // Initialize variable and property.            
            _helper = new AutomationAPIHelper();
            APIUrl = _helper.APIBaseUrl;
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

        private FunctionType _sendButtonType;
        public FunctionType SendButtontype 
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

        public string PrettyJson(string jsonString)
        {
            try
            {
                // Handle non-json format output.
                if (jsonString.StartsWith("{"))
                {
                    var obj = JsonConvert.DeserializeObject(jsonString);
                    return JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
                else
                {
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private const string CAMERA_EXAMPLE = "{\"position\":{\"x\":-92,\"y\":41,\"z\":11000000,\"spatialReference\":{\"wkid\":4326}},\"heading\":2.3335941892764884e-17,\"tilt\":6.144145559063083e-15,\"roll\":0}";
        private const string FLIGHT_EXAMPLE = "{\"camera\":{\"position\":{\"x\":-92,\"y\":41,\"z\":11000000,\"spatialReference\":{\"wkid\":4326}},\"heading\":2.3335941892764884e-17,\"tilt\":6.144145559063083e-15,\"roll\":0},\"duration\":2}";
        private const string ADDLAYER_EXAMPLE = @"{ ""URI"": ""https://www.arcgis.com/home/item.html?id=19dcff93eeb64f208d09d328656dd492"", ""target"": ""operationalLayers"", ""type"": ""PortalItem"" }";
        private const string GETLAYER_EXAMPLE = "311b7317-94f8-4f80-89f2-0e3ca5e77d28";
        private const string REMOVELAYER_EXAMPLE = "311b7317-94f8-4f80-89f2-0e3ca5e77d28";
        private const string REMOVELAYERS_EXAMPLE = "operationalLayers";
        private const string IMPORTWORKSPACE_EXAMPLE = @"{ ""url"": ""http://localhost:8000/workspaces/4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip"", ""path"": ""C:\\Users\\Username\\Documents\\ArcGISEarth\\automation\\workspaces\\4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip""}";
        private const string TAKESNAPSHOT_EXAMPLE = @"D:\ArcGISEarth.png";

        private async void ExecuteFuction(FunctionType functionType)
        {            
            switch (functionType)
            {
                // More about input string syntax, please refer to "examples.txt".
                case FunctionType.GetCamera:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.GetCamera;
                        OutputString = "";
                        break;
                    }
                case FunctionType.SetCamera:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.SetCamera;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(CAMERA_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.SetFlight:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.SetFlight;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(FLIGHT_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.AddLayer:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.AddLayer;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(ADDLAYER_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetLayer:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.GetLayer;
                        InputPlaceholderString = "Example:\n\n" + GETLAYER_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.RemoveLayer:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.RemoveLayer;
                        InputPlaceholderString = "Example:\n\n" + REMOVELAYER_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearLayers:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.ClearLayers;
                        InputPlaceholderString = "Example:\n\n" + REMOVELAYERS_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetWorkspace:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.GetWorkspace;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ImportWorkspace:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.ImportWorkspace;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(IMPORTWORKSPACE_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearWorkspace:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.ClearWorkspace;
                        OutputString = "";
                        break;
                    }
                case FunctionType.TakeSnapshot:
                    {
                        SendButtontype = FunctionType.TakeSnapshot;
                        InputPlaceholderString = "Example:\n\n" + TAKESNAPSHOT_EXAMPLE;
                        OutputString = "";
                        OutputImage = null;
                        break;
                    }
                case FunctionType.ClearInputputBox:
                    {
                        InputString = "";
                        break;
                    }
                case FunctionType.Send:
                    {
                        OutputString = "Waiting response...";
                        if (SendButtontype != FunctionType.TakeSnapshot)
                        {
                            string outputString = await SendMessage(_helper, SendButtontype, InputString);
                            OutputString = PrettyJson(outputString);
                        }
                        else
                        {
                            OutputImage = await TakeSnapshotSend(_helper, SendButtontype);
                        }
                        break;
                    }
            }            
        }

        private async Task<string> SendMessage(AutomationAPIHelper helper, FunctionType sendType, string inputStr)
        {
            string outputStr = null;
            switch (sendType)
            {
                case FunctionType.GetCamera:
                    outputStr = await helper.GetCamera();
                    break;
                case FunctionType.SetCamera:
                    outputStr = await helper.SetCamera(inputStr);
                    break;
                case FunctionType.SetFlight:
                    outputStr = await helper.SetFlight(inputStr);
                    break;
                case FunctionType.AddLayer:
                    outputStr = await helper.AddLayer(inputStr);
                    break;
                case FunctionType.GetLayer:
                    outputStr = await helper.GetLayer(inputStr);
                    break;
                case FunctionType.RemoveLayer:
                    outputStr = await helper.RemoveLayer(inputStr);
                    break;
                case FunctionType.ClearLayers:
                    outputStr = await helper.ClearLayers(inputStr);
                    break;
                case FunctionType.GetWorkspace:
                    outputStr = await helper.GetWorkspace();
                    break;
                case FunctionType.ImportWorkspace:
                    outputStr = await helper.ImportWorkspace(inputStr);
                    break;
                case FunctionType.ClearWorkspace:
                    outputStr = await helper.ClearWorkspace();
                    break;
                //case FunctionType.TakeSnapshot:
                //    outputStr = await helper.TakeSnapshot(inputStr);
                //    break;
            }

            return outputStr;
        }

        private async Task<ImageSource> TakeSnapshotSend(AutomationAPIHelper helper, FunctionType sendType)
        {
            ImageSource outputImg = null;
            if (sendType == FunctionType.TakeSnapshot)
            {
                outputImg = await helper.TakeSnapshot();
            }

            return outputImg;
        }
    }
}
