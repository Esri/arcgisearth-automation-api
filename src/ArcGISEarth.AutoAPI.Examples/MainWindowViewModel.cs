// Copyright 2023 Esri
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
using System.Text.Json;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        public ICommand AddGraphicCommand { get; private set; }

        public ICommand GetGraphicCommand { get; private set; }

        public ICommand UpdateGraphicCommand { get; private set; }

        public ICommand RemoveGraphicCommand { get; private set; }

        public ICommand ClearGraphicsCommand { get; private set; }

        public ICommand AddDrawingCommand { get; private set; }

        public ICommand RemoveDrawingCommand { get; private set; }

        public ICommand ClearDrawingsCommand { get; private set; }

        public ICommand NewMovieProjectCommand { get; private set; }

        public ICommand NewGlobalSceneProjectCommand { get; private set; }

        public ICommand NewLocalSceneProjectCommand { get; private set; }

        public ICommand OpenProjectCommand { get; private set; }

        public ICommand SaveProjectCommand { get; private set; }

        public ICommand SaveProjectAsCommand { get; private set; }

        public ICommand GetRecentProjectsCommand { get; private set; }

        public ICommand WatchCameraCommand { get; private set; }

        public ICommand WatchViewTapCommand { get; private set; }

        public ICommand StopWatchCommand { get; private set; }

        public ICommand TakeSnapshotCommand { get; private set; }

        public ICommand ClearInputBoxCommand { get; private set; }

        public ICommand SendButtonCommand { get; private set; }


        public MainWindowViewModel()
        {
            APIUrl = AutomationAPIHelper.APIBaseUrl;
            InputString = string.Empty;
            OutputString = string.Empty;
            GetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetCamera));
            SetCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetCamera));
            SetFlightCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SetFlight));
            AddLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.AddLayer));
            GetLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetLayer));
            RemoveLayerCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.RemoveLayer));
            ClearLayersCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearLayers));
            AddGraphicCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.AddGraphic));
            GetGraphicCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetGraphic));
            UpdateGraphicCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.UpdateGraphic));
            RemoveGraphicCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.RemoveGraphic));
            ClearGraphicsCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearGraphics));
            AddDrawingCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.AddDrawing));
            RemoveDrawingCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.RemoveDrawing));
            ClearDrawingsCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.ClearDrawings));
            NewMovieProjectCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.NewMovieProject));
            NewGlobalSceneProjectCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.NewGlobalSceneProject));
            NewLocalSceneProjectCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.NewLocalSceneProject));
            OpenProjectCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.OpenProject));
            SaveProjectCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SaveProject));
            SaveProjectAsCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.SaveProjectAs));
            GetRecentProjectsCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.GetRecentProjects));
            WatchCameraCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.WatchCamera));
            WatchViewTapCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.WatchViewTap));
            StopWatchCommand = new FunctionTypeCommand(e => ExecuteFuction(FunctionType.StopWatch));
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
                if (jsonString.StartsWith("{") || jsonString.StartsWith("["))
                {
                    var obj = JsonSerializer.Deserialize(jsonString, typeof(object));
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    return JsonSerializer.Serialize(obj, options);
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
        private const string ADDGRAPHIC_EXAMPLE = "{ \"geometry\": {\"type\": \"point\", \"x\": -100, \"y\": 40 }, \"symbol\": { \"type\": \"picture-marker\", \"url\": \"https://static.arcgis.com/images/Symbols/Shapes/BlackStarLargeB.png\", \"width\": \"64px\", \"height\": \"64px\", \"xoffset\": \"10px\", \"yoffset\": \"10px\"}}";
        private const string GETGRAPHIC_EXAMPLE = "311b7317-94f8-4f80-89f2-0e3ca5e77d28";
        private const string UPDATEGRAPHIC_EXAMPLE = "{\"id\": \"311b7317-94f8-4f80-89f2-0e3ca5e77d28\", \"geometry\": {\"type\": \"point\", \"x\": -100, \"y\": 40 }, \"symbol\": { \"type\": \"picture-marker\", \"url\": \"https://static.arcgis.com/images/Symbols/Shapes/BlackStarLargeB.png\", \"width\": \"64px\", \"height\": \"64px\", \"xoffset\": \"10px\", \"yoffset\": \"10px\"}}";
        private const string REMOVEGRAPHIC_EXAMPLE = "311b7317-94f8-4f80-89f2-0e3ca5e77d28";
        private const string ADDDRAWING_EXAMPLE = "{\"id\":\"8a1701c9-b8e1-1b0a-c1a7-ac6242c7645e\",\"visible\":true,\"title\":\"Point\",\"geometry\":{\"x\":-100,\"y\":40,\"spatialReference\":{\"wkid\":4326}},\"symbol\":{\"type\":\"picture-marker\",\"url\":\"https://static.arcgis.com/images/Symbols/Shapes/BlackStarLargeB.png\",\"size\":\"32px\"},\"labelSymbol\":{\"type\":\"text\",\"color\":[100,100,100,255],\"font\":{\"size\":\"16px\"}}}";
        private const string REMOVEDRAWING_EXAMPLE = "8a1701c9-b8e1-1b0a-c1a7-ac6242c7645e";
        private const string OPENPROJECT_EXAMPLE = @"{ ""path"": ""C:\\Users\\Username\\Documents\\ArcGISEarth\\Projects\\Global Scene.aescx"" }";
        private const string SAVEASPROJECT_EXAMPLE = @"{ ""path"": ""C:\\Users\\Username\\Documents\\ArcGISEarth\\Projects\\Global Scene.aescx"" }";
        private const string TAKESNAPSHOT_EXAMPLE = @"D:\ArcGISEarth.png";
        private CancellationTokenSource _notificationCancellationTokenSource;

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
                case FunctionType.AddGraphic:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.AddGraphic;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(ADDGRAPHIC_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetGraphic:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.GetGraphic;
                        InputPlaceholderString = "Example:\n\n" + GETGRAPHIC_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.UpdateGraphic:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.UpdateGraphic;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(UPDATEGRAPHIC_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.RemoveGraphic:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.RemoveGraphic;
                        InputPlaceholderString = "Example:\n\n" + REMOVEGRAPHIC_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearGraphics:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.ClearGraphics;
                        OutputString = "";
                        break;
                    }
                case FunctionType.AddDrawing:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.AddDrawing;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(ADDDRAWING_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.RemoveDrawing:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.RemoveDrawing;
                        InputPlaceholderString = "Example:\n\n" + REMOVEDRAWING_EXAMPLE;
                        OutputString = "";
                        break;
                    }
                case FunctionType.ClearDrawings:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.ClearDrawings;
                        OutputString = "";
                        break;
                    }
                case FunctionType.NewMovieProject:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.NewMovieProject;
                        OutputString = "";
                        break;
                    }
                case FunctionType.NewGlobalSceneProject:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.NewGlobalSceneProject;
                        OutputString = "";
                        break;
                    }
                case FunctionType.NewLocalSceneProject:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.NewLocalSceneProject;
                        OutputString = "";
                        break;
                    }
                case FunctionType.OpenProject:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.OpenProject;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(OPENPROJECT_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.SaveProject:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.SaveProject;
                        OutputString = "";
                        break;
                    }
                case FunctionType.SaveProjectAs:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.SaveProjectAs;
                        InputPlaceholderString = "Example:\n\n" + PrettyJson(SAVEASPROJECT_EXAMPLE);
                        OutputString = "";
                        break;
                    }
                case FunctionType.GetRecentProjects:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.GetRecentProjects;
                        OutputString = "";
                        break;
                    }
                case FunctionType.WatchCamera:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.WatchCamera;
                        OutputString = "";
                        break;
                    }
                case FunctionType.WatchViewTap:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.WatchViewTap;
                        OutputString = "";
                        break;
                    }
                case FunctionType.StopWatch:
                    {
                        InputString = "";
                        SendButtontype = FunctionType.StopWatch;
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
                            string outputString = await SendMessage(SendButtontype, InputString);
                            if (string.IsNullOrEmpty(outputString))
                            {
                                outputString = "Finished";
                            }
                            OutputString = PrettyJson(outputString);
                        }
                        else
                        {
                            OutputImage = await TakeSnapshotSend(SendButtontype);
                        }
                        break;
                    }
            }
        }

        private async Task<string> SendMessage(FunctionType sendType, string inputStr)
        {
            string outputStr = null;
            switch (sendType)
            {
                case FunctionType.GetCamera:
                    outputStr = await AutomationAPIHelper.GetCamera();
                    break;
                case FunctionType.SetCamera:
                    outputStr = await AutomationAPIHelper.SetCamera(inputStr);
                    break;
                case FunctionType.SetFlight:
                    outputStr = await AutomationAPIHelper.SetFlight(inputStr);
                    break;
                case FunctionType.AddLayer:
                    outputStr = await AutomationAPIHelper.AddLayer(inputStr);
                    break;
                case FunctionType.GetLayer:
                    outputStr = await AutomationAPIHelper.GetLayer(inputStr);
                    break;
                case FunctionType.RemoveLayer:
                    outputStr = await AutomationAPIHelper.RemoveLayer(inputStr);
                    break;
                case FunctionType.ClearLayers:
                    outputStr = await AutomationAPIHelper.ClearLayers(inputStr);
                    break;
                case FunctionType.AddGraphic:
                    outputStr = await AutomationAPIHelper.AddGraphic(inputStr);
                    break;
                case FunctionType.GetGraphic:
                    outputStr = await AutomationAPIHelper.GetGraphic(inputStr);
                    break;
                case FunctionType.UpdateGraphic:
                    outputStr = await AutomationAPIHelper.UpdateGraphic(inputStr);
                    break;
                case FunctionType.RemoveGraphic:
                    outputStr = await AutomationAPIHelper.RemoveGraphic(inputStr);
                    break;
                case FunctionType.ClearGraphics:
                    outputStr = await AutomationAPIHelper.ClearGraphics();
                    break;
                case FunctionType.AddDrawing:
                    outputStr = await AutomationAPIHelper.AddDrawing(inputStr);
                    break;
                case FunctionType.RemoveDrawing:
                    outputStr = await AutomationAPIHelper.RemoveDrawing(inputStr);
                    break;
                case FunctionType.ClearDrawings:
                    outputStr = await AutomationAPIHelper.ClearDrawings();
                    break;
                case FunctionType.NewMovieProject:
                    outputStr = await AutomationAPIHelper.NewProject("movie");
                    break;
                case FunctionType.NewGlobalSceneProject:
                    outputStr = await AutomationAPIHelper.NewProject("globalScene");
                    break;
                case FunctionType.NewLocalSceneProject:
                    outputStr = await AutomationAPIHelper.NewProject("localScene");
                    break;
                case FunctionType.OpenProject:
                    outputStr = await AutomationAPIHelper.OpenProject(inputStr);
                    break;
                case FunctionType.SaveProject:
                    outputStr = await AutomationAPIHelper.SaveProject();
                    break;
                case FunctionType.SaveProjectAs:
                    outputStr = await AutomationAPIHelper.SaveAsProject(inputStr);
                    break;
                case FunctionType.GetRecentProjects:
                    outputStr = await AutomationAPIHelper.GetRecentProjects();
                    break;
                case FunctionType.WatchCamera:
                    outputStr = StartWatchingNotifications("cameraChange", "camera changes");
                    break;
                case FunctionType.WatchViewTap:
                    outputStr = StartWatchingNotifications("viewTap", "view taps");
                    break;
                case FunctionType.StopWatch:
                    outputStr = StopWatchingNotifications();
                    break;
            }

            return outputStr;
        }

        private string StartWatchingNotifications(string eventName, string displayName)
        {
            CancelWatchingNotifications();
            _notificationCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _notificationCancellationTokenSource.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await AutomationAPIHelper.WatchNotifications(async (receivedEventName, data) =>
                    {
                        if (receivedEventName == eventName)
                        {
                            AppendOutput($"{DateTime.Now:HH:mm:ss} {displayName}\n{PrettyJson(data)}\n\n");
                        }

                        await Task.CompletedTask;
                    }, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    AppendOutput($"Notification listener error: {ex.Message}\n");
                }
            }, cancellationToken);

            return $"Listening for {displayName} notifications...";
        }

        private string StopWatchingNotifications()
        {
            if (_notificationCancellationTokenSource == null)
            {
                return "No notification listener is running.";
            }

            CancelWatchingNotifications();
            return "Stopped notification listener.";
        }

        private void CancelWatchingNotifications()
        {
            if (_notificationCancellationTokenSource == null)
            {
                return;
            }

            _notificationCancellationTokenSource.Cancel();
            _notificationCancellationTokenSource.Dispose();
            _notificationCancellationTokenSource = null;
        }

        private void AppendOutput(string message)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => OutputString += message);
                return;
            }

            OutputString += message;
        }

        private async Task<ImageSource> TakeSnapshotSend(FunctionType sendType)
        {
            ImageSource outputImg = null;
            if (sendType == FunctionType.TakeSnapshot)
            {
                outputImg = await AutomationAPIHelper.TakeSnapshot();
            }
            return outputImg;
        }
    }
}