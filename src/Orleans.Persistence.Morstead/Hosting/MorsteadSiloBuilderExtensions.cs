using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.Hosting
{
    /// <summary>
    /// Configure silo to use Morstead storage as the default grain storage.
    /// Adapted from Azure blob reference implementation which can be found on: 
    /// https://github.com/sjefvanleeuwen/orleans/blob/master/src/Azure/Orleans.Persistence.AzureStorage/Hosting/AzureBlobSiloBuilderExtensions.cs
    /// </summary>
    public static class MorsteadSiloBuilderExtensions
    {
        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddMorsteadGrainStorageAsDefault(this ISiloHostBuilder builder, Action<MorsteadStorageOptions> configureOptions)
        {
            return builder.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddMorsteadGrainStorage(this ISiloHostBuilder builder, string name, Action<MorsteadStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddMorsteadGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddMorsteadGrainStorageAsDefault(this ISiloHostBuilder builder, Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            return builder.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddMorsteadGrainStorage(this ISiloHostBuilder builder, string name, Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddMorsteadGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static ISiloBuilder AddMorsteadGrainStorageAsDefault(this ISiloBuilder builder, Action<MorsteadStorageOptions> configureOptions)
        {
            return builder.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static ISiloBuilder AddMorsteadGrainStorage(this ISiloBuilder builder, string name, Action<MorsteadStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddMorsteadGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static ISiloBuilder AddMorsteadGrainStorageAsDefault(this ISiloBuilder builder, Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            return builder.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static ISiloBuilder AddMorsteadGrainStorage(this ISiloBuilder builder, string name, Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddMorsteadGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static IServiceCollection AddMorsteadGrainStorageAsDefault(this IServiceCollection services, Action<MorsteadStorageOptions> configureOptions)
        {
            return services.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static IServiceCollection AddMorsteadGrainStorage(this IServiceCollection services, string name, Action<MorsteadStorageOptions> configureOptions)
        {
            return services.AddMorsteadGrainStorage(name, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Configure silo to use Morstead as the default grain storage.
        /// </summary>
        public static IServiceCollection AddMorsteadGrainStorageAsDefault(this IServiceCollection services, Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            return services.AddMorsteadGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Morstead for grain storage.
        /// </summary>
        public static IServiceCollection AddMorsteadGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<MorsteadStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<MorsteadStorageOptions>(name));
            services.AddTransient<IConfigurationValidator>(sp => new MorsteadStorageOptionsValidator(sp.GetRequiredService<IOptionsMonitor<MorsteadStorageOptions>>().Get(name), name));
            services.ConfigureNamedOptionForLogging<MorsteadStorageOptions>(name);
            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, MorsteadGrainStorageFactory.Create)
                           .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}
