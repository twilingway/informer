using OpenHardwareMonitor.Hardware;
using System.Linq;

public class MultiplyHardwareRangeSensor : OHMSensor
{
    private int[] _range;
    public Predicate CurrentPredicate { get; private set; }

    public MultiplyHardwareRangeSensor(string sensorName, SensorType type, Predicate predicate) : base(sensorName, type)
    {
        CurrentPredicate = predicate;
    }

    public override bool Check()
    {
        int warningCount = Sensors.Count(sensor => CheckWarning(sensor));

        return warningCount > 0;
    }

    public void UpdateRange(int[] range)
    {
        _range = range;
    }

    public int TresholdValue()
    {
        switch (CurrentPredicate)
        {
            case Predicate.Min:
                return _range[0];
            case Predicate.Max:
                return _range[1];
        }
        return 0;
    }

    private bool CheckWarning(int sensor)
    {
        switch (CurrentPredicate)
        {
            case Predicate.Min:
                return sensor < _range[0];
            case Predicate.Max:
                return sensor > _range[1];
            default:
                return false;
        }
    }

    public enum Predicate
    {
        Min,
        Max
    }
}
