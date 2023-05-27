using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

try
{
    using IHost host = await StartSiloAsync();
    Console.WriteLine("\n\n Press Enter to terminate...\n\n");
    Console.ReadLine();

    await host.StopAsync();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task<IHost> StartSiloAsync()
{
    var builder = new HostBuilder()
        .UseOrleans(silo =>
        {
            silo.UseLocalhostClustering()
                .AddMemoryGrainStorage("robotStateStore")
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .UseDashboard();
        });

    var host = builder.Build();
    await host.StartAsync();

    return host;
}