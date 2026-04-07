using CommunityToolkit.Mvvm.ComponentModel;

namespace UmweltMonitor3000.Application.Models;

public partial class Plant : ObservableObject
{
    [ObservableProperty]
    private int _plantID;
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private int _moistureSensor;
    [ObservableProperty]
    private int _moisturePercent;
    [ObservableProperty]
    private DateTime _lastUpdate;
    [ObservableProperty]
    private int _batteryState;
}