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
    public event Action<string>? ErrorLogged;
    public event Action<string, string>? MessageLogged;

    public MainWindowLogic()
    {
        _mqttService = new MqttService();
        _repository = new UmweltMonitorRepository();

        _mqttService.MessageReceived += OnMessageReceived;
        _mqttService.LogMessage += LogError;
        _repository.LogMessage += LogError;
    }

    private async void OnMessageReceived(string topic, string payload)
    {
        Log($"Received from: {topic} - Payload: {payload}");

        var sensorId = topic.Split('/').Last();

        await _repository.SaveSensorDataAsync(topic, payload);
        MessageLogged?.Invoke(topic, payload);
    }

    public async Task Connect(string ip, int port)
    {
        try
        {
            await _mqttService.ConnectAsync(ip, port, "client1", "", "");
            await _mqttService.SubscribeAsync("umweltmonitor/sensor/#"); // Anpassen je nach Bedarf (Topic)
            Status = "Connected";

            Log($"Mqtt is connected to {ip}:{port}");
        }
        catch (Exception ex)
        {
            LogError($"Error while connecting to {ip}:{port}: {ex.Message}");
        }
    }

    public async Task Disconnect()
    {
        await _mqttService.DisconnectAsync();
        Status = "Disconnected";

        Log($"Mqtt is Disconnected!");
    }

    private void Log(string message)
    {
        LogMessage?.Invoke(message);
    }

    public void LogError(string message)
    {
        LogMessage?.Invoke(message);
        ErrorLogged?.Invoke(message);
    }
}