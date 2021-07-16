using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VaultWorker
{
    public class Worker : BackgroundService
    {
        private string _vaultRootConfigPath = "../Vms/vault/";
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ExecuteCommand($"/C vault server -config {_vaultRootConfigPath + Environment.MachineName}.hcl -log-level=trace");
            Console.ReadLine();
        }

        private static void ExecuteCommand(string command)
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
