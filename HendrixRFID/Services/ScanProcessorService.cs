using System.Threading.Channels;
using HendrixRFID.DTOs;

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
            var decoder  = scope.ServiceProvider.GetRequiredService<EartagDecoder>();
            var pigLocationService = scope.ServiceProvider.GetRequiredService<PigLocationService>();

            // Altid opdater lampens heartbeat (også selvom Tags listen er tom)
            await pigLocationService.UpdateLampHeartbeatAsync(message.LampId);

            var validScans = new List<(string pigId, int signalStrength)>();

            foreach (var tag in message.Tags)
            {
                // Dekod EpcHex => PigId
                var pigId = decoder.Decode(tag.EpcHex);
                if (pigId is null)
                {
                    _logger.LogWarning("Kunne ikke dekode EPC: {EpcHex}", tag.EpcHex);
                    continue;
                }

                validScans.Add((pigId, tag.SignalStrength));
            }

            if (validScans.Any())
            {
                await pigLocationService.ProcessScansBatchAsync(message.LampId, validScans);
            }

            _logger.LogInformation("Behandlet {Count} tags fra {LampId}", message.Tags.Count, message.LampId);
        }
    }
}