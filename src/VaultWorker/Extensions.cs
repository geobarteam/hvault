using System;
using System.Collections.Generic;
using System.Text;

namespace VaultWorker
{
    public static class Extensions
    {
        public static void ExecuteAtCommandLine(this string command)
        {
            System.Diagnostics.Process vaultProcess = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            vaultProcess.StartInfo = startInfo;
            vaultProcess.Start();
        }
    }
}
