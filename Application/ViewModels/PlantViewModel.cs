using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class PlantViewModel : ObservableObject
{
    private const string PlantsFilePath = "plants.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly object _saveLock = new();
    private readonly MainWindowLogic _logic;

    [ObservableProperty]
    private ObservableCollection<Plant> _plantCollection = new();

    [ObservableProperty]
    private string _newPlantName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNameError))]
    private string _nameErrorMessage = string.Empty;

    public bool HasNameError => !string.IsNullOrEmpty(NameErrorMessage);

    public PlantViewModel(MainWindowLogic logic)
    {
        _logic = logic;
        _logic.MessageLogged += OnSensorDataReceived;
        LoadPlants();
    }

    private void LoadPlants()
    {
        if (!File.Exists(PlantsFilePath)) return;
        try
        {
            var json = File.ReadAllText(PlantsFilePath);
            var plants = JsonSerializer.Deserialize<List<Plant>>(json, _jsonOptions);
            if (plants == null) return;

            foreach (var plant in plants)
                PlantCollection.Add(plant);
        }
        catch (Exception ex)
        {
            _logic.LogError($"Fehler beim Laden der Pflanzen: {ex.Message}");
        }
    }

    private void SavePlants()
    {
        var snapshot = PlantCollection.ToList();
        _ = Task.Run(() =>
        {
            lock (_saveLock)
            {
                try { File.WriteAllText(PlantsFilePath, JsonSerializer.Serialize(snapshot, _jsonOptions)); }
                catch (Exception ex) { _logic.LogError($"Fehler beim Speichern der Pflanzen: {ex.Message}"); }
            }
        });
    }

    [RelayCommand]
    private void AddPlant()
    {
        if (string.IsNullOrWhiteSpace(NewPlantName)) return;

        var trimmed = NewPlantName.Trim();

        if (PlantCollection.Any(p => string.Equals(p.Name, trimmed, StringComparison.OrdinalIgnoreCase)))
        {
            NameErrorMessage = $"'{trimmed}' existiert bereits.";
            return;
        }

        NameErrorMessage = string.Empty;
        var nextId = PlantCollection.Count > 0 ? PlantCollection.Max(p => p.PlantID) + 1 : 1;
        PlantCollection.Add(new Plant
        {
            PlantID = nextId,
            Name = trimmed,
            LastUpdate = DateTime.Now
        });
        NewPlantName = string.Empty;
        SavePlants();
    }

    [RelayCommand]
    private void RemovePlant(Plant plant)
    {
        PlantCollection.Remove(plant);
        SavePlants();
    }

    private static bool TryParseJson(string payload, Plant plant)
    {
        try
        {
            var root = JsonDocument.Parse(payload).RootElement;
            if (root.TryGetProperty("moistureSensor", out var ms)) plant.MoistureSensor = ms.GetInt32();
            if (root.TryGetProperty("moisturePercent", out var mp)) plant.MoisturePercent = mp.GetInt32();
            if (root.TryGetProperty("battery", out var bat)) plant.BatteryState = bat.GetInt32();
            return true;
        }
        catch { return false; }
    }

    private static bool TryParseCsv(string payload, Plant plant)
    {
        var parts = payload.Split(',');
        if (parts.Length < 2) return false;

        static bool TryParseClean(string s, out double result) =>
            double.TryParse(s.Trim().TrimEnd('%'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out result);

        if (!TryParseClean(parts[0], out var sensor)) return false;
        plant.MoistureSensor = (int)sensor;

        if (parts.Length >= 2 && TryParseClean(parts[1], out var percent))
            plant.MoisturePercent = (int)percent;
        if (parts.Length >= 3 && TryParseClean(parts[2], out var battery))
            plant.BatteryState = (int)battery;

        return true;
    }

    private static bool TryParseSingleValue(string payload, Plant plant)
    {
        if (!double.TryParse(payload.Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val)) return false;
        plant.MoistureSensor = (int)val;
        return true;
    }

    private void OnSensorDataReceived(string topic, string payload)
    {
        var topicSegment = topic.Split('/').Last();

        var plant = PlantCollection.FirstOrDefault(p =>
            string.Equals(p.Name, topicSegment, StringComparison.OrdinalIgnoreCase));

        if (plant == null) return;

        App.Current.Dispatcher.Invoke(() =>
        {
            if (TryParseJson(payload, plant) || TryParseCsv(payload, plant) || TryParseSingleValue(payload, plant))
            {
                plant.LastUpdate = DateTime.Now;
                SavePlants();
            }
            else
            {
                _logic.LogError($"Unbekanntes Payload-Format von '{topic}': {payload}");
            }
        });
    }
}
