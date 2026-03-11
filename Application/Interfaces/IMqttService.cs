namespace UmweltMonitor3000.Application.Interfaces;

public interface IMqttService
{
    Task ConnectAsync(string brokerAddress, int brokerPort, string clientId, string username, string password);
    Task SubscribeAsync(string topic);
    Task PublishAsync(string topic, string payload);
    Task DisconnectAsync();
}
