using Microsoft.Toolkit.Uwp.Notifications;
using UmweltMonitor3000.Application.Interfaces;

namespace UmweltMonitor3000.Application.Services;

public class NotificationService : INotificationService
{
    public void SendNotification(string title, string message)
    {
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .Show();
    }
}