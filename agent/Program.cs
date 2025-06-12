using Agent.Models;
using Agent.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Agent;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting Semantic Kernel Agent...");

            var kernel = host.Services.GetRequiredService<Kernel>();
            
            // TODO: Add main logic here
            logger.LogInformation("Agent started successfully with {PluginCount} plugins", kernel.Plugins.Count);
            
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application terminated unexpectedly: {ex.Message}");
            Environment.Exit(1);
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                // Configure settings
                services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));

                // Add HttpClient for API calls
                services.AddHttpClient();

                // Configure Semantic Kernel
                var kernelBuilder = Kernel.CreateBuilder();
                
                // Add logging
                kernelBuilder.Services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Add HttpClient to kernel services
                kernelBuilder.Services.AddHttpClient();

                // Add API settings to kernel services
                kernelBuilder.Services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));

                // Build kernel
                var kernel = kernelBuilder.Build();

                // Register plugins with proper DI
                var httpClientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
                var apiSettings = Microsoft.Extensions.Options.Options.Create(context.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings());

                kernel.Plugins.AddFromObject(new ConnectPlugin(httpClient, loggerFactory.CreateLogger<ConnectPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new SmsPlugin(httpClient, loggerFactory.CreateLogger<SmsPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersAddressManagementPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersAddressManagementPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersCDRPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersCDRPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersDisconnectionPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersDisconnectionPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersEmergencyServicesPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersEmergencyServicesPlugin>(), apiSettings));
                kernel.Plugins.AddFromObject(new MyNumbersNumberPortingPlugin(httpClient, loggerFactory.CreateLogger<MyNumbersNumberPortingPlugin>(), apiSettings));

                services.AddSingleton(kernel);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });
}