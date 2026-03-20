using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class PlantViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Plant> PlantCollection { get; set; } = new();

    public PlantViewModel()
    {
        var plant1 = new Plant
        {
            PlantID = 1,
            MoistureSensor = 620,
            MoisturePercent = 60,
            LastUpdate = DateTime.Now,
            Name = "Glücksbaum",
            BatteryState = 99,
        };

        var plant2 = new Plant
        {
            PlantID = 2,
            MoistureSensor = 620,
            MoisturePercent = 60,
            LastUpdate = DateTime.Now,
            Name = "Glücksbaum",
            BatteryState = 99,
        };

        var plant3 = new Plant
        {
            PlantID = 3,
            MoistureSensor = 620,
            MoisturePercent = 60,
            LastUpdate = DateTime.Now,
            Name = "Glücksbaum",
            BatteryState = 99,
        };

        PlantCollection.Add(plant1);
        PlantCollection.Add(plant2);
        PlantCollection.Add(plant3);

    }
}
