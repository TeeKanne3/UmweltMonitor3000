using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class MqttViewModel : ObservableObject
{
    private const string LogFilePath = "logs.json";
    private const int MaxLogEntries = 500;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly object _saveLock = new();

    private readonly MainWindowLogic _logic;

    private readonly DispatcherTimer _upTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private TimeSpan _elapsed = TimeSpan.Zero;

    [ObservableProperty]
    private string _ipAdresse = "localhost";
    [ObservableProperty]
    private int _port = 1883;
    [ObservableProperty]
    private ObservableCollection<TopicLog> _logCollection = new();
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

    public MqttViewModel(MainWindowLogic logic)
    {
        _logic = logic;
        _logic.LogMessage += OnLogMessage;
        _logic.ErrorLogged += OnErrorLogged;
        _logic.MessageLogged += OnMessageLogged;

        _upTimer.Tick += (_, _) =>
        {
            _elapsed = _elapsed.Add(TimeSpan.FromSeconds(1));
            UpTime = _elapsed.ToString(@"hh\:mm\:ss");
        };

        LoadLogs();
    }

    private void LoadLogs()
    {
        if (!File.Exists(LogFilePath)) return;
        try
        {
            var json = File.ReadAllText(LogFilePath);
            var state = JsonSerializer.Deserialize<LogState>(json, _jsonOptions);
            if (state == null) return;

            foreach (var log in state.Logs)
                LogCollection.Add(log);

            ReceivedMessage = LogCollection.Count(l => l.Direction == "IN" && l.Topic != "-");
            ErrorCount = state.ErrorCount;
        }
        catch (Exception ex)
        {
            _logic.LogError($"Fehler beim Laden der Logs: {ex.Message}");
        }
    }

    private void SaveLogs()
    {
        var state = new LogState
        {
            Logs = LogCollection.ToList(),
            ErrorCount = ErrorCount
        };
        _ = Task.Run(() =>
        {
            lock (_saveLock)
            {
                try { File.WriteAllText(LogFilePath, JsonSerializer.Serialize(state, _jsonOptions)); }
                catch (Exception ex) { _logic.LogError($"Fehler beim Speichern der Logs: {ex.Message}"); }
            }
        });
    }

    [RelayCommand]
    private void ClearLogs()
    {
        LogCollection.Clear();
        ReceivedMessage = 0;
        ErrorCount = 0;
        if (File.Exists(LogFilePath))
            File.Delete(LogFilePath);
    }

    private record LogState
    {
        public List<TopicLog> Logs { get; init; } = [];
        public int ErrorCount { get; init; }
    }
 
    [RelayCommand]
    private async Task ConnectAsync()
    {
        await _logic.Connect(IpAdresse, Port);
 
        IsConnected = _logic.Status == "Connected";
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

            while (LogCollection.Count > MaxLogEntries)
                LogCollection.RemoveAt(LogCollection.Count - 1);

            ReceivedMessage = LogCollection.Count(l => l.Direction == "IN" && l.Topic != "-");
            SaveLogs();
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

            while (LogCollection.Count > MaxLogEntries)
                LogCollection.RemoveAt(LogCollection.Count - 1);

            ReceivedMessage++;
            SaveLogs();
        });
    }

    private void OnErrorLogged(string message)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            ErrorCount++;
            SaveLogs();
        });
    }
}
