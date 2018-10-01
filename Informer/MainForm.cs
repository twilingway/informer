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
//using MqttClientConnectedEventArgs = MQTTnet.Client.MqttClientConnectedEventArgs;
//using MqttClientDisconnectedEventArgs = MQTTnet.Client.MqttClientDisconnectedEventArgs;
using System.Globalization;

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
        public MainForm()
        {
            InitializeComponent();
            globalVars = new GlobalVars();
            PC = new Computer();

            PC.CPUEnabled = true;
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

            apiResponse.Params.Data_ranges.Temp = new int[2];
            apiResponse.Params.Data_ranges.Fan = new int[2];
            apiResponse.Params.Data_ranges.Load = new int[2];
            apiResponse.Params.Data_ranges.Clock = new int[2];
            apiResponse.Params.Data_ranges.Fan = new int[2];
            apiResponse.Params.Data_ranges.Mem = new int[2];

            СheckForNewVersion();

            var response = apiResponse.Load();

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
            if (string.IsNullOrEmpty(response.Params.Name) && string.IsNullOrEmpty(response.Params.Token))
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

        private void BtStopClick(object sender, EventArgs e)
        {
            _log.writeLogLine("Informer stopped", "log");
            Message("Informer Stopped!", globalVars,apiResponse);

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
                GPUTemp.GetGPU(globalVars,PC);
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
                Message("Informer Started!", globalVars,apiResponse);
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
            else {

                NextAutoStart.Start();
                AutoStartTimer.Start();
                globalVars.autostart = globalVars.autostart - 1;
                btStart.Text = MyStrings.btStart + "(" + globalVars.autostart.ToString() + ")";
            }

        }

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

                int i = 0;
                int tempMinCount = 0;
                int tempMaxCount = 0;
                int fanMinCount = 0;
                int fanMaxCount = 0;
                int loadMinCount = 0;
                int loadMaxCount = 0;
                int clockMinCount = 0;
                int clockMaxCount = 0;
                int memoryMinCount = 0;
                int memoryMaxCount = 0;
                
                foreach (var list in globalVars.gpuList)
                {
                    i++;
                    Debug.WriteLine("GPU TOTAL #: " + globalVars.gpuList.Count);
                    Debug.WriteLine("GPU list  #: " + list.Count);
                    foreach (var p in list)
                        {
                        Debug.WriteLine("GPU STATUS: "+ string.Format("{0} {1}", p.Key, p.Value));
                
                    switch (p.Key)
                        {
                            case "name":
                               

                                break;
                            case "temp":


                                //temp min
                                if (apiResponse.Params.Reboots.temp_min == false)
                                {

                                    labelStatusTempMin.Text = MyStrings.labelEvent;
                                    labelStatusTempMin.ForeColor = Color.Blue;
                                    labelCounterTempMin.Visible = false;
                                    globalVars.timer_t_min = -100;


                                }
                                else if (apiResponse.Params.Reboots.temp_min == true)
                                {
                                    if (Convert.ToInt32(p.Value) <= Convert.ToInt32(globalVars.Timer.temp_min) && Convert.ToInt32(p.Value) != 0)
                                    {

                                        GPUTempMinTimer.Enabled = true;
                                        globalVars.temp0 = false;
                                        labelStatusTempMin.Text = MyStrings.labelStatusTempMin;
                                        labelStatusTempMin.ForeColor = Color.Red;

                                        labelCounterTempMin.Visible = true;
                                        labelCounterTempMin.Text = globalVars.timer_t_min.ToString();
                                        labelCounterTempMin.ForeColor = Color.Red;
                                        tempMinCount++;
                                    }
                                    
                                    if (Convert.ToInt32(p.Value) == 0)
                                    {
                                        globalVars.temp0 = true;
                                        OHMTimer.Enabled = true;
                                        labelTestGPU.Text = "GPU LOST " + globalVars.counts;

                                    }
                                    if (tempMinCount == 0 && i == globalVars.gpuList.Count)
                                    {
                                        GPUTempMinTimer.Enabled = false;
                                        labelCounterTempMin.Visible = false;

                                        labelStatusTempMin.Text = MyStrings.labelStatusTempOK;
                                        labelStatusTempMin.ForeColor = Color.Green;
                                        globalVars.timer_t_min = -100;
                                    }
                                }

                                //temp max
                                if (apiResponse.Params.Reboots.temp_max == false)
                                {
                                    labelStatusTempMax.Text = MyStrings.labelEvent;
                                    labelStatusTempMax.ForeColor = Color.Blue;
                                    labelCounterTempMax.Visible = false;
                                    globalVars.timer_t_max = -100;
                                }
                                else if (apiResponse.Params.Reboots.temp_max == true)
                                {
                                    if (Convert.ToInt32(p.Value) >= Convert.ToInt32(globalVars.Timer.temp_max))
                                    {
                                        GPUTempMaxTimer.Enabled = true;

                                        labelStatusTempMax.Text = MyStrings.labelStatusTempMax;
                                        labelStatusTempMax.ForeColor = Color.Red;

                                        labelCounterTempMax.Visible = true;
                                        labelCounterTempMax.Text = globalVars.timer_t_max.ToString();
                                        labelCounterTempMax.ForeColor = Color.Red;
                                        tempMaxCount++;
                                    }
                                    

                                    if (tempMaxCount == 0 && i == globalVars.gpuList.Count)

                                    {
                                        GPUTempMaxTimer.Enabled = false;
                                        labelCounterTempMax.Visible = false;
                                        labelStatusTempMax.Text = MyStrings.labelStatusTempOK;
                                        labelStatusTempMax.ForeColor = Color.Green;
                                        globalVars.timer_t_max = -100;
                                    }
                                }


                                break;
                            case "core":
                               
                                //core
                                try
                                {
                                    IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "," };
                                    double bc = double.Parse(p.Value, formatter);
                                   

                                    int pc = (int)Math.Floor(bc);
                                    if (apiResponse.Params.Reboots.clock_min == true)
                                    {

                                        if (Convert.ToInt32(pc) <= globalVars.Timer.clock_min)
                                        {
                                            GPUCoreMinTimer.Enabled = true;


                                            labelStatusClockMin.Text = MyStrings.labelStatusClock;
                                            labelStatusClockMin.ForeColor = Color.Red;

                                            labelCounterClockMin.Visible = true;
                                            labelCounterClockMin.Text = globalVars.timer_clock_min.ToString();
                                            labelCounterClockMin.ForeColor = Color.Red;
                                            clockMinCount++;

                                        }
                                        
                                        if (clockMinCount == 0 && i == globalVars.gpuList.Count)
                                        {

                                            GPUCoreMinTimer.Enabled = false;
                                            labelCounterClockMin.Visible = false;
                                            labelStatusClockMin.Text = MyStrings.labelStatusOK;
                                            labelStatusClockMin.ForeColor = Color.Green;
                                            globalVars.timer_clock_min = -100;

                                        }


                                    }
                                    else if (apiResponse.Params.Reboots.clock_min == false)
                                    {
                                        labelStatusClockMin.Visible = true;
                                        labelStatusClockMin.Text = MyStrings.labelEvent;
                                        labelStatusClockMin.ForeColor = Color.Blue;
                                        labelCounterClockMin.Visible = false;
                                        globalVars.timer_clock_min = -100;
                                        GPUCoreMinTimer.Enabled = false;
                                    }

                                    if (apiResponse.Params.Reboots.clock_max == true)
                                    {
                                        if (Convert.ToInt32(pc) >= Convert.ToInt32(globalVars.Timer.clock_max))
                                        {
                                            GPUCoreMaxTimer.Enabled = true;


                                            labelStatusClockMax.Text = MyStrings.labelStatusClockMax;
                                            labelStatusClockMax.ForeColor = Color.Red;

                                            labelCounterClockMax.Visible = true;
                                            labelCounterClockMax.Text = globalVars.timer_clock_max.ToString();
                                            labelCounterClockMax.ForeColor = Color.Red;
                                            clockMaxCount++;

                                        }

                                        if (clockMaxCount == 0 && i == globalVars.gpuList.Count)
                                        {
                                            GPUCoreMaxTimer.Enabled = false;
                                            labelCounterClockMax.Visible = false;
                                            labelStatusClockMax.Text = MyStrings.labelStatusOK;
                                            labelStatusClockMax.ForeColor = Color.Green;
                                            globalVars.timer_clock_max = -100;

                                        }
                                    }
                                    else if (globalVars.Reboots.clock_max == false)
                                    {

                                        labelStatusClockMax.Visible = true;
                                        labelStatusClockMax.Text = MyStrings.labelEvent;
                                        labelStatusClockMax.ForeColor = Color.Blue;
                                        labelCounterClockMax.Visible = false;
                                        globalVars.timer_clock_max = -100;
                                        GPUCoreMaxTimer.Enabled = false;

                                    }
                                }
                                catch (Exception x)
                                {
                                    _error.writeLogLine("Core: " +x.Message, "error");
                                }


                                break;
                            case "memory":
                                // memory
                                try
                                {
                                    IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "," };
                                    double bm = double.Parse(p.Value, formatter);
                                    int pm = (int)Math.Floor(bm);

                                  


                                    if (globalVars.Reboots.mem_min == true)
                                    {
                                        if (Convert.ToInt32(pm) <= Convert.ToInt32(globalVars.Timer.mem_min))
                                        {
                                            GPUMemMinTimer.Enabled = true;


                                            labelStatusMemoryMin.Text = MyStrings.labelStatusMemoryMin;
                                            labelStatusMemoryMin.ForeColor = Color.Red;

                                            labelCounterMemoryMin.Visible = true;
                                            labelCounterMemoryMin.Text = globalVars.timer_memory_min.ToString();
                                            labelCounterMemoryMin.ForeColor = Color.Red;
                                            memoryMinCount++;

                                        }
                                        
                                        if (memoryMinCount == 0 && i == globalVars.gpuList.Count)
                                        {
                                            GPUMemMinTimer.Enabled = false;
                                            labelCounterMemoryMin.Visible = false;
                                            labelStatusMemoryMin.Text = MyStrings.labelStatusOK;
                                            labelStatusMemoryMin.ForeColor = Color.Green;
                                            globalVars.timer_memory_min = -100;
                                        }


                                    }
                                    else if (globalVars.Reboots.mem_min == false)
                                    {

                                        labelStatusMemoryMin.Visible = true;
                                        labelStatusMemoryMin.Text = MyStrings.labelEvent;
                                        labelStatusMemoryMin.ForeColor = Color.Blue;
                                        labelCounterMemoryMin.Visible = false;
                                        globalVars.timer_memory_min = -100;
                                        GPUMemMinTimer.Enabled = false;

                                    }

                                    if (globalVars.Reboots.mem_max == true)
                                    {
                                        if (Convert.ToInt32(pm) >= Convert.ToInt32(globalVars.Timer.mem_max))
                                        {
                                            GPUMemMaxTimer.Enabled = true;


                                            labelStatusMemoryMax.Text = MyStrings.labelStatusMemoryMax;
                                            labelStatusMemoryMax.ForeColor = Color.Red;

                                            labelCounterMemoryMax.Visible = true;
                                            labelCounterMemoryMax.Text = globalVars.timer_memory_max.ToString();
                                            labelCounterMemoryMax.ForeColor = Color.Red;
                                            memoryMaxCount++;

                                        }
                                        
                                        if (memoryMaxCount == 0 && i == globalVars.gpuList.Count)
                                        {
                                            GPUMemMaxTimer.Enabled = false;
                                            labelCounterMemoryMax.Visible = false;
                                            labelStatusMemoryMax.Text = MyStrings.labelStatusOK;
                                            labelStatusMemoryMax.ForeColor = Color.Green;
                                            globalVars.timer_memory_max = -100;

                                        }


                                    }
                                    else if (globalVars.Reboots.mem_max == false)
                                    {

                                        labelStatusMemoryMax.Visible = true;
                                        labelStatusMemoryMax.Text = MyStrings.labelEvent;
                                        labelStatusMemoryMax.ForeColor = Color.Blue;
                                        labelCounterMemoryMax.Visible = false;
                                        globalVars.timer_memory_max = -100;
                                        GPUMemMaxTimer.Enabled = false;
                                    }


                                   
                                }
                                catch (Exception ex)
                                {
                                    _error.writeLogLine("Memory: " + ex.Message, "error");
                                  
                                }
                                break;
                            case "load":

                                //gpu load min

                                if (globalVars.Reboots.load_min == true)
                                {
                                    if (Convert.ToInt32(p.Value) <= Convert.ToInt32(globalVars.Timer.load_min))
                                    {
                                        GPULoadMinTimer.Enabled = true;


                                        labelStatusLoadMin.Text = MyStrings.labelStatusLoadGPU;
                                        labelStatusLoadMin.ForeColor = Color.Red;

                                        labelCounterLoadMin.Visible = true;
                                        labelCounterLoadMin.Text = globalVars.timer_load_gpu_min.ToString();
                                        labelCounterLoadMin.ForeColor = Color.Red;
                                        loadMinCount++;

                                    }
                                    
                                    if (loadMinCount == 0 && i == globalVars.gpuList.Count)
                                    {
                                        GPULoadMinTimer.Enabled = false;
                                        labelCounterLoadMin.Visible = false;
                                        labelStatusLoadMin.Text = MyStrings.labelStatusOK;
                                        labelStatusLoadMin.ForeColor = Color.Green;
                                        globalVars.timer_load_gpu_min = -100;


                                    }

                                }
                                else if (globalVars.Reboots.load_min == false)
                                {
                                    labelStatusLoadMin.Visible = true;
                                    labelStatusLoadMin.Text = MyStrings.labelEvent;
                                    labelStatusLoadMin.ForeColor = Color.Blue;
                                    labelCounterLoadMin.Visible = false;
                                    globalVars.timer_load_gpu_min = -100;
                                    GPULoadMinTimer.Enabled = false;

                                }

                                if (globalVars.Reboots.load_max == true)
                                {
                                    if (Convert.ToInt32(p.Value) > Convert.ToInt32(globalVars.Timer.load_max))
                                    {
                                        GPULoadMaxTimer.Enabled = true;


                                        labelStatusLoadMax.Text = MyStrings.labelStatusLoadGPUMax;
                                        labelStatusLoadMax.ForeColor = Color.Red;

                                        labelCounterLoadMax.Visible = true;
                                        labelCounterLoadMax.Text = globalVars.timer_load_gpu_max.ToString();
                                        labelCounterLoadMax.ForeColor = Color.Red;
                                        loadMaxCount++;
                                    }
                                    if (loadMaxCount == 0 && i == globalVars.gpuList.Count)
                                    {
                                        GPULoadMaxTimer.Enabled = false;
                                        labelCounterLoadMax.Visible = false;
                                        labelStatusLoadMax.Text = MyStrings.labelStatusOK;
                                        labelStatusLoadMax.ForeColor = Color.Green;
                                        globalVars.timer_load_gpu_max = -100;


                                    }

                                }

                                else if (globalVars.Reboots.load_max == false)
                                {
                                    labelStatusLoadMax.Visible = true;
                                    labelStatusLoadMax.Text = MyStrings.labelEvent;
                                    labelStatusLoadMax.ForeColor = Color.Blue;
                                    labelCounterLoadMax.Visible = false;
                                    globalVars.timer_load_gpu_max = -100;
                                    GPULoadMaxTimer.Enabled = false;


                                }

                                break;
                            case "fan":

                                //fan min

                                if (globalVars.Reboots.fan_min == false)
                                {
                                    labelStatusFanMin.Visible = true;
                                    labelStatusFanMin.Text = MyStrings.labelEvent;
                                    labelStatusFanMin.ForeColor = Color.Blue;
                                    labelCounterFanMin.Visible = false;
                                    globalVars.timer_fan_min = -100;

                                }
                                else if (globalVars.Reboots.fan_min == true)
                                {
                                    if (Convert.ToInt32(p.Value) <= Convert.ToInt32(globalVars.Timer.fan_min))
                                    {

                                        GPUFanMinTimer.Enabled = true;


                                        labelStatusFanMin.Text = MyStrings.labelStatusFanMin;
                                        labelStatusFanMin.ForeColor = Color.Red;

                                        labelCounterFanMin.Visible = true;
                                        labelCounterFanMin.Text = globalVars.timer_fan_min.ToString();
                                        labelCounterFanMin.ForeColor = Color.Red;
                                        fanMinCount++;

                                    }
                                    
                                    if (fanMinCount == 0 && i == globalVars.gpuList.Count)
                                    {
                                        GPUFanMinTimer.Enabled = false;
                                        labelCounterFanMin.Visible = false;
                                        labelStatusFanMin.Text = MyStrings.labelStatusFanOK;
                                        labelStatusFanMin.ForeColor = Color.Green;
                                        globalVars.timer_t_min = -100;

                                    }

                                }

                                //fan max
                                if (globalVars.Reboots.fan_max == false)
                                {
                                    labelStatusFanMax.Visible = true;
                                    labelCounterFanMax.Visible = false;
                                    labelStatusFanMax.Text = MyStrings.labelEvent;
                                    labelStatusFanMax.ForeColor = Color.Blue;
                                    globalVars.timer_fan_max = -100;

                                }
                                else if (globalVars.Reboots.fan_max == true)
                                {


                                    if (Convert.ToInt32(p.Value) >= Convert.ToInt32(globalVars.Timer.fan_max))
                                    {

                                        GPUFanMaxTimer.Enabled = true;

                                        labelStatusFanMax.Text = MyStrings.labelStatusFanMax;
                                        labelStatusFanMax.ForeColor = Color.Red;

                                        labelCounterFanMax.Visible = true;
                                        labelCounterFanMax.Text = globalVars.timer_fan_max.ToString();
                                        labelCounterFanMax.ForeColor = Color.Red;
                                        fanMaxCount++;


                                    }
                                    
                                    if (fanMaxCount == 0 && i == globalVars.gpuList.Count)
                                    {
                                        GPUFanMaxTimer.Enabled = false;
                                        labelCounterFanMax.Visible = false;
                                        labelStatusFanMax.Text = MyStrings.labelStatusFanOK;
                                        labelStatusFanMax.ForeColor = Color.Green;
                                        globalVars.timer_fan_max = -100;

                                    }

                                }
                                break;
                        }

                    }


                }

                labelTest.Text ="tMin " + tempMinCount + " tMax " + tempMaxCount + " fMax " + fanMaxCount + " fMin "+ fanMinCount + " cMin " + clockMinCount + " cMax " + clockMaxCount + "\n"
                    + "mMin " +memoryMinCount + " mMax " + memoryMaxCount + " lMin "+ loadMinCount + " lMax " + loadMaxCount + " countGPU " + globalVars.counts + " TotalGPU "+ globalVars.counts;

                //no inet
                if (apiResponse.Params.Reboots.lost_inet == false)
                {
                    labelStatusInternet.Visible = true;
                    labelCounterInternet.Visible = false;
                    labelStatusInternet.Text = MyStrings.labelEvent;
                    labelStatusInternet.ForeColor = Color.Blue;
                    globalVars.timer_fan_max = -100;

                }
                else if (apiResponse.Params.Reboots.lost_inet == true)
                {

                    if (globalVars.mqttIsConnect == false && globalVars.ping == false && globalVars.firsrun == false)
                    {
                        DontHaveInternetTimer.Enabled = true;

                        labelStatusInternet.Text = MyStrings.labelStatusInternet;
                        labelStatusInternet.ForeColor = Color.Red;

                        labelCounterInternet.Visible = true;
                        labelCounterInternet.Text = globalVars.timer_inet.ToString();
                        labelCounterInternet.ForeColor = Color.Red;
                    }


                    else if (globalVars.mqttIsConnect == true || globalVars.ping == true)
                    {

                        DontHaveInternetTimer.Enabled = false;
                        labelCounterInternet.Visible = false;
                        labelStatusInternet.Text = MyStrings.labelStatusOK;
                        labelStatusInternet.ForeColor = Color.Green;
                        globalVars.timer_inet = -100;

                    }


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


                // gpu lost
                if (apiResponse.Params.Reboots.lost_gpu == false)
                {
                    labelStatusGPULost.Visible = true;
                    labelCounterGPULost.Visible = false;
                    labelStatusGPULost.Text = MyStrings.labelEvent;
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
            catch (Exception e) {

                Debug.WriteLine("GpuStatus: " + e);
            }

        }

        public static async Task SendData(GlobalVars globalVars,ApiResponse apiResponse)
        {
            
            if (globalVars.mqttIsConnect == true)
            {
                try
                {
                    globalVars.upTime = UpTime.ToString(@"dd\.hh\:mm\:ss");

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
                    Message("Send data MqttCommunicationException: " + ex.Message, globalVars,apiResponse);
                }
                catch (Exception ex)
                {

                    Message("Send data Ex: " + ex.Message, globalVars,apiResponse);

                }

            }
        }

        public void CheckForLauncher()
        {
        
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
                    "&reason="  + apiResponse.Params.Name + " " + msg
                   
                    );

                _log.writeLogLine("Reboot rig " + apiResponse.Params.Name + " " + msg, "log");

                 Process.Start(bat);
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Reboot: " + ex.Message, "error");
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

        public void Reload(string msg)
        {
            try
            {
                ReloadMinerTimer.Enabled         = false;
                globalVars.timer_r_min = -100;
                var ppsi               = Process.Start("cmd", @"/c taskkill /f /im " + globalVars.filename);
                ppsi.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled         = false;
                globalVars.timer_r_min = -100;
                var psiw               = Process.Start("cmd", @"/c taskkill /f /im conhost.exe");
                psiw.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled         = false;
                globalVars.timer_r_min = -100;
                var psi                = Process.Start("cmd", @"/c taskkill /f /im cmd.exe");
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
                    Message("Informer Started!",globalVars, apiResponse);
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

        private void BtnOpenSettingsFormClick(object sender, EventArgs e)
        {
            NextAutoStart.Stop();
            AutoStartTimer.Stop();
           // f2.ShowDialog();
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

        async private void TempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp MAX, Reboot!";
            string bat = "reboot_t_max.bat";
            if (globalVars.timer_t_max < 0) {
                globalVars.timer_t_max = globalVars.Timer.temp_max;
            }

            if (globalVars.timer_t_max == 0)
            {
                if (!globalVars.reboot1)
                {
                    Reboot(msg, bat);
                
                }
              
            }
            globalVars.timer_t_max = globalVars.timer_t_max - 1;
            await Task.Delay(1);
        }

        //timer temp min
       async private void LowTempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Reboot!";
            string bat = "reboot_t_min.bat";
            if (globalVars.timer_t_min < 0)
                {
                    globalVars.timer_t_min = globalVars.Timer.temp_min;
                }
                if (globalVars.timer_t_min == 0)
                {
                if (!globalVars.reboot2)
                {
                  
                        Reboot(msg, bat);
                  
                }
            }
                globalVars.timer_t_min = globalVars.timer_t_min - 1;
            await Task.Delay(1);
        }

        private void RerunTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Application Reboot " + globalVars.filename2 +"!";
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

        async private void FanMaxTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Max, Reboot! ";
            const string bat = "reboot_fan_max.bat";
            if (globalVars.timer_fan_max < 0)
            {
                globalVars.timer_fan_max = globalVars.Timer.fan_max;
            }
            if (globalVars.timer_fan_max == 0)
            {
                if (!globalVars.reboot3)
                {
                    Reboot(msg, bat);
                
                }
            }
            globalVars.timer_fan_max = globalVars.timer_fan_max - 1;
            await Task.Delay(1);
        }

        async private void FanMinTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Min, Reboot!";
            const string bat = "reboot_fan_min.bat";
            if (globalVars.timer_fan_min < 0)
            {
                globalVars.timer_fan_min = globalVars.Timer.fan_min;
            }
            if (globalVars.timer_fan_min == 0)
            {
                if (!globalVars.reboot4)
                {
                   
                        Reboot(msg, bat);
               
                   
                }
            }
            globalVars.timer_fan_min = globalVars.timer_fan_min - 1;
            await Task.Delay(1);
        }

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

        

        async private void GpuCoreMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Min , Reboot!";
            const string bat = "reboot_clock.bat";
            if (globalVars.timer_clock_min < 0)
            {
                globalVars.timer_clock_min = globalVars.Timer.clock_min;
            }
            if (globalVars.timer_clock_min == 0)
            {
                if (!globalVars.reboot5)
                {
                  
                        Reboot(msg, bat);
                  
                }
            }
            globalVars.timer_clock_min = globalVars.timer_clock_min - 1;
            await Task.Delay(1);
        }

       async private void GpuCoreMaxHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Max , Reboot!";
            const string bat = "reboot_clock_max.bat";
            if (globalVars.timer_clock_max < 0)
            {
                globalVars.timer_clock_max = globalVars.Timer.clock_max;
            }
            if (globalVars.timer_clock_max == 0)
            {
                if (!globalVars.coreMax)
                {

                    Reboot(msg, bat);

                }
            }
            globalVars.timer_clock_max = globalVars.timer_clock_max - 1;
            await Task.Delay(1);
        }


       async private void GpuMemoryMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Memory Min, Reboot!";
            const string bat = "reboot_memory.bat";
            if (globalVars.timer_memory_min < 0)
            {
                globalVars.timer_memory_min = globalVars.Timer.mem_min;
            }
            if (globalVars.timer_memory_min == 0)
            {
                if (!globalVars.reboot6)
                {
                  
                        Reboot(msg, bat);
                  
                }
            }
            globalVars.timer_memory_min = globalVars.timer_memory_min - 1;
            await Task.Delay(1);
        }


        async private void GPUMemoryMaxHzTimer_Tick(object sender, EventArgs e)
        {
            const string msg = "Memory Max, Reboot!";
            const string bat = "reboot_memory_max.bat";
            try
            {
                if (globalVars.timer_memory_max < 0)
                {
                    globalVars.timer_memory_max = globalVars.Timer.mem_max;
                }
                if (globalVars.timer_memory_max == 0)
                {
                    if (!globalVars.memMax)
                    {

                        Reboot(msg, bat);

                    }
                }
                globalVars.timer_memory_max = globalVars.timer_memory_max - 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Memory Max:" + ex);
            }
            await Task.Delay(1);
        }

    
        async private void InternetInactiveTimerTick(object sender, EventArgs e)
        {
            const string bat = "reboot_internet.bat";

           try
            {
                if (globalVars.timer_inet < 0)
                {
                    globalVars.timer_inet = globalVars.Timer.lost_inet;
                }
                if (globalVars.timer_inet == 0)
                {
                    if (!globalVars.rebootDontHaveInternet)
                    {
                        if (!globalVars.InternetIsActive)
                        {
                            Process.Start(bat);
                        }
                        else
                        {
                            globalVars.timer_inet = -100;
                        }
                    }
                }

                globalVars.timer_inet = globalVars.timer_inet - 1;
                
            }
            catch ( Exception ex)
            {

                Debug.WriteLine("InternetInactiveTimer: " + ex);
            }

            await Task.Delay(1);
        }

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

       async private void GPULoadMin_Tick(object sender, EventArgs e)
        {
            const string msg = "GPU Load Min, Reboot!";
            const string bat = "reboot_load_min.bat";

          
            if (globalVars.timer_load_gpu_min < 0)
                {
                    globalVars.timer_load_gpu_min = globalVars.Timer.load_min;
                }
                if (globalVars.timer_load_gpu_min == 0)
                {
                    if (!globalVars.rebootLoadGPU)
                    {
                       Reboot(msg, bat);
                    }
                }
                globalVars.timer_load_gpu_min = globalVars.timer_load_gpu_min - 1;
           
           
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

        async private void GPULoadMaxTimer_Tick(object sender, EventArgs e)
        {
            const string msg = "GPU Load Max, Reboot!";
            const string bat = "reboot_load_max.bat";

            //GPULoadMinTimer.Enabled = false;
            try
            {
                if (globalVars.timer_load_gpu_max < 0)
                {
                    globalVars.timer_load_gpu_max = globalVars.Timer.load_max;
                }
                if (globalVars.timer_load_gpu_max == 0)
                {
                    if (!globalVars.rebootLoadGPU)
                    {
                        Reboot(msg, bat);
                    }
                }
                globalVars.timer_load_gpu_max = globalVars.timer_load_gpu_max - 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GPU Load Max " + ex);
            }
            await Task.Delay(1);
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {

            globalVars.pingCount = 0;
            // В переменную hosts записываем все рабочие станции из файла
            hosts = getComputersListFromTxtFile("pinglist.txt");

            Debug.WriteLine("COUNT: "+hosts.Count);
            // Создаём Action типизированный string, данный Action будет запускать функцию Pinger

            Action<string> asyn = new Action<string>(Pinger);
            
             hosts.ForEach(p =>
             {
                 asyn.Invoke(p);
             });

            if (globalVars.pingCount >= hosts.Count)
            {
                globalVars.ping = false;
            }
            else if (globalVars.pingCount < hosts.Count)
            {
                globalVars.ping = true;
            }
        }
       
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
                        globalVars.pingCount = globalVars.pingCount + 1;
                    }

                    else if (pr.Status == IPStatus.Success)
                    {
                        globalVars.pingCount = globalVars.pingCount - 1;
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
            await MqttConnect.RunAsync(globalVars,apiResponse);
        }

        async private void OHMTimer_Tick(object sender, EventArgs e)
        {

            PC.Close();
            PC = null;
            PC = new Computer();
            PC.CPUEnabled = true;
            PC.GPUEnabled = true;
            PC.Open();
            GPUTemp.GetGPU(globalVars,PC);

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
    }




}




