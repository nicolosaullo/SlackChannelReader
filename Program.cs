using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlackChannelReader.Configuration;
using SlackChannelReader.Services;

namespace SlackChannelReader;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        try
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var orchestrator = host.Services.GetRequiredService<IArchiveOrchestrator>();

            logger.LogInformation("Starting Slack Channel Reader");

            // Parse command line arguments for date range (optional)
            DateTime? fromDate = null;
            DateTime? toDate = null;

            // Parse first argument if provided
            if (args.Length >= 1 && DateTime.TryParse(args[0], out var from))
            {
                fromDate = from;
                logger.LogInformation("From date specified: {FromDate:yyyy-MM-dd}", fromDate);
            }

            // Parse second argument if provided
            if (args.Length >= 2 && DateTime.TryParse(args[1], out var to))
            {
                toDate = to;
                logger.LogInformation("To date specified: {ToDate:yyyy-MM-dd}", toDate);
            }

            // If no dates specified, archive last 30 days instead of just today
            if (fromDate == null && toDate == null)
            {
                fromDate = DateTime.Today.AddDays(-30);
                toDate = DateTime.Today.AddDays(1).AddTicks(-1);
                logger.LogInformation("No date range specified. Archiving messages from last 30 days: {FromDate:yyyy-MM-dd} to {ToDate:yyyy-MM-dd}", fromDate, toDate);
            }
            else if (fromDate != null && toDate == null)
            {
                // If only from date specified, archive from that date to today
                toDate = DateTime.Today.AddDays(1).AddTicks(-1);
                logger.LogInformation("Only from date specified. Archiving from {FromDate:yyyy-MM-dd} to today", fromDate);
            }

            await orchestrator.ArchiveChannelsAsync(fromDate, toDate);

            logger.LogInformation("Archive process completed successfully");
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during the archive process");
            Environment.Exit(1);
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Configuration
                services.Configure<SlackOptions>(context.Configuration.GetSection("Slack"));
                services.Configure<ArchiveOptions>(context.Configuration.GetSection("Archive"));

                // HTTP Client
                services.AddHttpClient<ISlackClient, SlackClient>();

                // Services
                services.AddScoped<ISlackClient, SlackClient>();
                services.AddScoped<IJsonWriter, JsonWriter>();
                services.AddScoped<IArchiveOrchestrator, ArchiveOrchestrator>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}
