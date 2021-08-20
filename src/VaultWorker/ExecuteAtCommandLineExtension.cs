using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace VaultWorker
{
    public static class ExecuteAtCommandLineExtension
    {
        static StringBuilder _output;
        static System.Diagnostics.Process _process;

        static ILogger _logger;

        public static string ExecuteAtCommandLine(this string command, ILogger logger)
        {
            _output = new StringBuilder();
            _process = new System.Diagnostics.Process();
            _logger = logger;

            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived);
            _process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_ErrorDataReceived);
            _process.Exited += new System.EventHandler(process_Exited);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            _process.StartInfo = startInfo;
            _logger.LogInformation(command);
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            _process.WaitForExit();
            return _output.ToString();
        }

        private static void process_Exited(object sender, EventArgs e)
        {
            var error =  string.Format("process exited with code {0}", _process.ExitCode.ToString());
            _logger.LogError(error);
            _output.Append(error + "\n");
        }

        private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var error = e.Data;
            _logger.LogError(error);
            _output.Append(error + "\n");
        }

        private static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var info = e.Data;
            _logger.LogInformation(info);
            _output.Append(info + "\n");
        }
    }
}
