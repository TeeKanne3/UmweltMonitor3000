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

    [ObservableProperty]
    public partial ISeries[] MoistureSeries { get; set; } = [];

    [ObservableProperty]
    public partial ISeries[] TemperatureSeries { get; set; } = [];

    [ObservableProperty]
    public partial ISeries[] AllPlantMoisture { get; set; } = [];

    [ObservableProperty]
    public partial int MoisturePercent { get; set; }

    [ObservableProperty]
    public partial string MoistureTemperature { get; set; } = "-";

    [ObservableProperty]
    public partial string Temperature { get; set; } = "-";

    [ObservableProperty]
    public partial string CurrentTemperature { get; set; } = "-";

    [ObservableProperty]
    public partial Axis[] AllPlantSeriesXAxes { get; set; } = [];

    public Axis[] PlantSeriesXAxes { get; } =
    [
        new Axis
        {
            Labeler = value => value >= DateTime.MinValue.Ticks && value <= DateTime.MaxValue.Ticks
                ? new DateTime((long)value).ToString("HH:mm")
                : string.Empty,
            LabelsRotation = -45,
            TextSize = 10,
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
        }
    ];

    public Axis[] PlantSeriesYAxes { get; } =
    [
        new Axis
        {
            MinLimit = 0,
            MaxLimit = 100,
            TextSize = 10,
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
        }
    ];

    public Axis[] AllPlantSeriesYAxes { get; } =
    [
        new Axis
        {
            MinLimit = 0,
            MaxLimit = 100,
            TextSize = 10,
            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
        }
    ];

    public RectangularSection[] PlantSeriesSections { get; } =
    [
        new RectangularSection
        {
            Yi = 30, Yj = 30,
            Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 1 }
        },
        new RectangularSection
        {
            Yi = 70, Yj = 70,
            Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 1 }
        }
    ];

    public StatisticViewModel(MainWindowLogic logic, PlantViewModel plantViewModel)
    {
        _logic = logic;
        PlantCollection = plantViewModel.PlantCollection;
        PlantCollection.CollectionChanged += (_, _) => UpdateAllPlantMoisture();
        _logic.MessageLogged += OnMessageLogged;
    }

    private void OnMessageLogged(string topic, string payload)
    {
        UpdateAllPlantMoisture();
        if (SelectedPlant != null && topic == SelectedPlant.MqttTopic)
        {
            LoadPlantData(SelectedPlant);
            UpdateMoistureInfo(SelectedPlant);
        }
    }

    partial void OnSelectedPlantChanged(Plant? value)
    {
        if (value == null)
        {
            PlantSeries = [];
            MoistureSeries = [];
            MoisturePercent = 0;
            MoistureTemperature = "-";
            return;
        }
        LoadPlantData(value);
        UpdateMoistureInfo(value);
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

    private void UpdateMoistureInfo(Plant plant)
    {
        MoisturePercent = plant.MoisturePercent;
        MoistureTemperature = $"{plant.MoisturePercent}%";

        // PieChart MaxValue=50 + InitialRotation=-90 → Halbkreis-Gauge; Wert auf 0-50 skalieren
        double scaled = plant.MoisturePercent / 2.0;
        MoistureSeries =
        [
            new PieSeries<ObservableValue>
            {
                Values = [new ObservableValue(scaled)],
                InnerRadius = 60,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                Stroke = null,
            },
            new PieSeries<ObservableValue>
            {
                Values = [new ObservableValue(50 - scaled)],
                InnerRadius = 60,
                Fill = new SolidColorPaint(new SKColor(40, 55, 70)),
                Stroke = null,
            }
        ];
    }

    private void UpdateAllPlantMoisture()
    {
        var plants = PlantCollection.ToArray();

        AllPlantSeriesXAxes =
        [
            new Axis
            {
                Labels = plants.Select(p => p.Name).ToArray(),
                TextSize = 10,
                LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
            }
        ];

        AllPlantMoisture =
        [
            new ColumnSeries<int>
            {
                Values = plants.Select(p => p.MoisturePercent).ToArray(),
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                Stroke = null,
            }
        ];
    }
}
