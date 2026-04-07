using MQTTnet;
using System.Text;
using UmweltMonitor3000.Application.Interfaces;

namespace UmweltMonitor3000.Application.Services;

public class MqttService : IMqttService
{
    private IMqttClient? _mqttClient;

    public event Action<string, string>? MessageReceived;
    public event Action<string>? LogMessage;

    public async Task ConnectAsync(string brokerAddress, int brokerPort, string clientId, string username, string password)
    {
        if (_mqttClient != null)
        {
            if (_mqttClient.IsConnected)
                await _mqttClient.DisconnectAsync();
            _mqttClient.Dispose();
            _mqttClient = null;
        }

        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            MessageReceived?.Invoke(topic, payload);

            return Task.CompletedTask;
        };

        var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(brokerAddress, brokerPort)
            .WithCredentials(username, password)
            .WithCleanSession()
            .Build();


        await _mqttClient.ConnectAsync(options);
    }

    public async Task DisconnectAsync()
    {
        if (_mqttClient != null && _mqttClient.IsConnected)
            await _mqttClient.DisconnectAsync();
    }

    public async Task PublishAsync(string topic, string payload)
    {
        if (_mqttClient == null || !_mqttClient.IsConnected)
        {
            LogMessage?.Invoke("MQTT client is not connected.");
            return;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        await _mqttClient.PublishAsync(message);
    }

    public async Task SubscribeAsync(string topic)
    {
        if (_mqttClient == null || !_mqttClient.IsConnected)
        {
            LogMessage?.Invoke("MQTT client is not connected.");
            return;
        }

        await _mqttClient.SubscribeAsync(topic);
    }
}
