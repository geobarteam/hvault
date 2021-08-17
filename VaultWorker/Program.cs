using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxys.Framework.Bootstrap.Helpers;
using Fluxys.Framework.Bootstrap.HostBuilder;
using Fluxys.Framework.Bootstrap.Logging;
using Fluxys.Framework.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IHost = Microsoft.Extensions.Hosting.IHost;
using GenericHost = Microsoft.Extensions.Hosting.Host;

namespace VaultWorker
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var configuration = BootstrapConfiguration.ReadAppSettings();
                var fluxysConfig = configuration.GetFluxysConfiguration();
                var loggerFactory = BootstrapLoggerFactory.Create(fluxysConfig);
                var logger = loggerFactory.CreateLogger(typeof(Program));

                try
                {
                    logger.LogInformation("Starting hosts");

                    var hosts = new[]
                    {
                        CreateHostBuilder(fluxysConfig, loggerFactory, args)
                    };

                    await HostRunner.RunAsync(logger, hosts);

                    return 0;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Host terminated unexpectedly");
                    return 1;
                }
                finally
                {
                    // Delay so all logging is flushed. Log.CloseAndFlush will not work because we do not use the static logger.
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        public static IHost CreateHostBuilder(FluxysConfiguration fluxysConfiguration, ILoggerFactory loggerFactory, string[] args) 
         =>   GenericHost.CreateDefaultBuilder(args)
             .UseFluxysConfiguration(fluxysConfiguration)
             .UseCommonFluxysServices(loggerFactory)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(fluxysConfiguration);
                    services.AddTransient<IVaultStarterHandler, VaultStarterHandler>();
                    services.AddHostedService<Worker>();
                }).Build();
    }
}
