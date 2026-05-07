using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using MQTTnet;
using MQTTnet.Client;
using HendrixRFID.DTOs;

namespace HendrixRFID.Services;

public class MqttListenerService : BackgroundService
{
    private readonly ChannelWriter<MqttScanMessage> _channelWriter;
    private readonly ILogger<MqttListenerService> _logger;

    public MqttListenerService(Channel<MqttScanMessage> channel, ILogger<MqttListenerService> logger)
    {
        _channelWriter = channel.Writer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("hendrix-backend")
            .Build();

        // Håndter indkomne beskeder
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            var message = JsonSerializer.Deserialize<MqttScanMessage>(payload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (message is not null)
                await _channelWriter.WriteAsync(message, stoppingToken);
        };

        await mqttClient.ConnectAsync(options, stoppingToken);
        await mqttClient.SubscribeAsync("hendrix/scans/#", cancellationToken: stoppingToken);
        _logger.LogInformation("MQTT lytter på hendrix/scans/#");

        // Bliv ved med at køre indtil applikationen stoppes
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}