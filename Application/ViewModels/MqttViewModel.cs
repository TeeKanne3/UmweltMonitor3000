using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class MqttViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int IpAdresse { get; set; }
    [ObservableProperty]
    public partial int Port { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<TopicLog> LogCollection { get; set; }
    [ObservableProperty]
    public partial string Status { get; set; }
    [ObservableProperty]
    public partial string Broker { get; set; }
    [ObservableProperty]
    public partial int ReceivedMessage {  get; set; }
    [ObservableProperty]
    public partial string ErrorCount { get; set; }

}
