using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Configuration;
using Orleans.Persistence.Morstead.Provider;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Storage
{
    /// <summary>
    /// Storage provider for writing grain state data to Morstead in JSON format.
    /// Adapted from Azure blob reference implementation which can be found on: 
    /// https://github.com/sjefvanleeuwen/orleans/blob/master/src/Azure/Orleans.Persistence.AzureStorage/Providers/Storage/AzureBlobStorage.cs
    /// </summary>
    public class MorsteadGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly string name;
        private readonly MorsteadStorageOptions options;
        private readonly SerializationManager serializationManager;
        private readonly IGrainFactory grainFactory;
        private readonly ITypeResolver typeResolver;
        private readonly ILogger<MorsteadGrainStorage> logger;
        private JsonSerializerSettings jsonSettings;

        public MorsteadGrainStorage(
        string name,
        MorsteadStorageOptions options,
        SerializationManager serializationManager,
        IGrainFactory grainFactory,
        ITypeResolver typeResolver,
        ILogger<MorsteadGrainStorage> logger)
        {
            this.name = name;
            this.options = options;
            this.serializationManager = serializationManager;
            this.grainFactory = grainFactory;
            this.typeResolver = typeResolver;
            this.logger = logger;
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<MorsteadGrainStorage>(this.name), this.options.InitStage, Init);
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        /// <summary> Initialization function for this storage provider. </summary>
        private async Task Init(CancellationToken ct)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                this.logger.LogInformation((int)MorsteadProviderErrorCode.MorsteadProvider_InitProvider, $"AzureTableGrainStorage initializing: {this.options.ToString()}");
                this.logger.LogInformation((int)MorsteadProviderErrorCode.MorsteadProvider_ParamConnectionString, "AzureTableGrainStorage is using DataConnectionString: {0}", ConfigUtilities.RedactConnectionStringInfo(this.options.ConnectionString));
                this.jsonSettings = OrleansJsonSerializer.UpdateSerializerSettings(OrleansJsonSerializer.GetDefaultSerializerSettings(this.typeResolver, this.grainFactory), this.options.UseFullAssemblyNames, this.options.IndentJson, this.options.TypeNameHandling);

                this.options.ConfigureJsonSerializerSettings?.Invoke(this.jsonSettings);

                // TODO: Morstead Specific Code here.
                /*
                var account = CloudStorageAccount.Parse(this.options.ConnectionString);
                var blobClient = account.CreateCloudBlobClient();
                container = blobClient.GetContainerReference(this.options.ContainerName);
                await container.CreateIfNotExistsAsync().ConfigureAwait(false);
                */
                stopWatch.Stop();
                this.logger.LogInformation((int)MorsteadProviderErrorCode.MorsteadProvider_InitProvider, $"Initializing provider {this.name} of type {this.GetType().Name} in stage {this.options.InitStage} took {stopWatch.ElapsedMilliseconds} Milliseconds.");
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                this.logger.LogError((int)ErrorCode.Provider_ErrorFromInit, $"Initialization failed for provider {this.name} of type {this.GetType().Name} in stage {this.options.InitStage} in {stopWatch.ElapsedMilliseconds} Milliseconds.", ex);
                throw;
            }
        }
    }
}
