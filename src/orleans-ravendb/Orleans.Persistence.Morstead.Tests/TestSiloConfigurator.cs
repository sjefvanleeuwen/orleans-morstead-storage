using Microsoft.Extensions.Options;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Orleans.Persistence.Morstead.Tests
{
    public class TestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder
                .AddMorsteadGrainStorage(name: "unit-test");
        }
    }
}
