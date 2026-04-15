using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace UmweltMonitor3000.Application.ViewModels;

public partial class StatisticViewModel :ObservableObject
{
    [ObservableProperty]
    public partial ISeries[] PlantSeries { get; set; }
            =
            [
                new LineSeries<int>
                {
                    Values = [4, 6, 5, 3, -3, -1, 2]
                },
            ];


}
            

        


