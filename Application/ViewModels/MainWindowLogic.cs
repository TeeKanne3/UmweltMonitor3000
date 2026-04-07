using UmweltMonitor3000.Application.Interfaces;
using UmweltMonitor3000.Application.Repositories;
using UmweltMonitor3000.Application.Services;

namespace UmweltMonitor3000.Application.ViewModels;

public class MainWindowLogic
{
    private readonly IMqttService _mqttService;
    private readonly IUmweltMonitorRepository _repository;


    public string Status { get; private set; } = "Disconnected";

    public event Action<string>? LogMessage;

    public MainWindowLogic()
    {
        _mqttService = new MqttService();
        _repository = new UmweltMonitorRepository();

        _mqttService.MessageReceived += OnMessageReceived;
        _mqttService.LogMessage += Log;
        _repository.LogMessage += Log;
    }

    private async void OnMessageReceived(string topic, string payload)
    {
        Log($"Received from: {topic} - Payload: {payload}");

        var sensorId = topic.Split('/').Last();

        await _repository.SaveSensorDataAsync(sensorId, payload);
    }

    public async Task Connect(string ip, int port)
    {
        try
        {
            await _mqttService.ConnectAsync(ip, port, "client1", "", "");
            Status = "Connected";

            Log($"Mqtt is connected to {ip}:{port}");
        }
        catch
        {
            Log($"Error while connecting to {ip}:{port}");
        }
    }

    public async Task Disconnect()
    {
        await _mqttService.DisconnectAsync();
        Status = "Disconnected";

        Log($"Mqtt is Disconnected!");
    }

    public async Task Subscribe(string topic)
    {
        await _mqttService.SubscribeAsync(topic);
        Log($"Subscribed to topic: {topic}");
    }

    private void Log(string message)
    {
        LogMessage?.Invoke(message);
    }
}