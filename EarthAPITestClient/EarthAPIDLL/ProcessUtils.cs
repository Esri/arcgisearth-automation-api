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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthAPIUtils
{
    public delegate bool ProcessStdOutCallBack(string msg);

    public class ProcessUtils
    {
        private string _processName = "ArcGISEarth";
        private string _exePath = null;
        private Process _proc;
        private TaskCompletionSource<bool> _startup;

        public ProcessUtils(string processName, string exePath)
        {
            _processName = processName;
            _exePath = exePath;
        }

        public bool IsRunning()
        {
            bool isRunning = false;
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == _processName)
                {
                    isRunning = true;
                    break;
                }
            }
            return isRunning;
        }

        private Task Startup()
        {
            _startup = new TaskCompletionSource<bool>();
            return _startup.Task;
        }

        private void HandleMessageFromTarget(object sender, DataReceivedEventArgs e, ProcessStdOutCallBack customCallback)
        {
            string message = e.Data;
            if (!String.IsNullOrEmpty(message) && customCallback(message))
            {
                _startup.SetResult(true); 
            }
        }

        public async Task Start(ProcessStdOutCallBack callback)
        {
            _proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _exePath,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                },
            };

            _proc.OutputDataReceived += (sender, e) =>
            {
                HandleMessageFromTarget(sender, e, callback);
            };

            _proc.Start();
            _proc.BeginOutputReadLine();

            await Startup();
        }
    }
}
