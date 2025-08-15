﻿
using MisguidedLogs.SearchEngine.Repositories.Bunnycdn;

namespace MisguidedLogs.SearchEngine.Services.HostedService;

public class Updater(SearchEngine engine, BunnyCdnStorageLoader loader, ILogger<Updater> log) : IHostedService
{
    private static DateTime dateTime = DateTime.MinValue;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested) // Check for cancellation
            {
                var lastInfo = (await loader.GetListOfStorageObjects("misguided-logs-warcraftlogs/gold/"))
                    .FirstOrDefault(x => x.ObjectName == "players.json.gz");

                if (lastInfo != null && dateTime != lastInfo.LastChanged)
                {
                    log.LogInformation("New Update found");
                    dateTime = lastInfo.LastChanged;
                    await engine.UpdateAllDocuments();
                }

                // Delay for a specified time, but check for cancellation
                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
