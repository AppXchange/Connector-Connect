namespace Connector.App.v1;
using Connector.App.v1.Files;
using Connector.App.v1.Files.CompleteUpload;
using Connector.App.v1.Files.UploadFile;
using Connector.App.v1.Folders;
using Connector.App.v1.Folders.Create;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Action;

public class AppV1ActionProcessorServiceDefinition : BaseActionHandlerServiceDefinition<AppV1ActionProcessorConfig>
{
    public override string ModuleId => "app-1";
    public override Type ServiceType => typeof(GenericActionHandlerService<AppV1ActionProcessorConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
        var serviceConfig = JsonSerializer.Deserialize<AppV1ActionProcessorConfig>(serviceConfigJson, options);
        serviceCollection.AddSingleton<AppV1ActionProcessorConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericActionHandlerService<AppV1ActionProcessorConfig>>();
        serviceCollection.AddSingleton<IActionHandlerServiceDefinition<AppV1ActionProcessorConfig>>(this);
        // Register Action Handlers as scoped dependencies
        serviceCollection.AddScoped<CreateFoldersHandler>();
        serviceCollection.AddScoped<UploadFileFilesHandler>();
        serviceCollection.AddScoped<CompleteUploadFilesHandler>();
    }

    public override void ConfigureService(IActionHandlerService service, AppV1ActionProcessorConfig config)
    {
        // Register Action Handler configurations for the Action Processor Service
        service.RegisterHandlerForDataObjectAction<CreateFoldersHandler, FoldersDataObject>(ModuleId, "folders", "create", config.CreateFoldersConfig);
        service.RegisterHandlerForDataObjectAction<UploadFileFilesHandler, FilesDataObject>(ModuleId, "files", "upload-file", config.UploadFileFilesConfig);
        service.RegisterHandlerForDataObjectAction<CompleteUploadFilesHandler, FilesDataObject>(ModuleId, "files", "complete-upload", config.CompleteUploadFilesConfig);
    }
}