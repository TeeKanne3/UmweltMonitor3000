using Newtonsoft.Json;

namespace UmweltMonitor3000.Application.Models;

public class SensorData
{
    public Guid Id { get; set; }
    
    public string Topic { get; set; }

    public string SensorType { get; set; }

    public double Value { get; set; }

    public DateTime TimeStamp { get; set; }

    public static SensorData? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<SensorData>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}