using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer.Sensors
{
    public class TimableTrigger
    {
        private ITirggerAction _action;
        private DateTime _triggerTime;

        public State CurrentState { get; private set; }
        public double MillesecondsFromTriggered {
            get
            {
                return (_triggerTime - DateTime.Now).TotalMilliseconds;
            }
        }
        public float WaitTimeInMilleseconds
        {
            get;
            set;
        }
        public ISensor Sensor { get; private set; }

        public TimableTrigger(ISensor sensor, ITirggerAction action, float waitTimeInMilleseconds)
        {
            Sensor = sensor;
            _action = action;
            WaitTimeInMilleseconds = waitTimeInMilleseconds;
        }

        public void Update()
        {
            switch (CurrentState)
            {
                case State.Wait:
                    if (Sensor.Check())
                    {
                        CurrentState = State.TimeCount;
                        _triggerTime = DateTime.Now;
                    }
                    break;
                case State.TimeCount:
                    if(MillesecondsFromTriggered >= WaitTimeInMilleseconds)
                    {
                        _action.Action();
                        CurrentState = State.Wait;
                    }
                    break;
            }
        }

        public void Enable()
        {
            CurrentState = State.Wait;
        }

        public void Disable()
        {
            CurrentState = State.Unactive;
        }

        public enum State
        {
            Wait,
            TimeCount,
            Unactive
        }
    }
}
