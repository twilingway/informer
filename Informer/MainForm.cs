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
        public ApiResponse apiResponse;
        public Computer PC;
        GPUParams gpuParams;
        LabelOnForm tempMinLabel, tempMaxLabel, fanMinLabel,fanMaxLabel,loadMinLabel, loadMaxLabel,
                    clockMinLabel, clockMaxLabel,memoryMinLabel, memoryMaxLabel, NotInternetLabel;

        TimerOnForm tempMinTimer, tempMaxTimer, fanMinTimer,fanMaxTimer,loadMinTimer, loadMaxTimer,
                    clockMinTimer,clockMaxTimer,memoryMinTimer,memoryMaxTimer,NotInternetTimer;
        Danger danger;

        bool checkPing;
        public MainForm()
        {
            InitializeComponent();
           

            globalVars = new GlobalVars();
            danger = new Danger();
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


            

            KillDublicateProcess("Informer");
            KillDublicateProcess("Launcher_informer");

            apiResponse.Params = new Params
            {
                Timers = new Timers(),
                Reboots = new Reboots(),
                Data_ranges = new Data_ranges(),
                Version = "1.3.9"
            };

            apiResponse.Params.Data_ranges.Temp = new int[2];
            apiResponse.Params.Data_ranges.Fan = new int[2];
            apiResponse.Params.Data_ranges.Load = new int[2];
            apiResponse.Params.Data_ranges.Clock = new int[2];
            apiResponse.Params.Data_ranges.Fan = new int[2];
            apiResponse.Params.Data_ranges.Mem = new int[2];

            СheckForNewVersion();

            var response = apiResponse.Load();

            tempMinLabel = new LabelOnForm(labelStatusTempMin, labelCounterTempMin, response);
            tempMaxLabel = new LabelOnForm(labelStatusTempMax, labelCounterTempMax, response);
            fanMinLabel = new LabelOnForm(labelStatusFanMin, labelCounterFanMin, response);
            fanMaxLabel = new LabelOnForm(labelStatusFanMax, labelCounterFanMax, response);
            loadMinLabel = new LabelOnForm(labelStatusLoadMin, labelCounterLoadMin, response);
            loadMaxLabel = new LabelOnForm(labelStatusLoadMax, labelCounterLoadMax, response);
            clockMinLabel = new LabelOnForm(labelStatusClockMin, labelCounterClockMin, response);
            clockMaxLabel = new LabelOnForm(labelStatusClockMax, labelCounterClockMax, response);
            memoryMinLabel = new LabelOnForm(labelStatusMemoryMin, labelCounterMemoryMin, response);
            memoryMaxLabel = new LabelOnForm(labelStatusMemoryMax, labelCounterMemoryMax, response);
            NotInternetLabel = new LabelOnForm(labelStatusInternet, labelCounterInternet, response);

            tempMinTimer = new TimerOnForm(GPUTempMinTimer);
            tempMaxTimer = new TimerOnForm(GPUTempMaxTimer);
            fanMinTimer = new TimerOnForm(GPUFanMinTimer);
            fanMaxTimer = new TimerOnForm(GPUFanMaxTimer);
            loadMinTimer = new TimerOnForm(GPULoadMinTimer);
            loadMaxTimer = new TimerOnForm(GPULoadMaxTimer);
            clockMinTimer = new TimerOnForm(GPUCoreMinTimer);
            clockMaxTimer = new TimerOnForm(GPUCoreMaxTimer);
            memoryMinTimer = new TimerOnForm(GPUMemMinTimer);
            memoryMaxTimer = new TimerOnForm(GPUMemMaxTimer);
            NotInternetTimer = new TimerOnForm(DontHaveInternetTimer);
            bool start = false;

            if (response != null)
            {
                if (!string.IsNullOrEmpty(response.Params.Token))
                {
                    start = true;
                    tbRigName.ReadOnly = true;
                    tbToken.ReadOnly = true;
                    tbRigName.Text = response.Params.Name;
                    tbToken.Text = response.Params.Token;
                }
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
                tbRigName.Text = apiResponse.Params.Name;
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
            OHMTimer.Enabled = true;
            if (!string.IsNullOrWhiteSpace(apiResponse.Params.Name))
            {
                Message("Informer Started!", globalVars, apiResponse);
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
                labelTempMin.Text = "TEMP MIN(" + apiResponse.Params.Data_ranges.Temp[0] + "):";
                labelTempMax.Text = "TEMP MAX(" + apiResponse.Params.Data_ranges.Temp[1] + "):";
                labelFanMin.Text = "FAN MIN(" + apiResponse.Params.Data_ranges.Fan[0] + "):";
                labelFanMax.Text = "FAN MAX(" + apiResponse.Params.Data_ranges.Fan[1] + "):";
                labelLoadMin.Text = "LOAD MIN(" + apiResponse.Params.Data_ranges.Load[0] + "):";
                labelLoadMax.Text = "LOAD MAX(" + apiResponse.Params.Data_ranges.Load[1] + "):";
                labelClockMin.Text = "CLOCK MIN(" + apiResponse.Params.Data_ranges.Clock[0] + "):";
                labelClockMax.Text = "CLOCK MAX(" + apiResponse.Params.Data_ranges.Clock[1] + "):";
                labelMemoryMin.Text = "MEMORY MIN(" + apiResponse.Params.Data_ranges.Mem[0] + "):";
                labelMemoryMax.Text = "MEMORY MAX(" + apiResponse.Params.Data_ranges.Mem[1] + "):";
                labelFellOffGPU.Text = "GPU LOST(" + globalVars.count_GPU + "):";

                //temp min
                danger.GetStatusMin(apiResponse.Params.Reboots.temp_min,
                    gpuParams.Temperature,
                    apiResponse.Params.Data_ranges.Temp,
                    tempMinLabel,
                    tempMinTimer,
                    apiResponse.Params.Timers.temp_min);
                //temp max
                danger.GetStatusMax(apiResponse.Params.Reboots.temp_max,
                    gpuParams.Temperature,
                    apiResponse.Params.Data_ranges.Temp,
                    tempMaxLabel,
                    tempMaxTimer,
                    apiResponse.Params.Timers.temp_max);
                //fan min
                danger.GetStatusMin(apiResponse.Params.Reboots.fan_min,
                   gpuParams.FanSpeed,
                   apiResponse.Params.Data_ranges.Fan,
                   fanMinLabel,
                   fanMinTimer,
                   apiResponse.Params.Timers.fan_min);
                //fan max
                danger.GetStatusMax(apiResponse.Params.Reboots.fan_max,
                    gpuParams.FanSpeed,
                    apiResponse.Params.Data_ranges.Fan,
                    fanMaxLabel,
                    fanMaxTimer,
                    apiResponse.Params.Timers.fan_max);
                //load min
                danger.GetStatusMin(apiResponse.Params.Reboots.load_min,
                   gpuParams.Load,
                   apiResponse.Params.Data_ranges.Load,
                   loadMinLabel,
                   loadMinTimer,
                   apiResponse.Params.Timers.load_min);
                //load max
                danger.GetStatusMax(apiResponse.Params.Reboots.load_max,
                    gpuParams.Load,
                    apiResponse.Params.Data_ranges.Load,
                    loadMaxLabel,
                    loadMaxTimer,
                    apiResponse.Params.Timers.load_max);
                //clock min
                danger.GetStatusMin(apiResponse.Params.Reboots.clock_min,
                   gpuParams.Clock,
                   apiResponse.Params.Data_ranges.Clock,
                   clockMinLabel,
                   clockMinTimer,
                   apiResponse.Params.Timers.clock_min);
                //clock max
                danger.GetStatusMax(apiResponse.Params.Reboots.clock_max,
                    gpuParams.Clock,
                    apiResponse.Params.Data_ranges.Clock,
                    clockMaxLabel,
                    clockMaxTimer,
                    apiResponse.Params.Timers.clock_max);
                //memory min
                danger.GetStatusMin(apiResponse.Params.Reboots.mem_min,
                   gpuParams.Memory,
                   apiResponse.Params.Data_ranges.Mem,
                   memoryMinLabel,
                   memoryMinTimer,
                   apiResponse.Params.Timers.mem_min);
                //memory max
                danger.GetStatusMax(apiResponse.Params.Reboots.mem_max,
                    gpuParams.Memory,
                    apiResponse.Params.Data_ranges.Mem,
                    memoryMaxLabel,
                    memoryMaxTimer,
                    apiResponse.Params.Timers.mem_max);

                //dont have internet
                 danger.GetStatusInternet(apiResponse.Params.Reboots.lost_inet,
                    NotInternetLabel,
                    NotInternetTimer,
                    apiResponse.Params.Timers.lost_inet,checkPing);



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


                // gpu lost
                if (apiResponse.Params.Reboots.lost_gpu == false)
                {
                    labelStatusGPULost.Visible = true;
                    labelCounterGPULost.Visible = false;
                    labelStatusGPULost.Text = MyStrings.labelNotTracked;
                    labelStatusGPULost.ForeColor = Color.Blue;
                    globalVars.timer_gpu_lost = -100;
                    OHMTimer.Enabled = false;

                }
                else if (apiResponse.Params.Reboots.lost_gpu == true && globalVars.count_GPU > 0)
                {

                    labelStatusGPULost.Text = MyStrings.labelStatusOK;
                    labelStatusGPULost.ForeColor = Color.Green;


                    if (globalVars.count_GPU > globalVars.counts || globalVars.temp0 == true || globalVars.gpu_lost == true)
                    {
                        if (FellOffGPUTimer.Enabled == false)
                        {
                            globalVars.timer_gpu_lost = -100;
                        }

                        labelTestGPU.Text = labelTestGPU.Text + "GPU LOST \n";
                        OHMTimer.Enabled = true;

                        FellOffGPUTimer.Enabled = true;
                        labelCounterGPULost.Visible = true;
                        labelCounterGPULost.ForeColor = Color.Red;
                        labelCounterGPULost.Text = globalVars.timer_gpu_lost.ToString();
                        labelStatusGPULost.Text = MyStrings.labelStatusFellOffGPU;
                        labelStatusGPULost.ForeColor = Color.Red;

                        Task.Delay(10000);

                    }

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
                string v = apiResponse.Params.Version;
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

        public void Reboot(string msg, string bat)
        {
            try
            {
                _http.GetContent(
                    globalVars.host +
                    "/api.php?token=" + globalVars.token +
                    "&event=" + "reboot" +
                    "&reason=" + apiResponse.Params.Name + " " + msg

                    );

                _log.writeLogLine("Reboot rig " + apiResponse.Params.Name + " " + msg, "log");

                Process.Start(bat);
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Reboot: " + ex.Message, "error");
            }
        }


        public static async Task SendData(GlobalVars globalVars, ApiResponse apiResponse)
        {
            
            if (globalVars.mqttIsConnect == true)
            {
                try
                {
                    //globalVars.upTime = UpTime.ToString(@"dd\.hh\:mm\:ss");
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
                //  _error.writeLogLine("Send Message: " + ex.Message, "error");
                Debug.WriteLine("Send Message: " + ex.Message);
            }


        }

        /*
        public void Reload(string msg)
        {
            try
            {
                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
                var ppsi = Process.Start("cmd", @"/c taskkill /f /im " + globalVars.filename);
                ppsi.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
                var psiw = Process.Start("cmd", @"/c taskkill /f /im conhost.exe");
                psiw.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
                var psi = Process.Start("cmd", @"/c taskkill /f /im cmd.exe");
                psi.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
                System.Threading.Thread.Sleep(1000);
                Process.Start("nice.bat");

                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
                System.Threading.Thread.Sleep(1500);
                ProcessStartInfo rpsi;

                rpsi = new ProcessStartInfo
                {
                    WorkingDirectory = globalVars.dir2,
                    FileName = globalVars.pathreload2
                };
                System.Threading.Thread.Sleep(1000);

                Process.Start(rpsi);

                string pack = _http.GetContent(globalVars.host +
                    "/api.php?&worker=" + apiResponse.Params.Name +
                    "&gpu=" + globalVars.card +
                    "&temp=" + globalVars.temp +
                    "&fan=" + globalVars.fan +
                    "&status=reload" +
                    "&msg=" + msg);
                ReloadMinerTimer.Enabled = false;
                globalVars.timer_r_min = -100;
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Reload: " + ex.Message, "error");
            }
        }
        */

        private void BtStartClick(object sender, EventArgs e)
        {

            globalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;


            if (!string.IsNullOrWhiteSpace(tbToken.Text))
            {

                MqttConnectTimer.Enabled = true;
                GetTempretureTimer.Enabled = true;
                GPUStatusTimer.Enabled = true;
                SendDataTimer.Enabled = true;
                //globalVars.token = tbToken.Text;
                apiResponse.Params.Token = tbToken.Text;
                //apiResponse.Token = tbToken.Text;
                tbToken.ReadOnly = true;

                OHMTimer.Enabled = true;
                PingTimer.Enabled = true;


                if (!string.IsNullOrWhiteSpace(apiResponse.Params.Name))
                {
                    Message("Informer Started!", globalVars, apiResponse);
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
            Message("Informer Stopped!", globalVars, apiResponse);

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
            GPUTempMaxTimer.Enabled = false;
            GPUTempMinTimer.Enabled = false;
            ReloadMinerTimer.Enabled = false;
            GPUFanMaxTimer.Enabled = false;
            GPUFanMinTimer.Enabled = false;
            GPUCoreMinTimer.Enabled = false;
            GPUCoreMaxTimer.Enabled = false;
            GPUMemMinTimer.Enabled = false;
            GPUMemMaxTimer.Enabled = false;
            GPULoadMinTimer.Enabled = false;
            GPULoadMaxTimer.Enabled = false;
            FellOffGPUTimer.Enabled = false;
            InformationLabel.Text = MyStrings.labelInformationStop;
            InformationLabel.ForeColor = Color.Gray;
            globalVars.timer_t_max = -100;
            globalVars.timer_t_min = -100;
            globalVars.timer_fan_max = -100;
            globalVars.timer_fan_min = -100;
            globalVars.timer_r_min = -100;
            globalVars.timer_clock_min = -100;
            globalVars.timer_clock_max = -100;
            globalVars.timer_memory_min = -100;
            globalVars.timer_memory_max = -100;
            globalVars.timer_gpu_lost = -100;
            globalVars.timer_load_gpu_min = -100;
            globalVars.timer_load_gpu_max = -100;
            globalVars.timer_inet = -100;
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

        //timer temp min
        async private void LowTempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Reboot!";
            string bat = "reboot_t_min.bat";

            if (tempMinLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot2)
                {
                    Reboot(msg, bat);
                }
            }
            tempMinLabel.TimeReboot -= 1;
            await Task.Delay(1);
        }

        //timer temp max
        async private void TempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp MAX, Reboot!";
            string bat = "reboot_t_max.bat";
           
            if (tempMaxLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot1)
                {
                    Reboot(msg, bat);

                }

            }
            tempMaxLabel.TimeReboot -= 1;
            await Task.Delay(1);
        }

        /*
        private void RerunTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Application Reboot " + globalVars.filename2 + "!";
            if (globalVars.timer_r_min < 0)
            {
                globalVars.timer_r_min = globalVars.reload_time_min_file;
            }
            if (globalVars.timer_r_min == 0)
            {
                Reload(msg);
            }
            globalVars.timer_r_min = globalVars.timer_r_min - 1;
        }
        */

        //timer fan min
        async private void FanMinTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Min, Reboot!";
            const string bat = "reboot_fan_min.bat";
            
            if (fanMinLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot4)
                {
                    Reboot(msg, bat);
                }
            }
            fanMinLabel.TimeReboot -=1;
            await Task.Delay(1);
        }

        //timer fan max
        async private void FanMaxTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Max, Reboot! ";
            const string bat = "reboot_fan_max.bat";
            
            if (fanMaxLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot3)
                {
                    Reboot(msg, bat);

                }
            }
            fanMaxLabel.TimeReboot -=1;
            await Task.Delay(1);
        }
        
        //timer load min
        async private void GPULoadMin_Tick(object sender, EventArgs e)
        {
            const string msg = "GPU Load Min, Reboot!";
            const string bat = "reboot_load_min.bat";

            if (loadMinLabel.TimeReboot <= 0)
            {
                if (!globalVars.rebootLoadGPU)
                {
                    Reboot(msg, bat);
                }
            }
            loadMinLabel.TimeReboot -=1;
            await Task.Delay(1);
        }
        
        //timer load max
        async private void GPULoadMaxTimer_Tick(object sender, EventArgs e)
        {
            const string msg = "GPU Load Max, Reboot!";
            const string bat = "reboot_load_max.bat";

            //GPULoadMinTimer.Enabled = false;
            
                if (loadMaxLabel.TimeReboot == 0)
                {
                    if (!globalVars.rebootLoadGPU)
                    {
                        Reboot(msg, bat);
                    }
                }
                loadMaxLabel.TimeReboot -= 1;
            await Task.Delay(1);
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
                await SendData(globalVars, apiResponse);
            }

        }

        //timer clock min
        async private void GpuCoreMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Min, Reboot!";
            const string bat = "reboot_clock_min.bat";
            
            if (clockMinLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot5)
                {
                    Reboot(msg, bat);
                }
            }
            clockMinLabel.TimeReboot -=1;
            await Task.Delay(1);
        }
        
        //timer clock max
        async private void GpuCoreMaxHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Max , Reboot!";
            const string bat = "reboot_clock_max.bat";
            
            if (clockMaxLabel.TimeReboot <= 0)
            {
                if (!globalVars.coreMax)
                {
                    Reboot(msg, bat);
                }
            }
            clockMaxLabel.TimeReboot -= 1;
            await Task.Delay(1);
        }

        //timer memory min
        async private void GpuMemoryMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Memory Min, Reboot!";
            const string bat = "reboot_memory_min.bat";
            
            if (memoryMinLabel.TimeReboot <= 0)
            {
                if (!globalVars.reboot6)
                {
                    Reboot(msg, bat);
                }
            }
            memoryMinLabel.TimeReboot -=1;
            await Task.Delay(1);
        }

        //timer memory max
        async private void GPUMemoryMaxHzTimer_Tick(object sender, EventArgs e)
        {
            const string msg = "Memory Max, Reboot!";
            const string bat = "reboot_memory_max.bat";
            try
            {
                if (memoryMaxLabel.TimeReboot <= 0)
                {
                    if (!globalVars.memMax)
                    {
                        Reboot(msg, bat);
                    }
                }
                memoryMaxLabel.TimeReboot -=1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Memory Max:" + ex);
            }
            await Task.Delay(1);
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

        //timer FellOffGPU
        async private void FellOffTimerTick(object o, EventArgs e)
        {
            const string msg = "GPU fell, Reboot!";
            const string bat = "reboot_card.bat";

            try
            {
                if (globalVars.timer_gpu_lost < 0)
                {
                    globalVars.timer_gpu_lost = globalVars.Timer.lost_gpu;
                }
                if (globalVars.timer_gpu_lost == 0)
                {
                    if (!globalVars.IsRebootStarted)
                    {

                        Reboot(msg, bat);

                    }
                }
                globalVars.timer_gpu_lost = globalVars.timer_gpu_lost - 1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("GPU LOST " + ex);
            }
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

           // Debug.WriteLine("COUNT: " + hosts.Count);
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
            await MqttConnect.RunAsync(globalVars, apiResponse);
        }

        async private void OHMTimer_Tick(object sender, EventArgs e)
        {

            PC.Close();
            PC = null;
            PC = new Computer();
            PC.CPUEnabled = true;
            PC.GPUEnabled = true;
            PC.Open();
            GPUParams.GetGPU(globalVars, PC);

            await Task.Delay(1);


            if (globalVars.count_GPU == globalVars.counts)
            {
                globalVars.gpu_lost = false;

                labelTestGPU.Text = "GPU OK";
                OHMTimer.Enabled = false;
                FellOffGPUTimer.Enabled = false;
                labelCounterGPULost.Visible = false;
                globalVars.temp0 = false;
                //globalVars.gpu_lost = false;
                labelStatusGPULost.Text = MyStrings.labelStatusOK;
                labelStatusGPULost.ForeColor = Color.Green;
                globalVars.timer_gpu_lost = -100;

            }

            if (globalVars.count_GPU != globalVars.counts)
            {

                globalVars.gpu_lost = true;

            }


        }

        async private void GPUStatusTimer_Tick(object sender, EventArgs e)
        {
            GpuStatus();

            await Task.Delay(1);
        }

        //System Uptime

        public static TimeSpan UpTime
        {
            get
            {
                using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();       //Call this an extra time before reading its value
                                              // return TimeSpan.FromSeconds(uptime.NextValue());
                    return TimeSpan.FromSeconds(uptime.NextValue());
                }
            }
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
    public void GetStatusMin(bool paramReboot,int[] sensors,int[] dataRanges, LabelOnForm labelOnForm, TimerOnForm timerOnForm,int timers)
    {
        bool sensorAlarm = false;
        if (paramReboot)
        {
            foreach (var sensor in sensors)
            {
                if (sensor < dataRanges[0])
                {
                    labelOnForm.UpdateLable("true",timeReboot: timers);
                    timerOnForm.Enabled(true);
                    sensorAlarm = true;
                }
            }
            if (sensorAlarm == false)
            {
                labelOnForm.UpdateLable("ok", timeReboot: timers);
                timerOnForm.Enabled(false);
            }
        }
        else
        {
            labelOnForm.UpdateLable("false", timeReboot: timers);
            timerOnForm.Enabled(false);
        }
    }
    public void GetStatusMax(bool paramReboot, int[] sensors, int[] dataRanges, LabelOnForm labelOnForm, TimerOnForm timerOnForm, int timers)
    {
        bool sensorAlarm = false;
        if (paramReboot)
        {
            foreach (var sensor in sensors)
            {
                if (sensor > dataRanges[1])
                {
                    labelOnForm.UpdateLable("true", timeReboot: timers);
                    timerOnForm.Enabled(true);
                    sensorAlarm = true;
                }
            }
            if (sensorAlarm == false)
            {
                labelOnForm.UpdateLable("ok", timeReboot: timers);
                timerOnForm.Enabled(false);
            }
        }
        else
        {
            labelOnForm.UpdateLable("false", timeReboot: timers);
            timerOnForm.Enabled(false);
        }
    }
    public void GetStatusInternet(bool paramReboot, LabelOnForm labelOnForm, TimerOnForm timerOnForm, int timers, bool checkPing)
    {
        if (paramReboot)
        {
            if (checkPing)
            {
                labelOnForm.UpdateLable("ok", timeReboot: timers);
                timerOnForm.Enabled(false);
            }
            else
            {
                labelOnForm.UpdateLable("true", timeReboot: timers);
                timerOnForm.Enabled(true);
            }
        }
        else 
        {
            labelOnForm.UpdateLable("false", timeReboot: timers);
            timerOnForm.Enabled(false);
        }
    }
}

class TimerOnForm
{
    Timer StatusTimer;

    public TimerOnForm(Timer statusTimer)
    {
        StatusTimer = statusTimer;
    }

    public void Enabled(bool status)
    {
        if (status)
        {
            StatusTimer.Enabled = true;
        }
        else 
        {
            StatusTimer.Enabled = false;
        }
    }
}

class LabelOnForm
{
  
    ApiResponse apiResponse;
    Label StatusLabel;
    Label CounterLabel;
    public int TimeReboot = 300;

    public LabelOnForm(Label statusLabel, Label counterLabel,ApiResponse apiResp)
    {
        StatusLabel = statusLabel;
        CounterLabel = counterLabel;
        apiResponse = apiResp;
        if (apiResp != null)
        {
            TimeReboot = apiResponse.Params.Timers.temp_min;
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