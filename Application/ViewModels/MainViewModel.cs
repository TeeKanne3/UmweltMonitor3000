using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class MainViewModel :ObservableObject
{
    public ObservableCollection<object> Tabs { get; set; }

    public MainViewModel()
    {
        Tabs = new ObservableCollection<object>
        {
            new MqttViewModel(),
            new PlantViewModel(),
            new StatisticViewModel()
        };
        
    }
}
