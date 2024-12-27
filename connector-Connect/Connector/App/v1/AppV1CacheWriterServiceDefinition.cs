namespace Connector.App.v1;
using Connector.App.v1.Files;
using Connector.App.v1.Projects;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using Xchange.Connector.SDK.Abstraction.Change;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Hosting.Configuration;

/// <summary>
/// Service definition for the App V1 Cache Writer.
/// </summary>
public class AppV1CacheWriterServiceDefinition : BaseCacheWriterServiceDefinition<AppV1CacheWriterConfig>
{
    public override string ModuleId => "app-1";
    public override Type ServiceType => typeof(GenericCacheWriterService<AppV1CacheWriterConfig>);

    /// <summary>
    /// Configures dependencies for the service.
    /// </summary>
    /// <param name = "serviceCollection">The service collection to configure.</param>
    /// <param name = "serviceConfigJson">The configuration as a JSON string.</param>
    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var serviceConfig = JsonSerializer.Deserialize<AppV1CacheWriterConfig>(serviceConfigJson);
        if (serviceConfig == null)
        {
            throw new InvalidOperationException("Failed to deserialize the service configuration.");
        }

        serviceCollection.AddSingleton(serviceConfig);
        serviceCollection.AddSingleton<GenericCacheWriterService<AppV1CacheWriterConfig>>();
        serviceCollection.AddSingleton<ICacheWriterServiceDefinition<AppV1CacheWriterConfig>>(this);
        // Register Data Readers
        serviceCollection.AddSingleton<ProjectsDataReader>();
        serviceCollection.AddSingleton<FilesDataReader>();
    }

    /// <summary>
    /// Configures the Change Detector Provider.
    /// </summary>
    /// <param name = "factory">The change detector factory.</param>
    /// <param name = "connectorDefinition">The connector definition.</param>
    /// <returns>The configured Change Detector Provider.</returns>
    public override IDataObjectChangeDetectorProvider ConfigureChangeDetectorProvider(IChangeDetectorFactory factory, ConnectorDefinition connectorDefinition)
    {
        var options = factory.CreateProviderOptionsWithNoDefaultResolver();
        // Configure Data Object Keys for Projects
        RegisterKeysForObject<ProjectsDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<FilesDataObject>(options, connectorDefinition);
        return factory.CreateProvider(options);
    }

    private void RegisterKeysForObject<T>(IChangeDetectorResolverOptions options, ConnectorDefinition connectorDefinition)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Configures the Cache Writer Service.
    /// </summary>
    /// <param name = "service">The Cache Writer Service instance.</param>
    /// <param name = "config">The service configuration.</param>
    public override void ConfigureService(ICacheWriterService service, AppV1CacheWriterConfig config)
    {
        var dataReaderSettings = new DataReaderSettings
        {
            DisableDeletes = false,
            UseChangeDetection = true
        };
        // Register the Projects Data Reader
        service.RegisterDataReader<ProjectsDataReader, ProjectsDataObject>(ModuleId, config.ProjectsConfig, dataReaderSettings);
        service.RegisterDataReader<FilesDataReader, FilesDataObject>(ModuleId, config.FilesConfig, dataReaderSettings);
    }
}