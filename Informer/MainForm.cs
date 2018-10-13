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

namespace Informer
{
    public partial class MainForm : Form
    {
        private static List<string> hosts = new List<string>();
        private static Http _http;
        private LogFile _log, _error;
        private GlobalVars globalVars;
        public ApiResponse apiResponse ,response;
        public Computer PC;
        public MqttConnect mqttConnect;
        public CommandProcesser commandProcesser;

        GPUParams gpuParams;

        LabelOnForm tempMinLabel, tempMaxLabel, fanMinLabel,fanMaxLabel,loadMinLabel, loadMaxLabel,
                    clockMinLabel, clockMaxLabel,memoryMinLabel, memoryMaxLabel, NotInternetLabel;
        Danger tempMin,tempMax, fanMin,fanMax, loadMin,loadMax, clockMin,clockMax, memoryMin, memoryMax, internetOff;
        Danger[] dangers;
        Reboot reboot;

        bool checkPing;
        public MainForm()
        {
            InitializeComponent();
            globalVars = new GlobalVars();
            PC = new Computer();

           // PC.CPUEnabled = true;
            PC.GPUEnabled = true;
            PC.Open();

            gpuParams = new GPUParams(PC);

            gpuParams.SetParams();
            gpuParams.GetParams();

            _log = new LogFile("log");
            _error = new LogFile("error");
            _http = new Http();
            apiResponse = new ApiResponse();

            reboot = new Reboot(_log,_http, globalVars);
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

            tempMinLabel = new LabelOnForm(labelStatusTempMin, labelCounterTempMin, response, "Temp Min, Reboot!", "reboot_t_min.bat", reboot);
            tempMaxLabel = new LabelOnForm(labelStatusTempMax, labelCounterTempMax, response, "Temp Max, Reboot!", "reboot_t_max.bat", reboot);
            fanMinLabel = new LabelOnForm(labelStatusFanMin, labelCounterFanMin, response, "Fan Min, Reboot!", "reboot_fan_min.bat", reboot);
            fanMaxLabel = new LabelOnForm(labelStatusFanMax, labelCounterFanMax, response, "Fan Max, Reboot!", "reboot_fan_max.bat", reboot);
            loadMinLabel = new LabelOnForm(labelStatusLoadMin, labelCounterLoadMin, response, "Load Min, Reboot!", "reboot_load_min.bat", reboot);
            loadMaxLabel = new LabelOnForm(labelStatusLoadMax, labelCounterLoadMax, response, "Load Max, Reboot!", "reboot_load_max.bat", reboot);
            clockMinLabel = new LabelOnForm(labelStatusClockMin, labelCounterClockMin, response, "Clock Min, Reboot!", "reboot_clock_min.bat", reboot);
            clockMaxLabel = new LabelOnForm(labelStatusClockMax, labelCounterClockMax, response, "Clock Max, Reboot!", "reboot_clock_max.bat", reboot);
            memoryMinLabel = new LabelOnForm(labelStatusMemoryMin, labelCounterMemoryMin, response, "Memory Min, Reboot!", "reboot_mem_min.bat", reboot);
            memoryMaxLabel = new LabelOnForm(labelStatusMemoryMax, labelCounterMemoryMax, response, "Memory Max, Reboot!", "reboot_memory_max.bat", reboot);
            NotInternetLabel = new LabelOnForm(labelStatusInternet, labelCounterInternet, response, "Dont Have Internet", "reboot_internet.bat", reboot);

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
           
            tempMin = new Danger(response,response.Params.Reboots.temp_min,
                        gpuParams.Temperature,
                        response.Params.Data_ranges.Temp,
                        tempMinLabel,
                      
                        response.Params.Timers.temp_min,
                        Danger.Predicate.Min,
                        "GPU Core",
                        SensorType.Temperature,Danger.ParamsList.tempMin);

            tempMax = new Danger(response, response.Params.Reboots.temp_max,
                       gpuParams.Temperature,
                       response.Params.Data_ranges.Temp,
                       tempMaxLabel,
                      
                       response.Params.Timers.temp_max,
                       Danger.Predicate.Max,
                       "GPU Core",
                       SensorType.Temperature, Danger.ParamsList.tempMax);

            fanMin = new Danger(response, response.Params.Reboots.fan_min,
                   gpuParams.FanSpeed,
                   response.Params.Data_ranges.Fan,
                   fanMinLabel,
                  
                   response.Params.Timers.fan_min,
                   Danger.Predicate.Min,
                   "GPU Fan",
                   SensorType.Control, Danger.ParamsList.fanMin);

            fanMax = new Danger(response, response.Params.Reboots.fan_max,
                    gpuParams.FanSpeed,
                    response.Params.Data_ranges.Fan,
                    fanMaxLabel,
                  
                    response.Params.Timers.fan_max,
                    Danger.Predicate.Max,
                    "GPU Fan",
                    SensorType.Control, Danger.ParamsList.fanMax);

            loadMin = new Danger(response, response.Params.Reboots.load_min,
           gpuParams.Load,
           response.Params.Data_ranges.Load,
           loadMinLabel,
          
           response.Params.Timers.load_min,
           Danger.Predicate.Min,
           "GPU Core",
           SensorType.Load, Danger.ParamsList.loadMin);

            loadMax = new Danger(response, response.Params.Reboots.load_max,
           gpuParams.Load,
           response.Params.Data_ranges.Load,
           loadMaxLabel,
         
           response.Params.Timers.load_max,
           Danger.Predicate.Max,
           "GPU Core",
           SensorType.Load, Danger.ParamsList.loadMax);

            clockMin = new Danger(response, response.Params.Reboots.clock_min,
          gpuParams.Clock,
          response.Params.Data_ranges.Clock,
          clockMinLabel,
         
          response.Params.Timers.clock_min,
          Danger.Predicate.Min,
          "GPU Core",
          SensorType.Clock, Danger.ParamsList.clockMin);

            clockMax = new Danger(response, response.Params.Reboots.clock_max,
          gpuParams.Clock,
          response.Params.Data_ranges.Clock,
          clockMaxLabel,
         
          response.Params.Timers.clock_max,
          Danger.Predicate.Max,
          "GPU Core",
          SensorType.Clock, Danger.ParamsList.clockMax);


            memoryMin = new Danger(response, response.Params.Reboots.mem_min,
            gpuParams.Memory,
            response.Params.Data_ranges.Mem,
            memoryMinLabel,
           
            response.Params.Timers.mem_min,
            Danger.Predicate.Min,
            "GPU Memory",
            SensorType.Clock, Danger.ParamsList.memoryMin);

            memoryMax = new Danger(response, response.Params.Reboots.mem_max,
            gpuParams.Memory,
            response.Params.Data_ranges.Mem,
            memoryMaxLabel,
         
            response.Params.Timers.mem_max,
            Danger.Predicate.Max,
            "GPU Memory",
            SensorType.Clock, Danger.ParamsList.memoryMax);


            dangers = new Danger[] {
                tempMin, tempMax, fanMin, fanMax, loadMin,loadMax,
                clockMin,clockMax, memoryMin, memoryMax
            };
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
                Debug.WriteLine("TEMP MIN(" + response.Params.Data_ranges.Temp[0]);
                labelTempMin.Text = "TEMP MIN(" + response.Params.Data_ranges.Temp[0] + "):";
                labelTempMax.Text = "TEMP MAX(" + response.Params.Data_ranges.Temp[1] + "):";
                labelFanMin.Text = "FAN MIN(" + response.Params.Data_ranges.Fan[0] + "):";
                labelFanMax.Text = "FAN MAX(" + response.Params.Data_ranges.Fan[1] + "):";
                labelLoadMin.Text = "LOAD MIN(" + response.Params.Data_ranges.Load[0] + "):";
                labelLoadMax.Text = "LOAD MAX(" + response.Params.Data_ranges.Load[1] + "):";
                labelClockMin.Text = "CLOCK MIN(" + response.Params.Data_ranges.Clock[0] + "):";
                labelClockMax.Text = "CLOCK MAX(" + response.Params.Data_ranges.Clock[1] + "):";
                labelMemoryMin.Text = "MEMORY MIN(" + response.Params.Data_ranges.Mem[0] + "):";
                labelMemoryMax.Text = "MEMORY MAX(" + response.Params.Data_ranges.Mem[1] + "):";
               

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

        public static async Task SendData(GlobalVars globalVars, ApiResponse apiResponse)
        {
            
            if (globalVars.mqttIsConnect == true)
            {
                try
                {
                    globalVars.upTime = GetUptime().ToString(@"dd\.hh\:mm\:ss");
                    var send_data = new MqttApplicationMessageBuilder()
                         .WithTopic("devices/" + apiResponse.Params.Token + "/data")
                         .WithPayload("token=" + apiResponse.Params.Token +
                            "&gpu=" + globalVars.card +
                            "&temp=" + globalVars.temp +
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
            const string bat = "reboot_internet.bat";

                if (NotInternetLabel.TimeReboot <= 0)
                {
                  Process.Start(bat);
                }

            NotInternetLabel.TimeReboot -=1;
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
           
            // В переменную hosts записываем все рабочие станции из файла
            hosts = getComputersListFromTxtFile("pinglist.txt");

            if (globalVars.pingCount >= hosts.Count)
            {
                checkPing = false;
            }
            else if (globalVars.pingCount < hosts.Count)
            {
                checkPing = true;
            }
            // Создаём Action типизированный string, данный Action будет запускать функцию Pinger

            Action<string> asyn = new Action<string>(Pinger);

            hosts.ForEach(p =>
            {
                asyn.Invoke(p);

            }
            
            );
            globalVars.pingCount = hosts.Count;
           
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

            //костыли 
            apiResponse.Params.Data_ranges = response.Params.Data_ranges;
            apiResponse.Params.Reboots = response.Params.Reboots;
            apiResponse.Params.Timers = response.Params.Timers;


            GpuStatus();
            gpuParams.UpdateParams(dangers.Select(danger => new SensorForDanger(danger)).ToArray());

           
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

class Danger
{
    ApiResponse apiResponse;
    private bool paramReboot;
    private int[] sensors;
    private int[] dataRanges;
    private LabelOnForm labelOnForm;
   // private TimerOnForm timerOnForm;
    private int timers;
    private Predicate predicate;
    public string SensorName { get; private set; }
    public SensorType Type { get; private set; }
    private ParamsList paramsList;

    public Danger(ApiResponse apiResponse, bool paramReboot, int[] sensors, int[] dataRanges, LabelOnForm labelOnForm,  int timers, Predicate predicate, string sensorName, SensorType type,ParamsList paramsList)
    {
        this.apiResponse = apiResponse;
        this.paramReboot = paramReboot;
        this.sensors = sensors;
        this.dataRanges = dataRanges;
        this.labelOnForm = labelOnForm;
        this.timers = timers;
        this.predicate = predicate;
        this.SensorName = sensorName;
        this.Type = type;
        this.paramsList = paramsList;
    }

    public enum Predicate
    {
        Min,
        Max
    }

    public enum ParamsList
    {
        tempMin,
        tempMax,
        fanMin,
        fanMax,
        loadMin,
        loadMax,
        clockMin,
        clockMax,
        memoryMin,
        memoryMax,
        internetOff

    }

    private void UpdateStatus()
    {
        if (paramReboot)
        {
            foreach (var sensor in sensors)
            {
                if (Check(sensor))
                {
                    labelOnForm.UpdateLable("true", timeReboot: timers);

                    labelOnForm.Tick();

                    if (labelOnForm.IsNeedReboot())
                    {
                        labelOnForm.SendRebootMessage();
                    }
                    
                }
                else if(!Check(sensor))
                {
                    labelOnForm.UpdateLable("ok", timeReboot: timers);
                }
            }
        }
        else
        {
            labelOnForm.UpdateLable("false", timeReboot: timers);
        }
    }
    //костыли 
    public void UpdateParams()
    {
        switch (paramsList)
        {
            case ParamsList.tempMin:
                dataRanges[0] = apiResponse.Params.Data_ranges.Temp[0];
                paramReboot = apiResponse.Params.Reboots.temp_min;
                timers = apiResponse.Params.Timers.temp_min;
                break;
            case ParamsList.tempMax:
                dataRanges[1] = apiResponse.Params.Data_ranges.Temp[1];
                paramReboot = apiResponse.Params.Reboots.temp_max;
                timers = apiResponse.Params.Timers.temp_max;
                break;
        }
    }

    public void UpdateSensors(int[] sensors)
    {
        this.sensors = sensors;
        UpdateParams();
        UpdateStatus();
    }

    private bool Check(int sensor)
    {
        switch (predicate)
        {
            case Predicate.Min:
                return sensor < dataRanges[0];
             //   break;
            case Predicate.Max:
                return sensor > dataRanges[1];
             //   break;
            default:
                return false;
           //     break;
        }
    }

    public void GetStatusInternet(bool paramReboot, LabelOnForm labelOnForm, int timers, bool checkPing)
    {
        if (paramReboot)
        {
            if (checkPing)
            {
                labelOnForm.UpdateLable("ok", timeReboot: timers);
            }
            else
            {
                labelOnForm.UpdateLable("true", timeReboot: timers);
            }
        }
        else 
        {
            labelOnForm.UpdateLable("false", timeReboot: timers);
        }
    }
}


class Reboot
{
    LogFile _log;
    Http _http;
    GlobalVars _globalVars;

    public Reboot(LogFile log, Http http, GlobalVars globalVars )
    {
        
        _log = log;
        _http = http;
        _globalVars = globalVars;
    }
    
    public void Restart(string msg, string bat,ApiResponse response)
    {
        try
        {
            _http.GetContent(
                _globalVars.host +
                "/api.php?token=" + _globalVars.token +
                "&event=" + "reboot" +
                "&reason=" + response.Params.Name + " " + msg
                );

            _log.writeLogLine("Reboot rig " + response.Params.Name + " " + msg, "log");

            Process.Start(bat);
        }
        catch (Exception ex)
        {
            _log.writeLogLine("Reboot: " + ex.Message, "error");
        }
    }
}


    class LabelOnForm
{
    Reboot Reboot;
    ApiResponse apiResponse;
    Label StatusLabel;
    Label CounterLabel;
    public int TimeReboot = 300;
    public string _msg;
    public string _batFileName;

     public LabelOnForm(Label statusLabel, Label counterLabel, ApiResponse apiResp, string msg, string batFileName ,Reboot reboot)
    {
        StatusLabel = statusLabel;
        CounterLabel = counterLabel;
        apiResponse = apiResp;
        _msg = msg;
        _batFileName = batFileName;
        Reboot = reboot;
        if (apiResp != null)
        {
            TimeReboot = apiResponse.Params.Timers.temp_min;
        }
    }

    public void Tick()
    {
        TimeReboot--;
    }

    public bool IsNeedReboot()
    {
        if (TimeReboot <= 0)
        {
            return true;
        }
        return false;
    }

    public void SendRebootMessage()
    {
        if (TimeReboot <= 0)
        {
            Reboot.Restart(_msg, _batFileName, apiResponse);
        }
    }

    public void UpdateLable(string status, int timeReboot = 300)
    {
        if (status == "false")
        {
            StatusLabel.Text = MyStrings.labelNotTracked;
            StatusLabel.ForeColor = Color.Blue;
            CounterLabel.Visible = false;
            if (apiResponse != null)
            {
                TimeReboot = timeReboot;
            }
        }
        else if (status == "true")
        {   
            StatusLabel.Text = MyStrings.labelAlert;
            StatusLabel.ForeColor = Color.Red;
            CounterLabel.Visible = true;
            CounterLabel.Text = TimeReboot.ToString();
            CounterLabel.ForeColor = Color.Red;
        }
        else if (status == "ok")
        {
            StatusLabel.Text = MyStrings.labelOK;
            StatusLabel.ForeColor = Color.Green;
            CounterLabel.Visible = false;
            if (apiResponse != null)
            {
                TimeReboot = timeReboot;
            }
        }
    }
}