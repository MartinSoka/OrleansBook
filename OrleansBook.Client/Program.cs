using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrleansBook.GrainInterfaces;

try
{
    using IHost host = await StartClientAsync();
    var client = host.Services.GetRequiredService<IClusterClient>();

    await DoClientWorkAsync(client);
    Console.ReadKey();

    await host.StopAsync();

    return 0;
}
catch (Exception e)
{
    Console.WriteLine($$"""
        Exception while trying to run client: {{e.Message}}
        Make sure the silo the client is trying to connect to is running.
        Press any key to exit.
        """);

    Console.ReadKey();
    return 1;
}

static async Task<IHost> StartClientAsync()
{
    var builder = new HostBuilder()
        .UseOrleansClient(client =>
        {
            client.UseLocalhostClustering();
        })
        .ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
        });

    var host = builder.Build();
    await host.StartAsync();

    Console.WriteLine("Client successfully connected to silo host \n");

    return host;
}

static async Task DoClientWorkAsync(IClusterClient client)
{
    while (true)
    {
        Console.WriteLine("Please enter a robot name...");
        var grainId = Console.ReadLine();
        var grain = client.GetGrain<IRobotGrain>(grainId);
        Console.WriteLine("Please enter an instruction...");
        var instruction = Console.ReadLine();
        await grain.AddInstruction(instruction);

        var count = await grain.GetInstructionCount();
        Console.WriteLine($"{grainId} has {count} instruction(s)");
    }
}