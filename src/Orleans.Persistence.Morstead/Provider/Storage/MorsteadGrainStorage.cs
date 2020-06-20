using LiteDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Configuration;
using Orleans.Persistence.Morstead.Provider;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public static Dictionary<string, LiteDatabase> dbs = new Dictionary<string, LiteDatabase>();

        private readonly string name;
        private readonly MorsteadStorageOptions options;
        private readonly SerializationManager serializationManager;
        private readonly IGrainFactory grainFactory;
        private readonly ITypeResolver typeResolver;
        private readonly ILogger<MorsteadGrainStorage> logger;
        private JsonSerializerSettings jsonSettings;
        private ILiteCollection<MorsteadGrainStorageModel> grains;
        private LiteDatabase db;

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

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);
            if (this.logger.IsEnabled(LogLevel.Trace)) this.logger.Trace((int)MorsteadProviderErrorCode.MorsteadProvider_Storage_Reading, "Reading: GrainType={0} Grainid={1} ETag={2} from BlobName={3} in Container={4}", grainType, grainReference, grainState.ETag, blobName, name);
            
            var blob = grains.Query().Where(x => x.ETag == blobName).FirstOrDefault();
            if (blob == null)
                return;
            grainState.State = this.ConvertFromStorageFormat(blob.Contents);
            grainState.ETag = blob.ETag;
            return;
        }

        private object ConvertFromStorageFormat(byte[] contents)
        {
            object result;
            if (this.options.UseJson)
            {
                var str = Encoding.UTF8.GetString(contents);
                result = JsonConvert.DeserializeObject<object>(str, this.jsonSettings);
            }
            else
            {
                result = this.serializationManager.DeserializeFromByteArray<object>(contents);
            }

            return result;
        }
        private (byte[], string) ConvertToStorageFormat(object grainState)
        {
            byte[] data;
            string mimeType;
            if (this.options.UseJson)
            {
                data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(grainState, this.jsonSettings));
                mimeType = "application/json";
            }
            else
            {
                data = this.serializationManager.SerializeToByteArray(grainState);
                mimeType = "application/octet-stream";
            }

            return (data, mimeType);
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString());
        }
        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);
            var (contents, mimeType) = ConvertToStorageFormat(grainState.State);
            await Task.Run(()=>grains.Insert(new MorsteadGrainStorageModel() { ETag = blobName, Contents = contents }));
        }

        /// <summary> Initialization function for this storage provider. </summary>
        private async Task Init(CancellationToken ct)
        {
            var stopWatch = Stopwatch.StartNew();
            if (db != null)
                return;
            try
            {
                this.logger.LogInformation((int)MorsteadProviderErrorCode.MorsteadProvider_InitProvider, $"MoresteadGrainStorage initializing: {this.options.ToString()}");
                this.logger.LogInformation((int)MorsteadProviderErrorCode.MorsteadProvider_ParamConnectionString, "MoresteadGrainStorage is using Container Name {0}", this.options.ContainerName);
                this.jsonSettings = OrleansJsonSerializer.UpdateSerializerSettings(OrleansJsonSerializer.GetDefaultSerializerSettings(this.typeResolver, this.grainFactory), this.options.UseFullAssemblyNames, this.options.IndentJson, this.options.TypeNameHandling);

                this.options.ConfigureJsonSerializerSettings?.Invoke(this.jsonSettings);
                if (!dbs.ContainsKey(this.name))
                {
                    db = new LiteDatabase(this.name);
                    await Task.Run(() => grains = db.GetCollection<MorsteadGrainStorageModel>("grains"));
                    //grains.EnsureIndex(x => x.ETag);
                    dbs.Add(name, db);
                }
                else
                {
                    db = dbs[this.name];
                    await Task.Run(() => grains = db.GetCollection<MorsteadGrainStorageModel>("grains"));
                }
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
