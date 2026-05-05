
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using HendrixRFID.Data;
using HendrixRFID.DTOs;
using HendrixRFID.Models;

namespace HendrixRFID.Services;

public class ScanProcessorService : BackgroundService
{
    private readonly ChannelReader<MqttScanMessage> _channelReader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScanProcessorService> _logger;

    public ScanProcessorService(Channel<MqttScanMessage> channel, IServiceScopeFactory scopeFactory, ILogger<ScanProcessorService> logger)
    {
        _channelReader = channel.Reader;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channelReader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            foreach (var tag in message.Tags)
            {
                db.RawScans.Add(new RawScan
                {
                    PigId = tag.EpcHex,
                    LampId = message.LampId,
                    SignalStrength = tag.SignalStrength,
                    ScanTime = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Gemt {Count} scanninger fra {LampId}", message.Tags.Count, message.LampId);
        }
    }
}