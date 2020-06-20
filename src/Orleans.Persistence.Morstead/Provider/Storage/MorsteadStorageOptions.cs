using Newtonsoft.Json;
using System;

namespace Orleans.Configuration
{
    /// <summary>
    /// Storage options for writing grain state data to Morstead in JSON format.
    /// Adapted from Azure blob reference implementation which can be found on: 
    /// https://github.com/sjefvanleeuwen/orleans/blob/master/src/Azure/Orleans.Persistence.AzureStorage/Providers/Storage/AzureBlobStorageOptions.cs
    /// </summary>
    public class MorsteadStorageOptions
    {
        /// <summary>
        /// Container name where grain stage is stored
        /// TODO: What is the equivalent in Morstead
        /// </summary>
        public string ContainerName { get; set; } = DEFAULT_CONTAINER_NAME;
        public const string DEFAULT_CONTAINER_NAME = "grainstate";

        /// <summary>
        /// Stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = DEFAULT_INIT_STAGE;
        public const int DEFAULT_INIT_STAGE = ServiceLifecycleStage.ApplicationServices;

        public bool UseJson { get; set; }
        public bool UseFullAssemblyNames { get; set; }
        public bool IndentJson { get; set; }
        public TypeNameHandling? TypeNameHandling { get; set; }
        public Action<JsonSerializerSettings> ConfigureJsonSerializerSettings { get; set; }
    }

    /// <summary>
    /// Configuration validator for AzureTableStorageOptions
    /// </summary>
    public class MorsteadStorageOptionsValidator : IConfigurationValidator
    {
        private readonly MorsteadStorageOptions options;
        private readonly string name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The option to be validated.</param>
        /// <param name="name">The option name to be validated.</param>
        public MorsteadStorageOptionsValidator(MorsteadStorageOptions options, string name)
        {
            this.options = options;
            this.name = name;
        }

        public void ValidateConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
