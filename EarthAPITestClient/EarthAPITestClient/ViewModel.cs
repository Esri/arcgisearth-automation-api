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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EarthAPIUtils;

namespace EarthAPITestClient
{
    public enum FunctionType
    {
        GetCamera,
        SetCamera,
        FlyTo,
        AddLayer,
        ClearLayers,
        GetSnapshot,
        Connect,
        ClearInputputBox,
        ClearOutputBox,
        CloseConnect,
        Help,
        GetLayerLoadStatus,
        GetWorkspace,
        ImportWorkspace,
        Removelayer
    }
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;      
        private EarthNamedpipeAPIUtils _utils = null;

        public ViewModel()
        {
            this.InputString = "";
            this.OutputString = "";
        }
        
        private string _inputString ;
        public string InputString
        {
            get { return _inputString; }
            set
            {
                _inputString = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputString"));
                }
            }
        }

        private string _outputString;
        public string OutputString
        {
            get { return _outputString; }
            set
            {
                _outputString = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputString"));
                }
            }
        }

        private void GetMessageFromArcGISEarth(Object sender, EventArgs e)
        {
            if (e is MessageStringEventArgs)
            {
                this.OutputString = (e as MessageStringEventArgs).Message;
            }
        }

        public async void ExecuteFuction(FunctionType functionType)
        {
            if (_utils == null)
            {
                _utils = new EarthNamedpipeAPIUtils();
                _utils.OnNotify += GetMessageFromArcGISEarth;
            }

            switch (functionType)
            {
                case FunctionType.AddLayer:
                    {
                        // InputString is Json description of layer
                        OutputString = _utils.AddLayer(InputString);
                        break;
                    }
                case FunctionType.ClearLayers:
                    {
                        // InputString is Json description of target
                        OutputString = _utils.ClearLayers(InputString);
                        break;
                    }
                case FunctionType.Connect:
                    {
                        // InputString is path of arcgis earth
                        OutputString = await _utils.Connect(InputString);
                        break;
                    }
                case FunctionType.FlyTo:
                    {
                        OutputString = _utils.FlyTo(InputString);
                        break;
                    }
                case FunctionType.GetCamera:
                    {
                        OutputString = _utils.GetCamera();
                        break;
                    }
                case FunctionType.GetSnapshot:
                    {
                        OutputString = await _utils.GetSnapshot(InputString);
                        break;
                    }
                case FunctionType.SetCamera:
                    {
                        OutputString = _utils.SetCamera(InputString);
                        break;
                    }
                case FunctionType.Help:
                    {
                        //OutputString = File.ReadAllText("C:\\Projects\\arcgisearth-namedpipe-client\\EarthAPITest\\EarthAPITest\\examples.txt");
                        OutputString = "Example of parameters:\r\n\r\nConnect (Note, it is not belong to API)\r\nC:\\Projects\\earth\\output\\earth_windesktop_release\\bin\\ArcGISEarth.exe\r\n\r\nSetCamera\r\n{  \r\n   \"mapPoint\":{  \r\n      \"x\":-97.283978521275117,\r\n      \"y\":48.422233665100165,\r\n      \"z\":11000000,\r\n      \"spatialReference\":{  \r\n         \"wkid\":4326\r\n      }\r\n   },\r\n   \"heading\":0.0,\r\n   \"pitch\":0.10000000000019954\r\n}\r\n\r\nFlyTo\r\n{  \r\n   \"camera\":{  \r\n      \"mapPoint\":{  \r\n         \"x\":-92,\r\n         \"y\":41,\r\n         \"z\":11000000,\r\n         \"spatialReference\":{  \r\n            \"wkid\":4326\r\n         }\r\n      },\r\n      \"heading\":0.0,\r\n      \"pitch\":0.099999999996554886\r\n   },\r\n   \"duration\":2\r\n}\r\n\r\nAddLayer\r\n{  \r\n   \"type\":\"MapService\",\r\n   \"URI\":\"https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer\",\r\n   \"target\":\"OperationalLayers\"\r\n}\r\n\r\nClearLayers\r\n{  \r\n   \"target\":\"ElevationLayers\"\r\n}\r\n\r\nGetSnapshot (Note, the input parameters is Bitmap instead of string in API)\r\nD:/earth.jpg";
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
                case FunctionType.CloseConnect:
                    {
                        _utils.CloseConnect();
                        break;
                    }             
                case FunctionType.GetLayerLoadStatus:
                    {
                        OutputString = _utils.GetLayerLoadStatus(InputString);
                        break;
                    }
                case FunctionType.GetWorkspace:
                    {
                        OutputString = _utils.GetWorkspace();
                        break;
                    }
                case FunctionType.ImportWorkspace:
                    {
                        OutputString = _utils.ImportWorkspace(InputString);
                        break;
                    }
                case FunctionType.Removelayer:
                    {
                        OutputString = _utils.RemoveLayer(InputString);
                        break;
                    }
            }
        }

    }
}
