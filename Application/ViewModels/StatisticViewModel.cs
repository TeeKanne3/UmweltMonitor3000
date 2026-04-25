using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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

    public Axis[] XAxes { get; } =
    [
        new Axis
        {
            Labeler = value => value >= DateTime.MinValue.Ticks && value <= DateTime.MaxValue.Ticks
                ? new DateTime((long)value).ToString("HH:mm:ss")
                : string.Empty,
            LabelsRotation = -45,
            TextSize = 10,
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
        }
    ];

    public StatisticViewModel(MainWindowLogic logic, PlantViewModel plantViewModel)
    {
        _logic = logic;
        PlantCollection = plantViewModel.PlantCollection;
        _logic.MessageLogged += OnMessageLogged;
    }

    private void OnMessageLogged(string topic, string payload)
    {
        if (SelectedPlant != null && topic == SelectedPlant.MqttTopic)
            LoadPlantData(SelectedPlant);
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
        var points = data.Select(d => new DateTimePoint(d.TimeStamp.ToLocalTime(), d.Value)).ToArray();

        PlantSeries =
        [
            new LineSeries<DateTimePoint>
            {
                Values = points,
                Name = plant.Name
            }
        ];
    }
}
