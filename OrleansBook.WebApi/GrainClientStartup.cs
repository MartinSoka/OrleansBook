using Orleans.Configuration;
using System.Net;

namespace OrleansBook.WebApi
{
    public static class GrainClientStartup
    {
        public static IHostBuilder UseSharedUnitsOrleansSilo(this WebApplicationBuilder builder)
        {
            return builder.Host.UseOrleans((context, siloBuilder) =>
            {
                // using default storage account and connectionString in appsettings.json
                var connectionString = builder.Configuration.GetConnectionString("azurite");


                // Use azure table storage for silo data for the Orleans cluster
                siloBuilder.UseAzureStorageClustering(options =>
                        options.ConfigureTableServiceClient(connectionString))
                        // for persisting grains
                        .AddAzureTableGrainStorage("robots",
                                        options => options.ConfigureTableServiceClient(connectionString));
                siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "robots-cluster";
                        options.ServiceId = "robots-service";
                    });
            });
        }
    }
}
