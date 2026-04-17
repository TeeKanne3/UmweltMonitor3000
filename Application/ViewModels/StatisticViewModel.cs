using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using OpenTK.Graphics.ES20;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace UmweltMonitor3000.Application.ViewModels;

public partial class StatisticViewModel : ObservableObject
{
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
    public Axis[] PlantSeriesYAxes { get; set; } = {
    new Axis
    {
       MinLimit = 0,
        MaxLimit = 100,
        LabelsPaint = new SolidColorPaint(SKColors.White),
        Labeler = value => $"{value}%", // Fügt ein Prozentzeichen hinzu
        SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(30)) { StrokeThickness = 1 }
    }
};
    public RectangularSection[] PlantSeriesSections { get; set; } = {
    new RectangularSection
    {
        Yi = 0,
        Yj = 20,
        Fill = new SolidColorPaint(SKColors.Red.WithAlpha(50))
    }
};

    //Balkendiagramm für die Feuchtigkeit aller Pflanzen
    [ObservableProperty]
    public partial ISeries[] AllPlantMoisture { get; set; } = [
        new RowSeries<double> { Values = [920],Fill = new SolidColorPaint(SKColors.DeepSkyBlue),Name = "Lowenzahn"}, 
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
        new RowSeries<double> { Values = [1000], Fill = new SolidColorPaint(SKColors.Teal), Name = "Hamilton" },
    ];
    public Axis[] AllPlantSeriesXAxes { get; set; } = [
    new Axis
    {
        LabelsPaint = new SolidColorPaint(SKColors.White), 
        SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(50)) 
    }];
    public Axis[] AllPlantSeriesYAxes { get; set; } = [
    new Axis
    {
        LabelsPaint = new SolidColorPaint(SKColors.White)
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
            DataLabelsSize = 30,
            MaxRadialColumnWidth = 50,
            DataLabelsPaint = null
        }
            ];
    [ObservableProperty]
    public partial string MoistureTemperature { get; set; } = "30°C";



    [RelayCommand]
    public void OnPointMeasured(ChartPoint point)
    {
        var plant = (PlantMoisture?)point.Context.DataSource;
        if (point.Context.Visual is null || plant is null)
            return;

        point.Context.Visual.Fill = plant.Paint;
    }
    private static string GetPlantName(ChartPoint point)
    {
        var plant = (PlantMoisture?)point.Context.DataSource;
        return plant is null ? string.Empty : plant.Name;
    }
    private static PlantMoisture[] Fetch()
    {
        var paints = Enumerable.Range(0, 7)
            .Select(i => new SolidColorPaint(ColorPalletes.MaterialDesign500[i].AsSKColor()))
            .ToArray();

        return [
            new("Tsunoda",   500,  paints[0]),
            new("Sainz",     450,  paints[1]),
            new("Riccardo",  520,  paints[2]),
            new("Bottas",    550,  paints[3]),
            new("Perez",     660,  paints[4]),
            new("Verstapen", 920,  paints[5]),
            new("Hamilton",  1000, paints[6])
        ];
    }



    public class PlantMoisture : ObservableValue
    {
        public PlantMoisture(string name, int value, SolidColorPaint paint)
        {
            Name = name;
            Paint = paint;
            Value = value;
        }

        public string Name { get; set; }
        public SolidColorPaint Paint { get; set; }
    }
}






