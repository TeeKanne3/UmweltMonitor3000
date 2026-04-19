namespace UmweltMonitor3000.Application.Interfaces;

public interface INotificationService
{
    void SendNotification(string title, string message);
}