using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using System.Drawing;
using System.Windows.Media;

namespace UmweltMonitor3000.Application.Models;

public partial class Plant : ObservableObject
{
    [ObservableProperty]
    public partial int PlantID { get; set; }
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial int MoistureSensor { get; set; }
    [ObservableProperty]
    public partial int MoisturePercent { get; set; }
    [ObservableProperty]
    public partial DateTime LastUpdate { get; set; }
    [ObservableProperty]
    public partial int BatteryState { get; set; }
}
