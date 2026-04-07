using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class PlantViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<Plant> _plantCollection = new();

    public PlantViewModel()
    {
        var plant1 = new Plant { PlantID = 1, MoistureSensor = 620, MoisturePercent = 60, LastUpdate = DateTime.Now, Name = "Glücksbaum", BatteryState = 99 };
        var plant2 = new Plant { PlantID = 2, MoistureSensor = 620, MoisturePercent = 60, LastUpdate = DateTime.Now, Name = "Glücksbaum", BatteryState = 99 };
        var plant3 = new Plant { PlantID = 3, MoistureSensor = 620, MoisturePercent = 60, LastUpdate = DateTime.Now, Name = "Glücksbaum", BatteryState = 99 };

        PlantCollection.Add(plant1);
        PlantCollection.Add(plant2);
        PlantCollection.Add(plant3);
    }
}