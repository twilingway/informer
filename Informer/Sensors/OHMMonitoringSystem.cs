using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer.Sensors
{
    public class OHMMonitoringSystem
    {
        private List<TimableTrigger> _triggers = new List<TimableTrigger>();
        private List<OHMSensor> _sensors = new List<OHMSensor>();
        private GPUParams _gpuParams;

        public OHMMonitoringSystem()
        {
            Computer PC = new Computer();
            PC.GPUEnabled = true;
            PC.Open();

            _gpuParams = new GPUParams(PC);
        }

        public TimableTrigger BuildTrigger(OHMSensor sensor, ITirggerAction action, float waitTimeInMilleseconds)
        {
            var trigger = new TimableTrigger(sensor, action, waitTimeInMilleseconds);
            _triggers.Add(trigger);
            _sensors.Add(sensor);

            return trigger;
        }

        public void Update()
        {
            UpdateSensors();
            CheckTriggers();
        }

        private void UpdateSensors()
        {
            _gpuParams.UpdateParams(_sensors.Select(x => new SensorForDanger(x)).ToArray());
        }

        private void CheckTriggers()
        {
            foreach (var trigger in _triggers)
            {
                trigger.Update();
            }
        }

        public List<TimableTrigger> GetTriggers()
        {
            return _triggers;
        }
    }
}
