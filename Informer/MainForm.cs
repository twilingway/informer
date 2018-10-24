using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
using System.Net.NetworkInformation;
using MQTTnet;
using System.Globalization;
using System.Management;
using System.Management.Instrumentation;
using Informer;
using Informer.Sensors;

namespace Informer
{
    public partial class MainForm : Form
    {
        private static List<string> hosts = new List<string>();
        private static Http _http;
        private LogFile _log, _error;
        private GlobalVars globalVars;
        public ApiResponse apiResponse, response;
        public Computer PC;
        public MqttConnect mqttConnect;
        public CommandProcesser commandProcesser;

        private OHMMonitoringSystem _monitoringSystem;
        private List<TriggerOnForm> _triggersOnForm = new List<TriggerOnForm>();

        public MainForm()
        {
            InitializeComponent();
            globalVars = new GlobalVars();
            PC = new Computer();

           // PC.CPUEnabled = true;
            PC.GPUEnabled = true;
            PC.Open();

            _log = new LogFile("log");
            _error = new LogFile("error");
            _http = new Http();
            apiResponse = new ApiResponse();

            KillDublicateProcess("Informer");
            KillDublicateProcess("Launcher_informer");

            apiResponse.Params = new Params
            {
                Timers = new Timers(),
                Reboots = new Reboots(),
                Data_ranges = new Data_ranges(),
                Version = "1.3.9"
            };
            
            СheckForNewVersion();
            response = apiResponse.Load();

            //костыли 
            apiResponse = response;
            //


            bool start = false;
      
            if (!string.IsNullOrEmpty(response.Params.Token))
            {
                start = true;
                tbRigName.ReadOnly = true;
                tbToken.ReadOnly = true;
                tbRigName.Text = response.Params.Name;
                tbToken.Text = response.Params.Token;
            }

            else
            {
                start = false;
                tbRigName.ReadOnly = true;
            }
           
            if (start)
            {
                NextAutoStart.Interval = globalVars.autostart * 1000;
                NextAutoStart.Enabled = true;
                AutoStartTimer.Enabled = true;
                TimeWorkTimer.Enabled = true;
            }

            commandProcesser = new CommandProcesser(response);
            mqttConnect = new MqttConnect();

            _monitoringSystem = new OHMMonitoringSystem();
            var tempMin = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Temperature, MultiplyHardwareRangeSensor.Predicate.Min),
                                           new RebootTriggerAction("Temp Min, Reboot!", "reboot_t_min.bat", "token", "rigName", globalVars.host),300);
            var tempMax = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Temperature, MultiplyHardwareRangeSensor.Predicate.Max),
                                           new RebootTriggerAction("Temp Max, Reboot!", "reboot_t_max.bat", "token", "rigName", globalVars.host), 300);

            var memoryMin = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Memory", SensorType.Clock, MultiplyHardwareRangeSensor.Predicate.Min),
                                           new RebootTriggerAction("Memory Min, Reboot!", "reboot_mem_min.bat", "token", "rigName", globalVars.host), 300);
            var memoryMax = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Memory", SensorType.Clock, MultiplyHardwareRangeSensor.Predicate.Max),
            new RebootTriggerAction("Memory Max, Reboot!", "reboot_mem_max.bat", "token", "rigName", globalVars.host), 300);

            var loadMin = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Load, MultiplyHardwareRangeSensor.Predicate.Min),
                                          new RebootTriggerAction("Load Min, Reboot!", "reboot_load_min.bat", "token", "rigName", globalVars.host), 300);
            var loadMax = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Load, MultiplyHardwareRangeSensor.Predicate.Max),
                                          new RebootTriggerAction("Load Max, Reboot!", "reboot_load_max.bat", "token", "rigName", globalVars.host), 300);
            
            var fanMin = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Fan", SensorType.Control, MultiplyHardwareRangeSensor.Predicate.Min),
                                           new RebootTriggerAction("Fan Min, Reboot!", "reboot_fan_min.bat", "token", "rigName", globalVars.host), 300);
            var fanMax = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Fan", SensorType.Control, MultiplyHardwareRangeSensor.Predicate.Max),
            new RebootTriggerAction("Fan Max, Reboot!", "reboot_fan_max.bat", "token", "rigName", globalVars.host), 300);

            var clockMin = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Clock, MultiplyHardwareRangeSensor.Predicate.Min),
                                                      new RebootTriggerAction("Clock Min, Reboot!", "reboot_clock_min.bat", "token", "rigName", globalVars.host), 300);
            var clockMax = _monitoringSystem.BuildTrigger(new MultiplyHardwareRangeSensor("GPU Core", SensorType.Clock, MultiplyHardwareRangeSensor.Predicate.Max),
                                          new RebootTriggerAction("Clock Max, Reboot!", "reboot_clock_max.bat", "token", "rigName", globalVars.host), 300);



            var tempMinOnForm = new TriggerOnForm(labelTempMin, tempMin, labelStatusTempMin, labelCounterTempMin);
            var tempMaxOnForm = new TriggerOnForm(labelTempMax, tempMax, labelStatusTempMax, labelCounterTempMax);
            var memoryMinOnForm = new TriggerOnForm(labelMemoryMin, memoryMin, labelStatusMemoryMin, labelCounterMemoryMin);
            var memoryMaxOnForm = new TriggerOnForm(labelMemoryMax, memoryMax, labelStatusMemoryMax, labelCounterMemoryMax);
            var loadMinOnForm = new TriggerOnForm(labelLoadMin, loadMin, labelStatusLoadMin, labelCounterLoadMin);
            var loadMaxOnForm = new TriggerOnForm(labelLoadMax, loadMax, labelStatusLoadMax, labelCounterLoadMax);
            var fanMinOnForm = new TriggerOnForm(labelFanMin, fanMin, labelStatusFanMin, labelCounterFanMin);
            var fanMaxOnForm = new TriggerOnForm(labelFanMax, fanMax, labelStatusFanMax, labelCounterFanMax);
            var clockMinOnForm = new TriggerOnForm(labelClockMin, clockMin, labelStatusClockMin, labelCounterClockMin);
            var clockMaxOnForm = new TriggerOnForm(labelClockMax, clockMax, labelStatusClockMax, labelCounterClockMax);


            _triggersOnForm.Add(tempMinOnForm);
            _triggersOnForm.Add(tempMaxOnForm);
            _triggersOnForm.Add(memoryMinOnForm);
            _triggersOnForm.Add(memoryMaxOnForm);
           //вместо lost inet и lost gpu
            _triggersOnForm.Add(memoryMinOnForm);
            _triggersOnForm.Add(memoryMaxOnForm);
            //
            _triggersOnForm.Add(loadMinOnForm);
            _triggersOnForm.Add(loadMaxOnForm);
            _triggersOnForm.Add(fanMinOnForm);
            _triggersOnForm.Add(fanMaxOnForm);
            _triggersOnForm.Add(clockMinOnForm);
            _triggersOnForm.Add(clockMaxOnForm);


            //LabelOnForm tempMinLabel, tempMaxLabel, fanMinLabel, fanMaxLabel, loadMinLabel, loadMaxLabel,
            //               clockMinLabel, clockMaxLabel, memoryMinLabel, memoryMaxLabel;
            //Sensor tempMin, tempMax, fanMin, fanMax, loadMin, loadMax, clockMin, clockMax, memoryMin, memoryMax, internetOff;

            //tempMinLabel = new LabelOnForm(labelStatusTempMin, labelCounterTempMin, response, "Temp Min, Reboot!", "reboot_t_min.bat", reboot);
            //tempMaxLabel = new LabelOnForm(labelStatusTempMax, labelCounterTempMax, response, "Temp Max, Reboot!", "reboot_t_max.bat", reboot);
            //fanMinLabel = new LabelOnForm(labelStatusFanMin, labelCounterFanMin, response, "Fan Min, Reboot!", "reboot_fan_min.bat", reboot);
            //fanMaxLabel = new LabelOnForm(labelStatusFanMax, labelCounterFanMax, response, "Fan Max, Reboot!", "reboot_fan_max.bat", reboot);
            //loadMinLabel = new LabelOnForm(labelStatusLoadMin, labelCounterLoadMin, response, "Load Min, Reboot!", "reboot_load_min.bat", reboot);
            //loadMaxLabel = new LabelOnForm(labelStatusLoadMax, labelCounterLoadMax, response, "Load Max, Reboot!", "reboot_load_max.bat", reboot);
            //clockMinLabel = new LabelOnForm(labelStatusClockMin, labelCounterClockMin, response, "Clock Min, Reboot!", "reboot_clock_min.bat", reboot);
            //clockMaxLabel = new LabelOnForm(labelStatusClockMax, labelCounterClockMax, response, "Clock Max, Reboot!", "reboot_clock_max.bat", reboot);
            //memoryMinLabel = new LabelOnForm(labelStatusMemoryMin, labelCounterMemoryMin, response, "Memory Min, Reboot!", "reboot_mem_min.bat", reboot);
            //memoryMaxLabel = new LabelOnForm(labelStatusMemoryMax, labelCounterMemoryMax, response, "Memory Max, Reboot!", "reboot_memory_max.bat", reboot);
            //NotInternetLabel = new LabelOnForm(labelStatusInternet, labelCounterInternet, response, "Dont Have Internet", "reboot_internet.bat", reboot);

            //tempMin = new Sensor(tempMinLabel, Sensor.Predicate.Min, "GPU Core", SensorType.Temperature);
            //tempMax = new Sensor(tempMaxLabel, Sensor.Predicate.Max, "GPU Core", SensorType.Temperature);
            //fanMin = new Sensor(fanMinLabel, Sensor.Predicate.Min, "GPU Fan", SensorType.Control);
            //fanMax = new Sensor(fanMaxLabel, Sensor.Predicate.Max, "GPU Fan",SensorType.Control);
            //loadMin = new Sensor(loadMinLabel, Sensor.Predicate.Min, "GPU Core", SensorType.Load);
            //loadMax = new Sensor(loadMaxLabel, Sensor.Predicate.Max, "GPU Core", SensorType.Load);
            //clockMin = new Sensor(clockMinLabel,Sensor.Predicate.Min, "GPU Core",SensorType.Clock);
            //clockMax = new Sensor(clockMaxLabel,Sensor.Predicate.Max, "GPU Core",SensorType.Clock);
            //memoryMin = new Sensor(memoryMinLabel,Sensor.Predicate.Min,"GPU Memory",SensorType.Clock);
            //memoryMax = new Sensor(memoryMaxLabel,Sensor.Predicate.Max,"GPU Memory",SensorType.Clock);

            //dangers = new Sensor[] {
            //    tempMin, tempMax, fanMin, fanMax, loadMin,loadMax,
            //    clockMin,clockMax, memoryMin, memoryMax
            //};
        }

        private void KillDublicateProcess(string processName)
        {
            try
            {
                int myId = Process.GetCurrentProcess().Id;
                Process.GetProcesses()
                    .Where(p => p.ProcessName.ToLower() == processName.ToLower() && p.Id != myId)
                    .Count(p => { p.Kill(); return true; });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"KILL {processName}: " + e);
            }
        }

        // нужно разобраться с автостартом
        async private void GetTempretureTimerTick(object sender, EventArgs e)
        {

            if (globalVars.mqttIsConnect == true)
            {
                btStart.Enabled = false;
                btStop.Visible = true;

                AutoStartTimer.Enabled = false;

                tbToken.ReadOnly = true;
                tbRigName.Text = response.Params.Name;
            }

            try
            {
                btStart.Enabled = false;
                NextAutoStart.Enabled = false;
                AutoStartTimer.Enabled = false;
                btStop.Visible = true;
                GPUParams.GetGPU(globalVars, PC);
            }
            catch (Exception ex)
            {
                _error.writeLogLine("GetTempTimer: " + ex.Message, "error");
            }

            await Task.Delay(1000);
        }
        
        private void NextAutoStart_Tick(object sender, EventArgs e)
        {
            globalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            globalVars.token = tbToken.Text;
            SendDataTimer.Enabled = true;
            GPUStatusTimer.Enabled = true;
            GetTempretureTimer.Enabled = true;
            PingTimer.Enabled = true;
            AutoStartTimer.Enabled = false;
            btStop.Visible = true;
            btStart.Enabled = false;
            tbRigName.ReadOnly = true;
            tbToken.ReadOnly = true;
          
            if (!string.IsNullOrWhiteSpace(response.Params.Name))
            {
                Message("Informer Started!", globalVars, response);
            }

            MqttConnectTimer.Enabled = true;
            InformationLabel.Visible = true;
            InformationLabel.Text = MyStrings.labelStatusStarted;
            InformationLabel.ForeColor = Color.Green;
            Hide();
        }

        private void AutoStart_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(globalVars.token))
            {
                tbToken.ReadOnly = true;
                NextAutoStart.Start();
                AutoStartTimer.Start();
                globalVars.autostart = globalVars.autostart - 1;
                btStart.Text = MyStrings.btStart + "(" + globalVars.autostart.ToString() + ")";

            }
            else
            {

                NextAutoStart.Start();
                AutoStartTimer.Start();
                globalVars.autostart = globalVars.autostart - 1;
                btStart.Text = MyStrings.btStart + "(" + globalVars.autostart.ToString() + ")";
            }

        }

        //
        public void GpuStatus()
        {
            try
            {
                foreach (var triggerOnForm in _triggersOnForm)
                {
                    triggerOnForm.UpdateLables();
                }

                //autorization
                if (globalVars.mqttIsConnect == false && globalVars.firsrun == false)
                {
                    InformationLabel.Text = MyStrings.labelInformationAuthorizationFailed;
                    InformationLabel.ForeColor = Color.Red;
                }
                else if (globalVars.mqttIsConnect == true && globalVars.firsrun == false)
                {
                    InformationLabel.Text = MyStrings.labelInformationAuthorizationOK;
                    InformationLabel.ForeColor = Color.Green;
                }


            }
            catch (Exception e)
            {

                Debug.WriteLine("GpuStatus: " + e);
            }

        }

        public void СheckForNewVersion()
        {
            try
            {
                string v = response.Params.Version;
                string pack = _http.GetContent(globalVars.host + "/api/?method=version");
                VersionResponse m = JsonConvert.DeserializeObject<VersionResponse>(pack);
                string ver = m.version;
                globalVars.link = m.link;

                if (v == ver)
                {
                    linkLabelUpdate.Visible = false;
                }
                else
                {
                    linkLabelUpdate.Visible = true;
                    linkLabelUpdate.Text = "Upgrade to v" + ver;
                    try
                    {
                        Process.Start("launcher_informer.exe");
                    }
                    catch (Exception ex)
                    {
                        _error.writeLogLine("Start launcher " + ex.Message, "error");
                    }
                }
            }

            catch (Exception ex)
            {
                _error.writeLogLine("CheckNewVersion " + ex.Message, "error");
            }
        }

        private void CheckNewVersionTimerTick(object sender, EventArgs e)
        {
            СheckForNewVersion();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            СheckForNewVersion();
        }

        async private void UptimeTimerTick(object sender, EventArgs e)
        {
            globalVars.timeOnline = globalVars.timeOnline + 1;
            int min = globalVars.timeOnline;
            int d = min / 1440;
            min = min - (d * 1440);
            int h = min / 60;
            min = min - (h * 60);

            labelTimeWork2.Text = ("Days: " + d.ToString() + " Hours: " + h.ToString() + " Minute: " + min.ToString());
            await Task.Delay(1);

        }

        public async Task SendData(GlobalVars globalVars, ApiResponse apiResponse)
        {

            var triggers = _monitoringSystem.GetTriggers();

            if (globalVars.mqttIsConnect == true)
            {
                try
                {
                    globalVars.upTime = GetUptime().ToString(@"dd\.hh\:mm\:ss");
                    var send_data = new MqttApplicationMessageBuilder()
                         .WithTopic("devices/" + apiResponse.Params.Token + "/data")
                         .WithPayload("token=" + apiResponse.Params.Token +
                            "&gpu=" + globalVars.card +
                            "&temp=" + string.Join(",", ((OHMSensor)triggers[0].Sensor).Sensors) +
                            "&fan=" + globalVars.fan +
                            "&start_timestamp=" + globalVars.start_timestamp.ToString() +
                            "&v=" + apiResponse.Params.Version +
                            "&load=" + globalVars.load +
                            "&clock=" + globalVars.clock +
                            "&mem=" + globalVars.mem +
                            "&upTime=" + globalVars.upTime)
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                    await globalVars.client.PublishAsync(send_data);
                }
                catch (MQTTnet.Exceptions.MqttCommunicationException ex)
                {
                    // MqttConnect();
                    Debug.WriteLine("Send data MqttCommunicationException: " + ex.Message);
                    Message("Send data MqttCommunicationException: " + ex.Message, globalVars, apiResponse);
                }
                catch (Exception ex)
                {
                    Message("Send data Ex: " + ex.Message, globalVars, apiResponse);
                }

            }
        }
        //send event message

        public static void Message(string msg, GlobalVars globalVars, ApiResponse apiResponse)
        {

            try
            {
                _http.GetContent(
                    globalVars.host +
                    "/api.php?token=" + apiResponse.Params.Token +
                    "&event=" + "message" +
                    "&reason=" + apiResponse.Params.Name + " " + msg
                    );
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Send Message: " + ex.Message);
            }


        }

        private void BtStartClick(object sender, EventArgs e)
        {

            globalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;


            if (!string.IsNullOrWhiteSpace(tbToken.Text))
            {
                MqttConnectTimer.Enabled = true;
                GetTempretureTimer.Enabled = true;
                GPUStatusTimer.Enabled = true;
                SendDataTimer.Enabled = true;
                response.Params.Token = tbToken.Text;
                tbToken.ReadOnly = true;

               
                PingTimer.Enabled = true;


                if (!string.IsNullOrWhiteSpace(response.Params.Name))
                {
                    Message("Informer Started!", globalVars, response);
                }

                InformationLabel.Visible = true;
                InformationLabel.Text = MyStrings.labelStatusStarted;
                InformationLabel.ForeColor = Color.Green;
            }

            else
            {
                MessageBox.Show("Enter the token!");
            }
        }

        private void BtStopClick(object sender, EventArgs e)
        {
            _log.writeLogLine("Informer stopped", "log");
            Message("Informer Stopped!", globalVars, response);

            globalVars.firsrun = true;

            GPUStatusTimer.Enabled = false;
            GetTempretureTimer.Enabled = false;
            PingTimer.Enabled = false;
            MqttConnectTimer.Enabled = false;
            globalVars.mqttIsConnect = false;

            AutoStartTimer.Enabled = false;
            AutoStartTimer.Stop();

            NextAutoStart.Stop();
            btStop.Visible = false;
            btStart.Enabled = true;
            SendDataTimer.Enabled = false;
           
           
            InformationLabel.Text = MyStrings.labelInformationStop;
            InformationLabel.ForeColor = Color.Gray;
           
            tbToken.ReadOnly = false;
        }

        private void BtnExitClick(object sender, EventArgs e)
        {
            if (MessageBox.Show(MyStrings.ExitRequest, MyStrings.ExitTitle, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.ShowInTaskbar = false;
                Application.Exit();
                this.Close();
            }
        }

        //SendData
        private async void SendDataTimerTick(object sender, EventArgs e)
        {
            if (globalVars.mqttIsConnect == true)
            {
                if (globalVars.interval > 0)
                {
                    SendDataTimer.Interval = globalVars.interval * 1000;
                }
                //   Debug.WriteLine("Interval: " + SendDataTimer.Interval);
                await SendData(globalVars, response);
            }

        }

        //timer dont have Internet
        async private void InternetInactiveTimerTick(object sender, EventArgs e)
        {
            //const string bat = "reboot_internet.bat";

            //    if (NotInternetLabel.TimeReboot <= 0)
            //    {
            //      Process.Start(bat);
            //    }

            //NotInternetLabel.TimeReboot -=1;
            await Task.Delay(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Properties.Settings.Default.Language = cbLocalize.SelectedValue.ToString();
                Properties.Settings.Default.Save();
                this.ShowInTaskbar = false;
                Application.Exit();
            }

            catch (Exception ex)
            {
                _error.writeLogLine("MainFormClosing:" + ex.Message, "error");
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // _log.writeLogLine("Informer is started ","log");
            cbLocalize.DataSource = new System.Globalization.CultureInfo[] {
                System.Globalization.CultureInfo.GetCultureInfo("ru-RU"),
                System.Globalization.CultureInfo.GetCultureInfo("en-US")
            };

            cbLocalize.DisplayMember = "NativeName";
            cbLocalize.ValueMember = "Name";

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                cbLocalize.SelectedValue = Properties.Settings.Default.Language;
            }
        }

        // chek ping timer
        private void PingTimer_Tick(object sender, EventArgs e)
        {         
            //// В переменную hosts записываем все рабочие станции из файла
            //hosts = getComputersListFromTxtFile("pinglist.txt");

            //if (globalVars.pingCount >= hosts.Count)
            //{
            //    checkPing = false;
            //}
            //else if (globalVars.pingCount < hosts.Count)
            //{
            //    checkPing = true;
            //}
            //// Создаём Action типизированный string, данный Action будет запускать функцию Pinger

            //Action<string> asyn = new Action<string>(Pinger);

            //hosts.ForEach(p =>
            //{
            //    asyn.Invoke(p);

            //}
            
            //);
            //globalVars.pingCount = hosts.Count;
           
        }

        // получаем из файла список серверов для ping
        private static List<string> getComputersListFromTxtFile(string pathToFile)
        {
            List<string> computersList = new List<string>();
            try
            {

                using (StreamReader sr = new StreamReader(pathToFile, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        computersList.Add(line);
                    }
                }
                return computersList;
            }
            catch (FileNotFoundException e)
            {
                computersList.Add("google.com");
                return computersList;

            }
            catch (Exception e)
            {
                computersList.Add("google.com");
                return computersList;
            }
        }

        // Наш основной класс, который будет отправлять команду ping
        private async void Pinger(string hostAdress)
        {
            // Создаём экземпляр класса Ping
            Ping png = new Ping();

            try
            {
                // Пингуем рабочую станцию hostAdress
                PingReply pr = await png.SendPingAsync(hostAdress);

                // то такую машину заносим в список
                if (pr.Status != IPStatus.Success)
                {
                  //  globalVars.pingCount = globalVars.pingCount + 1;
                 //   Debug.WriteLine("DONT Success");
                   
                }

                else if (pr.Status == IPStatus.Success)
                {
                    globalVars.pingCount -=1;
               //     Debug.WriteLine("Success");
                }

                // Записываем в файл все проблемные машины
                //  writeProblemComputersToFile("D:\\problemsWithAdminShare.txt", problemComputersList);
            }
            catch (Exception e)
            {
                globalVars.pingCount = globalVars.pingCount + 1;
                Debug.WriteLine("Возникла ошибка! " + hostAdress + " " + globalVars.pingCount + " Ex " + e.Message);
                globalVars.ping = false;
            }

        }

        async void MqttConnectTimer_Tick(object sender, EventArgs e)
        {
            await mqttConnect.RunAsync(globalVars, response,commandProcesser);
        }

        async private void GPUStatusTimer_Tick(object sender, EventArgs e)   
        {
            response = commandProcesser.GetApiResponse();
            GpuStatus();

            response.Params.Update(_monitoringSystem);
            _monitoringSystem.Update();

            await Task.Delay(1);     
        }

        public static TimeSpan GetUptime()
        {
            ManagementObject mo = new ManagementObject(@"\\.\root\cimv2:Win32_OperatingSystem=@");
            DateTime lastBootUp = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
            return DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime();
        }
    }

}
