using System;
using ESR.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xchange.Connector.SDK.Hosting;
using Xchange.Connector.SDK.Test.Local;
using Microsoft.Extensions.Configuration;

namespace Connector;

/// <summary>
/// Main executable entry point for the connector.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // Check for required environment variables
        var requiredVariables = new[] { "ESR__QueueName", "CONNECTOR_API_KEY", "CONNECTOR_REGION" };
        foreach (var variable in requiredVariables)
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(variable)))
            {
                Console.Error.WriteLine($"Exiting: '{variable}' environment variable not provided");
                Environment.Exit(1);
            }
        }

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information); // Adjust level as needed
            })
            .ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "DOTNET_");
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;
                config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLocalDevelopment(args)
            .UseGenericServiceRun<ServiceRunner>(new ConnectorRegistration(), args)
            .Build();

        host.Run();
    }
}
