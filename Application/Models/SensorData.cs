using System;

namespace UmweltMonitor3000.Application.Models;

public class SensorData
{
    public Guid Id { get; set; }

    public string SensorType { get; set; }

    public double Value { get; set; }

    public DateTime TimeStamp { get; set; }
}