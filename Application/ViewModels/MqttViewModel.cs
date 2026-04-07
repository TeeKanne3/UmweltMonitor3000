using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class MqttViewModel : ObservableObject
{
    private readonly MainWindowLogic _logic;

    private readonly DispatcherTimer _upTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private TimeSpan _elapsed = TimeSpan.Zero;

    [ObservableProperty]
    public string _ipAdresse = "localhost";
    [ObservableProperty]
    private int _port = 1883;
    [ObservableProperty]
    private ObservableCollection<TopicLog> _logCollection = new();
    [ObservableProperty]
    private string _status = "Getrennt";
    [ObservableProperty]
    private string _broker = "-";
    [ObservableProperty]
    private int _receivedMessage = 0;
    [ObservableProperty]
    private int _errorCount = 0;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    private bool _isConnected = false;

    public bool IsNotConnected => !IsConnected;
    [ObservableProperty]
    private string _upTime = "--:--:--";

    public MqttViewModel()
    {
        _logic = new MainWindowLogic();
        _logic.LogMessage += OnLogMessage;
        _logic.ErrorLogged += OnErrorLogged;
        _logic.MessageLogged += OnMessageLogged;

        _upTimer.Tick += (_, _) =>
        {
            _elapsed = _elapsed.Add(TimeSpan.FromSeconds(1));
            UpTime = _elapsed.ToString(@"hh\:mm\:ss");
        };
    }
 
    [RelayCommand]
    private async Task ConnectAsync()
    {
        await _logic.Connect(IpAdresse, Port);
 
        IsConnected = _logic.Status == "Connected";
        Status = IsConnected ? "Verbunden" : "Getrennt";
        Broker = IsConnected ? $"{IpAdresse}:{Port}" : "-";

        if (IsConnected)
        {
            _elapsed = TimeSpan.Zero;
            UpTime = "00:00:00";
            _upTimer.Start();
        }
    }
 
    [RelayCommand]
    private async Task DisconnectAsync()
    {
        await _logic.Disconnect();
 
        IsConnected = false;
        Status = "Getrennt";
        Broker = "-";

        _upTimer.Stop();
    }
 
    private void OnLogMessage(string message)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            LogCollection.Insert(0, new TopicLog
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateOnly.FromDateTime(DateTime.Now),
                LogTyp = "INFO",
                Direction = "IN",
                Topic = "-",
                Payload = message
            });

            ReceivedMessage = LogCollection.Count(l => l.Direction == "IN" && l.Topic != "-");
        });
    }

    private void OnMessageLogged(string topic, string payload)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            LogCollection.Insert(0, new TopicLog
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateOnly.FromDateTime(DateTime.Now),
                LogTyp = "MQTT",
                Direction = "IN",
                Topic = topic,
                Payload = payload
            });
            ReceivedMessage++;
        });
    }

    private void OnErrorLogged(string message)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            ErrorCount++;
        });
    }
}
