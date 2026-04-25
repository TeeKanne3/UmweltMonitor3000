using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class StatisticViewModel : ObservableObject
{
    private readonly MainWindowLogic _logic;

    public ObservableCollection<Plant> PlantCollection { get; }

    [ObservableProperty]
    public partial double MoisturePercent {  get; set; }

    //Linien Chart Properties
    [ObservableProperty]
    public partial ISeries[] PlantSeries { get; set; }
            =
            [
                new LineSeries<int>
                {
                    Values = [40, 60, 50, 30, 25, 10, 15],
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 3, IsAntialias = true },
                    GeometryFill = new SolidColorPaint(SKColors.DeepSkyBlue),
                    GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2, IsAntialias = true },
                    GeometrySize = 10,
                    Fill = new SolidColorPaint(SKColors.DeepSkyBlue.WithAlpha(50))
                },
            ];
    public Axis[] PlantSeriesXAxes { get; set; } = {
    new Axis
    {
        LabelsPaint = new SolidColorPaint(SKColors.White),
        Labeler = value => DateTime.Now.AddHours(value - 24).ToString("HH:mm"),
        UnitWidth = 1,
    }
};
    public Axis[] PlantSeriesYAxes { get; set; } = [
    new Axis
    {
        MinLimit = 0,
        MaxLimit = 100,
        LabelsPaint = new SolidColorPaint(SKColors.White),
        Labeler = value => $"{value}%", 
        SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(30)) { StrokeThickness = 1 }
    }];
    public RectangularSection[] PlantSeriesSections { get; set; } = [
    new RectangularSection
    {
        Yi = 0,
        Yj = 20,
        Fill = new SolidColorPaint(SKColors.Red.WithAlpha(50))
    }];

    //Balkendiagramm für die Feuchtigkeit aller Pflanzen
    [ObservableProperty]
    public partial ISeries[] AllPlantMoisture { get; set; } = [
        new RowSeries<double> { Values = [60],Fill = new SolidColorPaint(SKColors.DeepSkyBlue)},
        new RowSeries<double> { Values = [70], Fill = new SolidColorPaint(SKColors.Teal)},
        new RowSeries<double> { Values = [55], Fill = new SolidColorPaint(SKColors.Teal)},
        new RowSeries<double> { Values = [87], Fill = new SolidColorPaint(SKColors.Teal) },
        new RowSeries<double> { Values = [39], Fill = new SolidColorPaint(SKColors.Teal) },
        new RowSeries<double> { Values = [21], Fill = new SolidColorPaint(SKColors.Teal) },
        new RowSeries<double> { Values = [44], Fill = new SolidColorPaint(SKColors.Teal) },
        new RowSeries<double> { Values = [61], Fill = new SolidColorPaint(SKColors.Teal) },
        new RowSeries<double> { Values = [5], Fill = new SolidColorPaint(SKColors.Teal) },
    ];
    public Axis[] AllPlantSeriesXAxes { get; set; } = [
    new Axis
    {
        
        LabelsPaint = new SolidColorPaint(SKColors.White),
        SeparatorsPaint = null,
        MaxLimit = 100,
        MinLimit = 0,
        Labeler = value => $"{value}%",
    }];
    public Axis[] AllPlantSeriesYAxes { get; set; } = [
    new Axis
    {
        LabelsPaint = new SolidColorPaint(SKColors.White),
        Labels =
        [
            "Löwenzahn",
            "Hamilton 1",
            "Hamilton 2",
            "Hamilton 3",
            "Hamilton 4",
            "Hamilton 5",
            "Hamilton 6",
            "Hamilton 7",
            "Hamilton 8"
        ],

        TextSize = 14,
        Padding = new Padding(0, 0, 15, 0),
        ForceStepToMin = true,
        MinStep = 1
    }];


    //Raum Temperatur Gauge
    [ObservableProperty]
    public partial ISeries[] TemperatureSeries { get; set; }
    =
            [
        new PieSeries<double>
        {
            Values = [30.0],
            DataLabelsSize = 30,
            MaxRadialColumnWidth = 50,
            DataLabelsPaint = null
        }
            ];
    [ObservableProperty]
    public partial string CurrentTemperature { get; set; } = "30°C";

    //Feuchtigkeits Gauge der ausgewählten Pflanze
    [ObservableProperty]
    public partial ISeries[] MoistureSeries { get; set; }
    =
            [
        new PieSeries<double>
        {
            Values = [30.0],
            Fill = new SolidColorPaint(SKColors.YellowGreen),
            DataLabelsSize = 30,
            MaxRadialColumnWidth = 50,
            DataLabelsPaint = null
        }
            ];
    [ObservableProperty]
    public partial string MoistureTemperature { get; set; } = "30%";

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

    //partial void OnSelectedPlantChanged(Plant? value)
    //{
    //    if (value == null)
    //    {
    //        PlantSeries = [];
    //        return;
    //    }
    //    LoadPlantData(value);
    //}

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
