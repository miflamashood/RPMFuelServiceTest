using RPMFuelService;
using Microsoft.EntityFrameworkCore;
using RPMFuelService.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext,services) =>
    {
        services.AddTransient<FuelDbContext>();
        services.AddTransient<IRPMFuelService, RPMFuelService.RPMFuelService>();
        services.AddTransient<IRPMFuelRepository, RPMFuelRepository>();
        services.AddTransient<IRPMFuelDataProvider, RPMFuelDataProvider>();
        services.AddHostedService<RPMFuelServiceWorker>();
        services.AddDbContext<FuelDbContext>(options =>
            options.UseSqlServer(builderContext.Configuration.GetConnectionString("DefaultConnection")));

        services.Configure<RPMFuelServiceConfig>(builderContext.Configuration.GetSection(nameof(RPMFuelServiceConfig)));

        services.AddLogging(cfg =>
        {
            cfg.AddConfiguration(
                builderContext.Configuration); // This doesn't work. Why? Try using Host.CreateDefaultBuilder() instead: https://www.thecodebuzz.com/loading-configuration-ini-xml-json-net-core-console-winform/
            cfg.AddConsole();
            cfg.AddDebug();
        });
        services.AddSingleton(builderContext.Configuration);


    })
    //.ConfigureAppConfiguration((context, builder) =>
    //{
    // // builder.SetBasePath(Directory.GetCurrentDirectory());
    //    builder.AddJsonFile("appsettings.json");
    //  //  builder.AddEnvironmentVariables();
    //   // builder.AddCommandLine(args);

    //})
    .Build();

CreateDbIfNotExists(host);
await host.RunAsync();

static void CreateDbIfNotExists(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FuelDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
