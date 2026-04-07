using CommunityToolkit.Mvvm.ComponentModel;

namespace UmweltMonitor3000.Application.Models;

public partial class Plant : ObservableObject
{
    [ObservableProperty]
    private int _plantID;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MqttTopic))]
    private string _name = string.Empty;

    public string MqttTopic => $"umweltmonitor/sensor/{Name}";
    [ObservableProperty]
    private int _moistureSensor;
    [ObservableProperty]
    private int _moisturePercent;
    [ObservableProperty]
    private DateTime _lastUpdate;
    [ObservableProperty]
    private int _batteryState;
}