using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;

namespace Informer
{
    class SensorForDanger
    {
        private Danger _danger;
        private List<int> _hardwers = new List<int>();

        public SensorForDanger(Danger danger)
        {
            _danger = danger;
        }

        public bool IsTargetSensor(string name, SensorType type)
        {
            return _danger.SensorName == name && 
                _danger.Type == type;
        }

        public void AddSensor(int value)
        {
            _hardwers.Add(value);
        }

        public void UpdateValue()
        {
            _danger.UpdateSensors(_hardwers.ToArray());
        }
    }
}
        
    
