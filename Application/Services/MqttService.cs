using MQTTnet;
using UmweltMonitor3000.Application.Interfaces;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.Services;

public class MqttService : IMqttService
{
    private readonly IMqttClient _client;
    private readonly string _broker;
    private readonly int _port;

    public event Action<SensorData>? OnMessageReceived;

    public MqttService(string broker = "localhost", int port = 1883)
    {
        _broker = broker;
        _port = port;
        MqttClientFactory factory = new();
        _client = factory.CreateMqttClient();
    }

    public async Task ConnectAsync(string clientId, string username, string password)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_broker, _port)
            .WithCleanSession()
            .Build();
        _client.ApplicationMessageReceivedAsync += HandleMessage;
        await _client.ConnectAsync(options);
    }

    public async Task DisconnectAsync()
    {
        await _client.DisconnectAsync();
    }

    public async Task SubscribeAsync(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build();

        await _client.SubscribeAsync(topicFilter);
    }

    private Task HandleMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        string json = eventArgs.ApplicationMessage.ConvertPayloadToString();
        SensorData? msg = SensorData.FromJson(json);

        if (msg is null)
            return Task.CompletedTask;

        msg.Topic = eventArgs.ApplicationMessage.Topic;
        OnMessageReceived?.Invoke(msg);
        return Task.CompletedTask;
    }
}