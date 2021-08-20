using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Fluxys.Framework.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Environment = System.Environment;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace VaultWorker
{
    public class Worker : BackgroundService
    {
        private string _vaultRootConfigPath = "./Hcl/";
        private readonly ILogger<Worker> _logger;
        private readonly IVaultStarterHandler _vaultStarterHandler;
        private readonly FluxysConfiguration _fluxysConfiguration;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IVaultStarterHandler vaultStarterHandler, FluxysConfiguration fluxysConfiguration, IConfiguration configuration)
        {
            _logger = logger;
            _vaultStarterHandler = vaultStarterHandler;
            _fluxysConfiguration = fluxysConfiguration;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{_fluxysConfiguration.Application.GetConfigurationItemName()} starting up.");
            //"/C $env:VAULT_ADDR=\"http://127.0.0.1:8200\"".ExecuteAtCommandLine(_logger);
            $"/C vault server -config {_vaultRootConfigPath + Environment.MachineName}.hcl -log-level=trace".ExecuteAtCommandLine(_logger);
            $"/C vault operator unseal {_configuration["Secrets:key1"]}".ExecuteAtCommandLine(_logger);
            $"/C vault operator unseal {_configuration["Secrets:key2"]}".ExecuteAtCommandLine(_logger);
            $"/C vault operator unseal {_configuration["Secrets:key3"]}".ExecuteAtCommandLine(_logger);
            $"/C vault status".ExecuteAtCommandLine(_logger);
        }
    }
}
