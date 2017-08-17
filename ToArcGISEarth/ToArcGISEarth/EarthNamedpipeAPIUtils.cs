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

using ArcGISEarth.WCFNamedPipeIPC;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace EarthAPITest
{
    public class EarthNamedpipeAPIUtils : IEarthNamedpipeCallbackService
    {
        public void Notify(string message)
        {
        }

        public const string cBasePipeAddress = "net.pipe://localhost/arcgisearth";
		public const int cMaxBuffer = 2147483647;

		private IEarthNamedpipeService _channel = null;
		private ChannelFactory<IEarthNamedpipeService> _factory = null;

		public void CloseConnect()
		{
			if(_factory != null)
			{
				_factory.Close();
				_factory = null;
			}
			_channel = null;
		}

		public IEarthNamedpipeService CreateChannel(string address)
		{
			try
			{
				NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
				binding.MaxBufferPoolSize = cMaxBuffer;
				binding.MaxBufferSize = cMaxBuffer;
				binding.MaxReceivedMessageSize = cMaxBuffer;
				binding.ReceiveTimeout = TimeSpan.MaxValue;

				ServiceEndpoint se = new ServiceEndpoint(
					ContractDescription.GetContract(typeof(IEarthNamedpipeService)),
					binding,
					new EndpointAddress(address));

				_factory = new DuplexChannelFactory<IEarthNamedpipeService>(new InstanceContext(this), se);
				IEarthNamedpipeService channel = _factory.CreateChannel();
				return channel;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public bool Connect()
		{
			try
			{
				_channel = CreateChannel(cBasePipeAddress);
                  // call a function to test consistency of contract file.
				string test = _channel.GetCameraJson();

				if (_channel != null)
				{
					return true;
				}
				else
				{
					_channel = null;
					return false;
				}

			}
			catch
			{
				return false;
			}
		}

		public bool SetCamera(string json)
		{
			if (_channel == null)
			{
				return false;
			}

			try
			{
				if (_channel.SetCamera(json))
				{
					return true;
				}
                else
                {
                    return false;
                }

            }
			catch
			{
                return false;
			}
		}
	}
}
