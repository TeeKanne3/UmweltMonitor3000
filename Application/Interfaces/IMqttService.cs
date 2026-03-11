using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.Interfaces;

public interface IMqttService
{
    event Action<SensorData> OnMessageReceived;
    Task ConnectAsync(string clientId, string username, string password);
    Task SubscribeAsync(string topic);
    Task DisconnectAsync();
}
