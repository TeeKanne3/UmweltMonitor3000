using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class StatisticViewModel : ObservableObject
{
    private readonly MainWindowLogic _logic;

    public ObservableCollection<Plant> PlantCollection { get; }

    [ObservableProperty]
    private Plant? _selectedPlant;

    [ObservableProperty]
    public partial ISeries[] PlantSeries { get; set; } = [];

    public StatisticViewModel(MainWindowLogic logic, PlantViewModel plantViewModel)
    {
        _logic = logic;
        PlantCollection = plantViewModel.PlantCollection;
    }

    partial void OnSelectedPlantChanged(Plant? value)
    {
        if (value == null)
        {
            PlantSeries = [];
            return;
        }
        LoadPlantData(value);
    }

    private async void LoadPlantData(Plant plant)
    {
        var data = await _logic.GetPlantHistoryAsync(plant.MqttTopic);
        var values = data.Select(d => d.Value).ToArray();

        PlantSeries =
        [
            new LineSeries<double>
            {
                Values = values,
                Name = plant.Name
            }
        ];
    }
}
