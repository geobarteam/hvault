using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VaultWorker
{
    public static class Extensions
    {
        static string _output;
        static System.Diagnostics.Process _process;

        public static string ExecuteAtCommandLine(this string command)
        {
            _output = "";
            _process = new System.Diagnostics.Process();
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
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            _process.WaitForExit();
            return _output;
        }

        private static void process_Exited(object sender, EventArgs e)
        {
            _output += string.Format("process exited with code {0}\n", _process.ExitCode.ToString());
        }

        private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _output += e.Data + "\n";
        }

        private static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _output += e.Data + "\n";
        }
    }
}
