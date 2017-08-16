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
