using System.IO;
using System.Text.Json;
using UmweltMonitor3000.Application.Interfaces;
using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.Repositories;

public class UmweltMonitorRepository : IUmweltMonitorRepository
{
    private readonly string _jsonFilePath = $"sensor_data.json";

    public event Action<string>? LogMessage;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public async Task SaveSensorDataAsync(string sensorId, string data)
    {
        var sensorData = ParseSensorData(sensorId, data);

        if (sensorData == null)
            return;

        var allData = await LoadAllAsync();

        allData.Add(sensorData);

        await SaveAllAsync(allData);
    }
    public async Task<string> GetSensorDataAsync(string sensorId)
    {
        var allData = await LoadAllAsync();

        var result = allData
            .Where(x => x.SensorType == sensorId)
            .OrderByDescending(x => x.TimeStamp)
            .FirstOrDefault();

        return result != null
            ? JsonSerializer.Serialize(result, _options)
            : string.Empty;
    }

    private async Task<List<SensorData>> LoadAllAsync()
    {
        if (!File.Exists(_jsonFilePath))
            return new List<SensorData>();

        var json = await File.ReadAllTextAsync(_jsonFilePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<SensorData>();

        return JsonSerializer.Deserialize<List<SensorData>>(json) ?? new List<SensorData>();
    }

    private async Task SaveAllAsync(List<SensorData> data)
    {
        var json = JsonSerializer.Serialize(data, _options);
        await File.WriteAllTextAsync(_jsonFilePath, json);
    }

    private SensorData? ParseSensorData(string sensorId, string payload)
    {
        double value = 0;

        if (double.TryParse(payload.Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var parsedValue))
        {
            value = parsedValue;
        }
        else if (TryParseCsvFirstValue(payload, out var csvValue))
        {
            value = csvValue;
        }
        else
        {
            try
            {
                var doc = JsonDocument.Parse(payload);
                value = doc.RootElement.GetProperty("value").GetDouble();
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error parsing sensor data for sensor '{sensorId}': {ex.Message} (Payload: {payload})");
                return null;
            }
        }

        return new SensorData
        {
            Id = Guid.NewGuid(),
            SensorType = sensorId,
            Value = value,
            TimeStamp = DateTime.UtcNow,
        };
    }

    private static bool TryParseCsvFirstValue(string payload, out double result)
    {
        result = 0;
        var parts = payload.Split(',');
        if (parts.Length < 2) return false;
        return double.TryParse(parts[0].Trim().TrimEnd('%'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out result);
    }
}