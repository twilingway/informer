using System.Drawing;
using System.Windows.Forms;
using Informer;
using Informer.Sensors;

public class TriggerOnForm
{
    private TimableTrigger _trigger;
    private Label _statusLabel;
    private Label _counterLabel;
    private Label _textLabel;

    public TriggerOnForm(Label textLabel, TimableTrigger trigger, Label statusLabel, Label counterLabel)
    {
        _trigger = trigger;
        _statusLabel = statusLabel;
        _counterLabel = counterLabel;
        _textLabel = textLabel;
    }

    public void UpdateLables()
    {
        if (_trigger.Sensor is MultiplyHardwareRangeSensor) {

            var ohmSensor = ((MultiplyHardwareRangeSensor)_trigger.Sensor);

            _textLabel.Text = string.Format("{0} {1} {2} ({3})", 
                                             ohmSensor.SensorName.Replace("GPU Core", "").Replace("GPU", " ").ToUpper(), 
                                             ohmSensor.Type.ToString().ToUpper(),
                                             ohmSensor.CurrentPredicate.ToString().ToUpper(),
                                             ohmSensor.TresholdValue().ToString());
        }

        switch (_trigger.CurrentState)
        {
            case TimableTrigger.State.Wait:
                _statusLabel.Text = MyStrings.labelOK;
                _statusLabel.ForeColor = Color.Green;
                _counterLabel.Visible = false;
                break;
            case TimableTrigger.State.TimeCount:
                _statusLabel.Text = MyStrings.labelAlert;
                _statusLabel.ForeColor = Color.Red;
                _counterLabel.Visible = true;
                _counterLabel.Text = _trigger.MillesecondsFromTriggered.ToString();
                _counterLabel.ForeColor = Color.Red;
                break;
            case TimableTrigger.State.Unactive:
                _statusLabel.Text = MyStrings.labelNotTracked;
                _statusLabel.ForeColor = Color.Blue;
                _counterLabel.Visible = false;
                break;
        }
    }
}
