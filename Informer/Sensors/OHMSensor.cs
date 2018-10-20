using OpenHardwareMonitor.Hardware;

public abstract class OHMSensor : ISensor
{
    public int[] Sensors { get; private set; }

    public OHMSensor(string sensorName, SensorType type)
    {
        SensorName = sensorName;
        Type = type;
    }

    public string SensorName { get; private set; }
    public SensorType Type { get; private set; }

    public void UpdateSensors(int[] sensors)
    {
        Sensors = sensors;
    }

    public abstract bool Check();
}
