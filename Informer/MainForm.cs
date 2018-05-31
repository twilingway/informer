using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Exceptions;
//using MQTTnet.Extensions.Rpc;
using MQTTnet.Implementations;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MqttClientConnectedEventArgs = MQTTnet.Client.MqttClientConnectedEventArgs;
using MqttClientDisconnectedEventArgs = MQTTnet.Client.MqttClientDisconnectedEventArgs;

namespace Informer
{
    public partial class MainForm : Form
    {


        private Computer _pc;
        private static Http _http = new Http();
        private LogFile _log, _error;
        //private INIManager GlobalVars._manager;
        List<String> gpusList = new List<string>();

        private Form f2;

        //static MqttClient client = new MqttClient("allminer.ru", int.Parse("1883"), false, MqttSslProtocols.None, null, null);



        public MainForm()
        {





            GlobalVars.gpuList = new Dictionary<int, List<string>>();

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            }


            InitializeComponent();

            //  GlobalVars.mqttClient = new MqttClient("allminer.ru", int.Parse("1883"), false, MqttSslProtocols.None, null, null);
            //  GlobalVars.mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            //  GlobalVars.mqttClient.ProtocolVersion = MqttProtocolVersion.Version_3_1;

            //string fullPath = Application.StartupPath.ToString();
            //GlobalVars.GlobalVars._manager = new INIManager(fullPath + "\\my.ini");
            GlobalVars._manager.WritePrivateString("main", "version", "1.3.9");

            try
            {
                Process psiwer;
                psiwer = Process.Start("cmd", @"/c taskkill /f /im launcher_informer.exe");
                psiwer.Close();
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Kill launcher: " + ex.Message, "error");
            }
            _pc = new Computer();

            _pc.CPUEnabled = true;
            //_pc.Open();
            _pc.GPUEnabled = true;

            _pc.Open();


            _log = new LogFile("log");
            _error = new LogFile("error");


            //СheckForPing();
            СheckForNewVersion();
            InitFromIni();

            f2 = new SettingsForm();



            // Create TCP based options using the builder.
            GlobalVars.options = new MqttClientOptionsBuilder()
            //var options = 
                .WithClientId(GlobalVars.token)
                .WithTcpServer("allminer.ru", 1883)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
                .WithCredentials(GlobalVars.token, GlobalVars.token)
                //.WithTls()
                .WithCleanSession(true)
                .Build();

            // Create a new MQTT client.
            // GlobalVars.factory = new MqttFactory();
            GlobalVars.mqttClient = GlobalVars.factory.CreateMqttClient();
            // var receive = new Receive();

        }

        //Инициализация компонентов

        public void InitFromIni()
        {
            GlobalVars.time_start = 60;
            //string worker;
            GlobalVars.versions = GlobalVars._manager.GetPrivateString("main", "version");
            GlobalVars.token = GlobalVars._manager.GetPrivateString("main", "token");
            GlobalVars.name = GlobalVars._manager.GetPrivateString("main", "name");

            try
            {
                GlobalVars.time_temp_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_temp_min"));
                GlobalVars.time_temp_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_temp_max"));

                GlobalVars.time_mem_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_mem_min"));
                GlobalVars.time_mem_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_mem_max"));

                GlobalVars.time_lost_inet = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_lost_inet"));
                GlobalVars.time_lost_gpu = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_lost_gpu"));

                GlobalVars.time_load_GPU_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_load_GPU_min"));
                GlobalVars.time_load_GPU_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_load_GPU_max"));

                GlobalVars.time_fan_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_fan_min"));
                GlobalVars.time_fan_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_fan_max"));

                GlobalVars.time_clock_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_clock_min"));
                GlobalVars.time_clock_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_clock_max"));

                GlobalVars.reboots_temp_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_temp_min"));
                GlobalVars.reboots_temp_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_temp_max"));

                GlobalVars.reboots_fan_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_fan_min"));
                GlobalVars.reboots_fan_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_fan_max"));

                GlobalVars.reboots_load_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_load_min"));
                GlobalVars.reboots_load_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_load_max"));

                GlobalVars.reboots_clock_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_clock_min"));
                GlobalVars.reboots_clock_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_clock_max"));

                GlobalVars.reboots_mem_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_mem_min"));
                GlobalVars.reboots_mem_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_mem_max"));

                GlobalVars.reboots_lost_gpu = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_lost_gpu"));
                GlobalVars.reboots_lost_inet = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_lost_inet"));


                GlobalVars.temp_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "temp_min"));
                GlobalVars.temp_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "temp_max"));

                GlobalVars.mem_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_min"));
                GlobalVars.mem_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_max"));

                GlobalVars.load_GPU_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "load_GPU_min"));
                GlobalVars.load_GPU_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "load_GPU_max"));

                GlobalVars.fan_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "fan_min"));
                GlobalVars.fan_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "fan_max"));

                GlobalVars.clock_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_min"));
                GlobalVars.clock_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_max"));


                GlobalVars.time_start = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_start"));

                //fsdf


            }
            catch (Exception e) {

                _error.writeLogLine("InitFromIni: " + e.Message, "error");
            }


            //----------------------------****************----------------------------

            bool start = false;

            if (!string.IsNullOrEmpty(GlobalVars.token))
            {
                start = true;
                tbEmail.ReadOnly = true;
                tbSecret.ReadOnly = true;
                tbRigName.ReadOnly = true;
                tbToken.ReadOnly = true;
                tbRigName.Text = GlobalVars.name;
                tbToken.Text = GlobalVars.token;
            }

            if (string.IsNullOrEmpty(GlobalVars.name) && string.IsNullOrEmpty(GlobalVars.token))
            {
                start = false;
                tbRigName.ReadOnly = true;

            }


            if (start)
            {
                NextAutoStart.Interval = GlobalVars.time_start * 1000;
                NextAutoStart.Enabled = true;
                AutoStartTimer.Enabled = true;
                TimeWorkTimer.Enabled = true;
            }


        }



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


        public static async Task MqttConnect()

        {
            if (!GlobalVars.mqttClient.IsConnected)
            {
                try
                {

                    GlobalVars.mqttClient.ApplicationMessageReceived += (s, e) =>
                    {
                        Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                        Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                        Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                        Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                        CommandProcesser.onMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload), e.ApplicationMessage.Topic);

                    // Debug.WriteLine();
                };


                    GlobalVars.mqttClient.Connected += async (s, e) =>
                    {
                        await GlobalVars.mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("devices/" + GlobalVars.token + "/commands").Build());
                    };

                    /*
                    mqttClient.Disconnected += async (s, e) =>
                    {
                        Debug.WriteLine("### DISCONNECTED FROM SERVER ###");
                        await Task.Delay(TimeSpan.FromSeconds(5));

                        try
                        {
                            await mqttClient.ConnectAsync(options);
                        }
                        catch
                        {
                            Debug.WriteLine("### RECONNECTING FAILED ###");
                        }
                    };
                    */



                    try
                    {
                        await GlobalVars.mqttClient.ConnectAsync(GlobalVars.options);

                        if (GlobalVars.mqttClient.IsConnected)
                        {
                            Debug.WriteLine("IsConnected: " );
                          //  GlobalVars.token_status = true;
                            SendData();

                            
                        }
                        
                        // InformationLabel.Text = MyStrings.labelInformationAuthorizationFailed;
                        // InformationLabel.ForeColor = Color.Red;


                        try
                        {
                            var message = new MqttApplicationMessageBuilder()
                            .WithTopic("devices/" + GlobalVars.token + "/init")
                            .WithPayload("1")
                            .WithExactlyOnceQoS()
                            .WithRetainFlag()
                            .Build();

                            await GlobalVars.mqttClient.PublishAsync(message);

                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("PublishFailed: " + e);

                        }

                    }

                    catch (MQTTnet.Adapter.MqttConnectingFailedException e)
                    {

                        Debug.WriteLine("AuthorizationFailed: " + e);
                        //InformationLabel.Text = MyStrings.labelInformationAuthorizationFailed;
                        //InformationLabel.ForeColor = Color.Red;
                        GlobalVars.token_status = false;

                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine("MqttConnectError: " + e);
                    }



                }

                catch (Exception e)
                {


                }
            }
           

        }


        private void MqttStop() {

            /*
            if (GlobalVars.mqttClient.IsConnected)
            {
                try
                {
                    //GlobalVars.mqttClientUnsubscribe(["sdfs",]);
                    GlobalVars.mqttClient.Unsubscribe(new string[] { "devices/" + GlobalVars.token + "/commands" });
                    GlobalVars.mqttClient.Disconnect();
                   
                    // GlobalVars.mqttClient = null;



                }
                catch (Exception ex)
                {
                    _error.writeLogLine("MqttStop: " + ex.Message, "error");
                }
            }

            */
        }



        private void BtStopClick(object sender, EventArgs e)
        {

            if (GlobalVars.token_status)
            {
                //  MqttStop();

                _log.writeLogLine("Informer stopped", "log");
                Message("Informer Stopped!");

            }

            GetTempretureTimer.Enabled = false;
            AutoStartTimer.Enabled = false;
            AutoStartTimer.Stop();
            // PingInternetTimer.Stop();
            NextAutoStart.Stop();
            btStop.Visible = false;
            btStart.Enabled = true;
            SendDataTimer.Enabled = false;
            GPUTempMaxTimer.Enabled = false;
            GPUTempMinTimer.Enabled = false;
            ReloadMinerTimer.Enabled = false;
            GPUFanMaxTimer.Enabled = false;
            GPUFanMinTImer.Enabled = false;
            GPUCoreMinTimer.Enabled = false;
            GPUMemMinTimer.Enabled = false;
            FellOffGPUTimer.Enabled = false;
            InformationLabel.Text = MyStrings.labelInformationStop;
            InformationLabel.ForeColor = Color.Gray;
            GlobalVars.timer_t_max = -100;
            GlobalVars.timer_t_min = -100;
            GlobalVars.timer_fan_max = -100;
            GlobalVars.timer_fan_min = -100;
            GlobalVars.timer_r_min = -100;
            GlobalVars.timer_clock = -100;
            GlobalVars.timer_memory = -100;
            GlobalVars.timer_t_card = -100;
            GlobalVars.timer_load_gpu = -100;
            GlobalVars.timer_inet = -100;
            tbToken.ReadOnly = false;

        }

        private void GetTempretureTimerTick(object sender, EventArgs e)
        {
            try
            {
                btStart.Enabled = false;
                NextAutoStart.Enabled = false;
                AutoStartTimer.Enabled = false;
                btStop.Visible = true;
                gpu_temp();



            }
            catch (Exception ex)
            {
                _error.writeLogLine("GetTempTimer: " + ex.Message, "error");
            }
        }

        private void NextAutoStart_Tick(object sender, EventArgs e)
        {
            GetTempretureTimer.Enabled = true;
            SendDataTimer.Enabled = true;
            AutoStartTimer.Enabled = false;
            btStop.Visible = true;
            btStart.Enabled = false;
            GlobalVars.timeOnline = 0;
            tbEmail.ReadOnly = true;
            tbSecret.ReadOnly = true;
            tbRigName.ReadOnly = true;
            tbToken.ReadOnly = true;
            //InformationLabel.Text = "Запущен";
            InformationLabel.ForeColor = Color.Green;
            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            //labelTest.Text = GlobalVars.start_timestamp.ToString();
            gpu_temp();
            Message("Informer Started!");
            //try
            // {
            MqttConnect();
            //   GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/init", Encoding.UTF8.GetBytes("1"));
            // }
            // catch {

            // }
            SendData();

            Hide();


        }

        private void AutoStart_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GlobalVars.token))
            {
                tbToken.ReadOnly = true;
                NextAutoStart.Start();
                AutoStartTimer.Start();
                GlobalVars.time_start = GlobalVars.time_start - 1;
                btStart.Text = MyStrings.btStart + "(" + GlobalVars.time_start.ToString() + ")";

            }
            else {

                NextAutoStart.Start();
                AutoStartTimer.Start();
                GlobalVars.time_start = GlobalVars.time_start - 1;
                btStart.Text = MyStrings.btStart + "(" + GlobalVars.time_start.ToString() + ")";
            }

        }

        public void gpu_temp()
        {
            try
            {

                GlobalVars.card = "";
                GlobalVars.temp = "";
                GlobalVars.fan = "";
                GlobalVars.load = "";
                GlobalVars.clock = "";
                GlobalVars.mem = "";
                GlobalVars.counts = 0;
                int r_count = 0;
                int c_count = 0;
                int m_count = 0;
                int max_count = 0;
                int min_count = 0;
                int fan_max_count = 0;
                int fan_min_count = 0;
                int load_count = 0;

                int temp1 = 0;
                int fan1 = 0;
                int fanmin = 200;
                int tempmin = 200;
                int tempmax = -10;
                int fanmax = -10;
                int clockmin = 99999;
                int memorymin = 99999;
                int loadmin = 200;
                int clockk1;
                int mem1;
                int load1;
                gpusList.Clear();
                GlobalVars.gpuList.Clear();
                foreach (var hard in _pc.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
                {

                    hard.Update();


                    if (hard.HardwareType == HardwareType.GpuAti || hard.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                    {

                        GlobalVars.counts = GlobalVars.counts + 1;
                        GlobalVars.card += hard.Name + ",";
                        gpusList.Add(hard.Name);

                        foreach (var sensor in hard.Sensors)//ИДЕМ по сенсорам
                        {


                            if (sensor.SensorType == SensorType.Clock)
                            {//ЧАСТОТЫ



                                if (sensor.Name == "GPU Core")//ЯДРО
                                {

                                    GlobalVars.clock += sensor.Value.GetValueOrDefault() + ";";
                                    clockk1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());

                                    gpusList.Add(Convert.ToString(clockk1));

                                    if (clockmin > clockk1)
                                    {
                                        clockmin = clockk1;
                                    }


                                }



                                if (hard.HardwareType == HardwareType.GpuAti)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        mem1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                        if (memorymin > mem1)
                                        {
                                            memorymin = mem1;
                                        }
                                        gpusList.Add(Convert.ToString(mem1));

                                    }

                                }
                                else if (hard.HardwareType == HardwareType.GpuNvidia)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        mem1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                        gpusList.Add(Convert.ToString(mem1));
                                        if (memorymin > mem1)
                                        {
                                            memorymin = mem1;
                                        }
                                    }
                                }
                                else
                                {

                                }



                            }
                            else if (sensor.SensorType == SensorType.Temperature)//Температура
                            {


                                GlobalVars.temp += sensor.Value.GetValueOrDefault() + ",";
                                temp1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                gpusList.Add(Convert.ToString(temp1));

                                if (tempmin > temp1)
                                {
                                    tempmin = temp1;
                                }
                                if (tempmax < temp1)
                                {
                                    tempmax = temp1;
                                }
                            }
                            else if (sensor.SensorType == SensorType.Control)// FAN
                            {

                                GlobalVars.fan += sensor.Value.GetValueOrDefault() + ",";
                                fan1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                gpusList.Add(Convert.ToString(fan1));
                                if (fanmin > fan1)
                                {
                                    fanmin = fan1;
                                }
                                if (fanmax < fan1)
                                {
                                    fanmax = fan1;
                                }

                            }


                            else if (sensor.SensorType == SensorType.Load)//LOAD
                            {
                                if (sensor.Name == "GPU Core")
                                {
                                    GlobalVars.load += sensor.Value.GetValueOrDefault() + ",";
                                    load1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                    gpusList.Add(Convert.ToString(load1));


                                    if (loadmin > load1)
                                    {
                                        loadmin = load1;
                                    }
                                }
                                else if (sensor.Name == "GPU Memory Controller")
                                {

                                }

                            }




                            //Проверки на перезагрузку
                            if (GlobalVars.reboot_load_GPU == "1")
                            {
                                if (loadmin <= GlobalVars.load_GPU_min)
                                {
                                    load_count = 1;
                                }
                                else
                                {
                                    load_count = 0;
                                }

                            }

                            if (GlobalVars.reboot_temp_max == "1")
                            {
                                if (tempmax >= GlobalVars.temp_max)
                                {

                                    max_count = 1;
                                }
                                else
                                {
                                    max_count = 0;
                                }

                            }
                            if (GlobalVars.reboot_temp_min == "1")
                            {
                                if (tempmin <= GlobalVars.temp_min)
                                {
                                    min_count = 1;

                                }
                                else
                                {
                                    min_count = 0;

                                }
                            }
                            if (GlobalVars.reboot_max_fan == "1")
                            {
                                if (fanmax >= GlobalVars.fan_max)
                                {

                                    fan_max_count = 1;

                                }
                                else
                                {
                                    fan_max_count = 0;

                                }
                            }
                            if (GlobalVars.reboot_min_fan == "1")
                            {
                                if (fanmin <= GlobalVars.fan_min)
                                {

                                    fan_min_count = 1;

                                }
                                else
                                {
                                    fan_min_count = 0;

                                }


                            }
                            if (GlobalVars.reload_file == "1")
                            {
                                if (tempmin <= GlobalVars.reload_temp_min_file)
                                {
                                    r_count = 1;
                                }
                                else
                                {
                                    r_count = 0;
                                }


                            }
                            if (GlobalVars.reboot_clock == "1")
                            {
                                if (clockmin < GlobalVars.clock_min)
                                {
                                    c_count = 1;
                                }
                                else
                                {
                                    c_count = 0;
                                }


                            }
                            if (GlobalVars.reboot_memory == "1")
                            {
                                if (memorymin < GlobalVars.mem_min)
                                {
                                    m_count = 1;
                                }
                                else
                                {
                                    m_count = 0;
                                }
                            }
                        }

                        GlobalVars.gpuList.Add(GlobalVars.counts, gpusList);
                    }


                }

                //ПРОВЕРКА

                if (GlobalVars.reboot_temp_max == "1")
                {
                    if (max_count > 0)
                    {

                        GPUTempMaxTimer.Enabled = true;
                        labelStatusTempMax.Text = MyStrings.labelStatusTempMax;
                        labelStatusTempMax.ForeColor = Color.Red;
                        labelCounterTempMax.Text = GlobalVars.timer_t_max.ToString();
                        labelCounterTempMax.ForeColor = Color.Red;

                    }
                    else
                    {
                        GPUTempMaxTimer.Enabled = false;
                        GlobalVars.timer_t_max = -100;

                        labelStatusTempMax.Text = "OK";
                        labelStatusTempMax.ForeColor = Color.Green;
                        labelCounterTempMax.Text = "";

                    }
                }
                else
                {
                    labelStatusTempMax.Text = MyStrings.labelEvent;
                    labelStatusTempMax.ForeColor = Color.Blue;
                    labelCounterTempMax.Text = "";
                }

                if (GlobalVars.reboot_temp_min == "1")
                {
                    if (min_count > 0)//min temp
                    {

                        labelStatusTempMin.Text = MyStrings.labelStatusTempMin;
                        labelStatusTempMin.ForeColor = Color.Red;
                        labelCounterTempMin.Text = GlobalVars.timer_t_min.ToString();
                        labelCounterTempMin.ForeColor = Color.Red;
                        GPUTempMinTimer.Enabled = true;

                    }
                    else
                    {

                        GlobalVars.timer_t_min = -100;
                        labelStatusTempMin.Text = "OK";
                        labelStatusTempMin.ForeColor = Color.Green;
                        labelCounterTempMin.Text = "";
                        GPUTempMinTimer.Enabled = false;

                    }
                }
                else
                {
                    labelStatusTempMin.Text = MyStrings.labelEvent;
                    labelStatusTempMin.ForeColor = Color.Blue;
                    labelCounterTempMin.Text = "";
                }

                if (GlobalVars.reboot_load_GPU == "1")
                {
                    if (load_count > 0)
                    {
                        labelStatusLoadGPU.Text = MyStrings.labelStatusLoadGPU;
                        labelStatusLoadGPU.ForeColor = Color.Red;
                        labelCounterLoadGPU.Text = GlobalVars.timer_load_gpu.ToString();
                        labelCounterLoadGPU.ForeColor = Color.Red;
                        GPULoadMin.Enabled = true;
                    }
                    else
                    {
                        GPULoadMin.Enabled = true;
                        GlobalVars.timer_load_gpu = -100;
                        labelStatusLoadGPU.Text = "OK";
                        labelStatusLoadGPU.ForeColor = Color.Green;
                        labelCounterLoadGPU.Text = "";
                    }

                }
                else
                {
                    labelStatusLoadGPU.Text = MyStrings.labelEvent;
                    labelStatusLoadGPU.ForeColor = Color.Blue;
                    labelCounterLoadGPU.Text = "";
                }

                if (GlobalVars.reload_file == "1")
                {
                    if (r_count > 0)
                    {
                        ReloadMinerTimer.Enabled = true;

                        labelStatusReloadFile.Text = MyStrings.labelStatusTempMin;
                        labelStatusReloadFile.ForeColor = Color.Red;
                        labelCounterReloadFile.Text = GlobalVars.timer_r_min.ToString();
                        labelCounterReloadFile.ForeColor = Color.Red;

                    }
                    else
                    {
                        ReloadMinerTimer.Enabled = false;
                        GlobalVars.timer_r_min = -100;
                        labelStatusReloadFile.Text = "ОК";
                        labelStatusReloadFile.ForeColor = Color.Green;
                        labelCounterReloadFile.Text = "";
                    }

                }
                else
                {
                    labelStatusReloadFile.Text = MyStrings.labelEvent;
                    labelStatusReloadFile.ForeColor = Color.Blue;
                    labelCounterReloadFile.Text = "";
                }

                if (GlobalVars.reboot_max_fan == "1")
                {
                    if (fan_max_count > 0)
                    {
                        GPUFanMaxTimer.Enabled = true;
                        labelStatusFanMax.Text = MyStrings.labelStatusFanMax;
                        labelStatusFanMax.ForeColor = Color.Red;
                        labelCounterFanMax.Text = GlobalVars.timer_fan_max.ToString();
                        labelCounterFanMax.ForeColor = Color.Red;
                    }
                    else
                    {
                        GPUFanMaxTimer.Enabled = false;
                        GlobalVars.timer_fan_max = -100;
                        labelStatusFanMax.Text = "ОК";
                        labelStatusFanMax.ForeColor = Color.Green;
                        labelCounterFanMax.Text = "";
                    }

                }
                else
                {
                    labelStatusFanMax.Text = MyStrings.labelEvent;
                    labelStatusFanMax.ForeColor = Color.Blue;
                    labelCounterFanMax.Text = "";
                }

                if (GlobalVars.reboot_min_fan == "1")
                {
                    if (fan_min_count > 0)
                    {
                        GPUFanMinTImer.Enabled = true;
                        labelStatusFanMin.Text = MyStrings.labelStatusFanMin;
                        labelStatusFanMin.ForeColor = Color.Red;
                        labelCounterFanMin.Text = GlobalVars.timer_fan_min.ToString();
                        labelCounterFanMin.ForeColor = Color.Red;
                    }
                    else
                    {
                        GPUFanMinTImer.Enabled = false;
                        GlobalVars.timer_fan_min = -100;
                        labelStatusFanMin.Text = "ОК";
                        labelStatusFanMin.ForeColor = Color.Green;
                        labelCounterFanMin.Text = "";
                    }
                }
                else
                {

                    labelStatusFanMin.Text = MyStrings.labelEvent;
                    labelStatusFanMin.ForeColor = Color.Blue;
                    labelCounterFanMin.Text = "";
                }
                //clock
                if (GlobalVars.reboot_clock == "1")
                {
                    if (c_count > 0)
                    {
                        GPUCoreMinTimer.Enabled = true;
                        labelStatusClock.Text = MyStrings.labelStatusClock;
                        labelStatusClock.ForeColor = Color.Red;
                        labelCounterClock.Text = GlobalVars.timer_clock.ToString();
                        labelCounterClock.ForeColor = Color.Red;
                    }
                    else
                    {
                        GPUCoreMinTimer.Enabled = false;
                        GlobalVars.timer_clock = -100;
                        labelStatusClock.Text = "ОК";
                        labelStatusClock.ForeColor = Color.Green;
                        labelCounterClock.Text = "";
                    }
                }
                else
                {
                    labelStatusClock.Text = MyStrings.labelEvent;
                    labelStatusClock.ForeColor = Color.Blue;
                    labelCounterClock.Text = "";
                }
                if (GlobalVars.reboot_memory == "1")
                {
                    if (m_count > 0)
                    {
                        GPUMemMinTimer.Enabled = true;
                        labelStatusMemory.Text = MyStrings.labelStatusMemory;
                        labelStatusMemory.ForeColor = Color.Red;
                        labelCounterMemory.Text = GlobalVars.timer_memory.ToString();
                        labelCounterMemory.ForeColor = Color.Red;
                    }
                    else
                    {
                        GPUMemMinTimer.Enabled = false;
                        GlobalVars.timer_memory = -100;
                        labelStatusMemory.Text = "ОК";
                        labelStatusMemory.ForeColor = Color.Green;
                        labelCounterMemory.Text = "";
                    }
                }
                else
                {
                    labelStatusMemory.Text = MyStrings.labelEvent;
                    labelStatusMemory.ForeColor = Color.Blue;
                    labelCounterMemory.Text = "";
                }

                if (GlobalVars.reboot_GPU == "1")
                {
                    if (GlobalVars.counts != GlobalVars.count_GPU)
                    {
                        FellOffGPUTimer.Enabled = true;
                        labelStatusFellOffGPU.Text = MyStrings.labelStatusFellOffGPU;
                        labelStatusFellOffGPU.ForeColor = Color.Red;
                        labelCounerFellOff.Text = GlobalVars.timer_t_card.ToString();
                        labelCounerFellOff.ForeColor = Color.Red;
                    }
                    else
                    {
                        FellOffGPUTimer.Enabled = false;
                        GlobalVars.timer_t_card = -100;
                        labelStatusFellOffGPU.Text = "OK";
                        labelStatusFellOffGPU.ForeColor = Color.Green;
                        labelCounerFellOff.Text = "";
                    }
                }
                else
                {
                    labelStatusFellOffGPU.Text = MyStrings.labelEvent;
                    labelStatusFellOffGPU.ForeColor = Color.Blue;
                    labelCounerFellOff.Text = "";
                }




                if (GlobalVars.reboot_internet == "1")
                {
                    if (!GlobalVars.InternetIsActive)
                    {
                        DontHaveInternetTimer.Enabled = true;
                        labelStatusInternet.Text = MyStrings.labelStatusInternet;
                        label3CounterInternet.Text = GlobalVars.timer_inet.ToString();
                        labelStatusInternet.ForeColor = Color.Red;
                    }
                    else
                    {
                        DontHaveInternetTimer.Enabled = false;
                        labelStatusInternet.Text = "OK";
                        labelStatusInternet.ForeColor = Color.Green;
                        label3CounterInternet.Text = "";
                    }


                }
                else
                {
                    labelStatusInternet.Text = MyStrings.labelEvent;
                    labelStatusInternet.ForeColor = Color.Blue;
                    label3CounterInternet.Text = "";
                }
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Get sensors: " + ex.Message, "error");
            }
        }

        //public void SendData()
        public static async Task SendData()
        {
           

            try
            {

                GlobalVars.upTime = UpTime.ToString(@"dd\.hh\:mm\:ss");


                //labelTest.Text = "";
                /*
                int i = 0;
                foreach (KeyValuePair<int, List<String>> keyValue in GlobalVars.gpuList)
                {
                    labelTest.Text = "";
                    int j = 0;
                    foreach (String p in keyValue.Value)
                    {

                        switch (j)
                        {
                            case 0:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/name", Encoding.UTF8.GetBytes(p));
                                break;
                            case 1:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/temp", Encoding.UTF8.GetBytes(p));
                                break;
                            case 2:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/core", Encoding.UTF8.GetBytes(p));
                                break;
                            case 3:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/memory", Encoding.UTF8.GetBytes(p));
                                break;
                            case 4:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/load", Encoding.UTF8.GetBytes(p));
                                break;
                            case 5:
                                GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/fan", Encoding.UTF8.GetBytes(p));
                               // GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/common/uptime/" + i + "/fan", Encoding.UTF8.GetBytes(p));
                                break;




                        }

                        // Console.WriteLine(p.Name);
                        labelTest.Text += " " + p;
                        j++;


                    }


                     //   GlobalVars.mqttClient.Publish("Pi/LEDControl2", Encoding.UTF8.GetBytes("SEND: " + labelTest.Text));

                    labelTest.Text = labelTest.Text + "\n";
                    i++;

                }
                */

                var send_data = new MqttApplicationMessageBuilder()
                     .WithTopic("devices/" + GlobalVars.token + "/data")
                     .WithPayload("token=" + GlobalVars.token +
                        "&gpu=" + GlobalVars.card +
                        "&temp=" + GlobalVars.temp +
                        "&fan=" + GlobalVars.fan +
                        "&start_timestamp=" + GlobalVars.start_timestamp.ToString() +
                        "&v=" + GlobalVars.versions +
                        "&load=" + GlobalVars.load +
                        "&clock=" + GlobalVars.clock +
                        "&mem=" + GlobalVars.mem +
                        "&upTime=" + GlobalVars.upTime)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                 await   GlobalVars.mqttClient.PublishAsync(send_data);

                

            }
            catch (Exception ex)
            {
                 
                Debug.WriteLine("Send data: " + ex.Message + GlobalVars.json_send);

            }

        }

    
        


        public void СheckForNewVersion()
        {
            if (GlobalVars.InternetIsActive == true)
            {
                try
                {
                    string v = GlobalVars._manager.GetPrivateString("main", "version");
                    string pack = _http.GetContent(GlobalVars.host + "/api/?method=version");
                    VersionResponse m = JsonConvert.DeserializeObject<VersionResponse>(pack);
                    string ver = m.version;
                    GlobalVars.link = m.link;

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
            else {
                 linkLabelUpdate.Visible = false;
                _log.writeLogLine("Internet is Off ", "log");
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

        private void UptimeTimerTick(object sender, EventArgs e)
        {
            GlobalVars.timeOnline = GlobalVars.timeOnline + 1;
            int min = GlobalVars.timeOnline;
            int d = min / 1440;
            min = min - (d * 1440);
            int h = min / 60;
            min = min - (h * 60);
            // int sec = min - (min * 60);
            labelTimeWork2.Text = ("Days: " + d.ToString() + " Hours: " + h.ToString() + " Minute: " + min.ToString());
            //labelTimeWork2.Text = ("Days: " + d.ToString() + " Hours: " + h.ToString() + " Minute: " + min.ToString() + " Second: " + sec.ToString());
        }

        public void Reboot(string msg, string bat)
        {
            try
            {
                // GetTempretureTimer.Enabled = false;
               
                GPUTempMinTimer.Enabled = false;
                _http.GetContent(
                    GlobalVars.host +
                    "/api.php?token=" + GlobalVars.token +
                    "&event=" + "reboot" +
                    "&reason="  + GlobalVars.name + " " + msg
                    /*
                    "&fan=" + GlobalVars.fan +
                    "&start_timestamp=" + GlobalVars.start_timestamp.ToString() +
                    "&v=" + GlobalVars.versions +
                    "&load=" + GlobalVars.load +
                    "&clock=" + GlobalVars.clock +
                     "&mem=" + GlobalVars.mem +
                   
                    "&hash=" + "417"
                    */
                    );

                _log.writeLogLine("Reboot rig " + GlobalVars.name + " " + msg, "log");

                 Process.Start(bat);
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Reboot: " + ex.Message, "error");
            }
        }


        //send event message
        public void Message(string msg)
        {
            
            
            try
            {
                _http.GetContent(
                    GlobalVars.host +
                    "/api.php?token=" + GlobalVars.token +
                    "&event=" + "message" +
                    "&reason=" + GlobalVars.name + " " + msg
                    );

                _log.writeLogLine("Message " + msg, "log");
            }

            catch (Exception ex)
            {
                _error.writeLogLine("Send Message: " + ex.Message, "error");
            }


        }

        public void Reload(string msg)
        {
            try
            {
                ReloadMinerTimer.Enabled         = false;
                GlobalVars.timer_r_min = -100;
                var ppsi               = Process.Start("cmd", @"/c taskkill /f /im " + GlobalVars.filename);
                ppsi.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled         = false;
                GlobalVars.timer_r_min = -100;
                var psiw               = Process.Start("cmd", @"/c taskkill /f /im conhost.exe");
                psiw.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled         = false;
                GlobalVars.timer_r_min = -100;
                var psi                = Process.Start("cmd", @"/c taskkill /f /im cmd.exe");
                psi.Close();
                System.Threading.Thread.Sleep(1000);

                ReloadMinerTimer.Enabled = false;
                GlobalVars.timer_r_min = -100;
                System.Threading.Thread.Sleep(1000);
                Process.Start("nice.bat");
                ReloadMinerTimer.Enabled = false;
                GlobalVars.timer_r_min = -100;
                System.Threading.Thread.Sleep(1500);
                ProcessStartInfo rpsi;
                rpsi = new ProcessStartInfo
                {
                    WorkingDirectory = GlobalVars.dir2,
                    FileName = GlobalVars.pathreload2
                };
                System.Threading.Thread.Sleep(1000);

                Process.Start(rpsi);/**/

                string pack = _http.GetContent(GlobalVars.host + 
                    "/api.php?&worker=" + GlobalVars.name + 
                    "&gpu=" + GlobalVars.card + 
                    "&temp=" + GlobalVars.temp + 
                    "&fan=" + GlobalVars.fan + 
                    "&status=reload" + 
                    "&msg=" + msg);
                ReloadMinerTimer.Enabled = false;
                GlobalVars.timer_r_min = -100;
            }
            catch (Exception ex)
            {
                _error.writeLogLine("Reload: " + ex.Message, "error");
            }
            }
        public void GetPoolInfo(string pool)
        {

            if (pool == "nanozec")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/zec/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                    }
                }

            }
            else if (pool == "nanoetc")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/etc/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                    }
                }
            }
            else if (pool == "nanoeth")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/eth/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                        // MessageBox.Show(item.hashrate.ToString());
                    }
                }
            }
            else if (pool == "nanosia")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/sia/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                        // MessageBox.Show(item.hashrate.ToString());
                    }
                }
            }
            else if (pool == "nanoxmr")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/xmr/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                        // MessageBox.Show(item.hashrate.ToString());
                    }
                }
            }
            else if (pool == "nanopasc")
            {
                string pack = _http.GetContent("https://api.nanopool.org/v1/pasc/workers/" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
                foreach (var item in m.data)
                {
                    if (item.id == GlobalVars.name)
                    {
                        labelStatusTempMax.Text = item.hashrate.ToString();
                        // MessageBox.Show(item.hashrate.ToString());
                    }
                }
            }
            else if (pool == "nicehash")
            {
                string pack = _http.GetContent("https://api.nicehash.com/api?method=stats.provider.ex&addr=" + GlobalVars.wallet);//свой кошель
                PoolInfoResponse m = JsonConvert.DeserializeObject<PoolInfoResponse>(pack);
            }
        }






        private void BtStartClick(object sender, EventArgs e)
        {


            



            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;


            // string name = tbRigName.Text;
            //string token = tbToken.Text;

            if (!string.IsNullOrEmpty(tbToken.Text))
            {

                MqttConnect();


                    _log.writeLogLine("Informer is started ", "log");
                    Message("Informer Started!");
                    GlobalVars._manager.WritePrivateString("main", "token", tbToken.Text);
                    GetTempretureTimer.Enabled = true;
                    // PingInternetTimer.Start();
                    SendDataTimer.Enabled = true;
                    btStart.Enabled = false;
                    btStop.Visible = true;
                    AutoStartTimer.Enabled = false;
                    GlobalVars.token = tbToken.Text;
                    GlobalVars.name = tbRigName.Text;
                    gpu_temp();
                  //  SendData();
                    GlobalVars.timeOnline = 0;
                    //InformationLabel.Text = "Запущен";
                    //InformationLabel.ForeColor = Color.Green;
                    tbToken.ReadOnly = true;

                }
            /*
                else
                {

                    _log.writeLogLine("Informer is started ", "log");
                    GetTempretureTimer.Enabled = true;
                    // PingInternetTimer.Start();
                    SendDataTimer.Enabled = false;
                    btStart.Enabled = false;
                    btStop.Visible = true;
                    AutoStartTimer.Enabled = false;
                    GlobalVars.token = tbToken.Text;
                    GlobalVars.name = tbRigName.Text;
                    gpu_temp();
                    SendData();
                    GlobalVars.timeOnline = 0;
                    //InformationLabel.Text = "Запущен";
                    //InformationLabel.ForeColor = Color.Green;
                    tbToken.ReadOnly = true;
                }

            */

            






            else
            {
                /*
                if (string.IsNullOrEmpty(tbToken.Text))
                {


                    GlobalVars._manager.WritePrivateString("main", "token", tbToken.Text);
                    GetTempretureTimer.Enabled = true;

                    SendDataTimer.Enabled = true;
                    btStart.Enabled = false;
                    btStop.Visible = true;
                    AutoStartTimer.Enabled = false;

                    GlobalVars.token = tbToken.Text;
                    GlobalVars.name = tbRigName.Text;
                    gpu_temp();


                    _log.writeLogLine("Informer is started ", "log");


                    SendData();


                    GlobalVars.timeOnline = 0;
                   // InformationLabel.Text = "Запущен";
                    InformationLabel.ForeColor = Color.Green;
                    tbEmail.ReadOnly = true;
                    tbSecret.ReadOnly = true;
                    tbRigName.ReadOnly = true;
                    tbToken.ReadOnly = true;

                   
                }
                
                else
                {
                   

                }
                */
                MessageBox.Show("Enter the token!");
            }
              
          
        }

        private void BtnOpenSettingsFormClick(object sender, EventArgs e)
        {
            NextAutoStart.Stop();
            AutoStartTimer.Stop();
            f2.ShowDialog();

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

        private void TempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp MAX, Reboot!";
            string bat = "reboot_t_max.bat";
            if (GlobalVars.timer_t_max < 0) {
                GlobalVars.timer_t_max = GlobalVars.time_temp_max;
            }

            if (GlobalVars.timer_t_max == 0)
            {
                if (!GlobalVars.reboot1)
                {
                    Reboot(msg, bat);
                  //  GlobalVars.reboot1 = true;
                }
              
            }
            GlobalVars.timer_t_max = GlobalVars.timer_t_max - 1;
        }

        private void LowTempretureTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Reboot!";
            string bat = "reboot_t_min.bat";
            if (GlobalVars.timer_t_min < 0)
                {
                    GlobalVars.timer_t_min = GlobalVars.time_temp_min;
                }
                if (GlobalVars.timer_t_min == 0)
                {
                if (!GlobalVars.reboot2)
                {
                   // if (GlobalVars.InternetIsActive)
                   // {
                        Reboot(msg, bat);
                       // GlobalVars.reboot2 = true;
                  //  }
                  //  else
                  //  {
                     //   GlobalVars.timer_t_min = -100;
                        
                  //  }
                }
            }
                GlobalVars.timer_t_min = GlobalVars.timer_t_min - 1;
        }

        private void RerunTimerTick(object sender, EventArgs e)
        {
            string msg = "Temp Min, Application Reboot " + GlobalVars.filename2 +"!";
            if (GlobalVars.timer_r_min < 0)
                {
                    GlobalVars.timer_r_min = GlobalVars.reload_time_min_file;
                }
                if (GlobalVars.timer_r_min == 0)
                {
                    Reload(msg);
            }
            GlobalVars.timer_r_min = GlobalVars.timer_r_min - 1;
        }

        private void FanMaxTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Max, Reboot! ";
            const string bat = "reboot_fan_max.bat";
            if (GlobalVars.timer_fan_max < 0)
            {
                GlobalVars.timer_fan_max = GlobalVars.time_fan_max;
            }
            if (GlobalVars.timer_fan_max == 0)
            {
                if (!GlobalVars.reboot3)
                {
                    Reboot(msg, bat);
                 //   GlobalVars.reboot3 = true;
                }
            }
            GlobalVars.timer_fan_max = GlobalVars.timer_fan_max - 1;
        }

        private void FanMinTimerTick(object sender, EventArgs e)
        {
            const string msg = "FAN Min, Reboot!";
            const string bat = "reboot_fan_min.bat";
            if (GlobalVars.timer_fan_min < 0)
            {
                GlobalVars.timer_fan_min = GlobalVars.time_fan_min;
            }
            if (GlobalVars.timer_fan_min == 0)
            {
                if (!GlobalVars.reboot4)
                {
                   
                        Reboot(msg, bat);
                 //       GlobalVars.reboot4 = true;
                   
                }
            }
            GlobalVars.timer_fan_min = GlobalVars.timer_fan_min - 1;
        }

        private void SendDataTimerTick(object sender, EventArgs e)
        {
            //СheckForMQTT();
            SendData();
            
        }

        

        private void GpuCoreMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Min , Reboot!";
            const string bat = "reboot_clock.bat";
            if (GlobalVars.timer_clock < 0)
            {
                GlobalVars.timer_clock = GlobalVars.time_clock_min;
            }
            if (GlobalVars.timer_clock == 0)
            {
                if (!GlobalVars.reboot5)
                {
                   // if (GlobalVars.InternetIsActive)
                   // {
                        Reboot(msg, bat);
                  //      GlobalVars.reboot5 = true;
                   // }
                  //  else
                  //  {
                  //      GlobalVars.timer_clock = -100;
                  //  }
                }
            }
            GlobalVars.timer_clock = GlobalVars.timer_clock - 1;
        }

        private void GpuMemoryMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Memory Min, Reboot!";
            const string bat = "reboot_memory.bat";
            if (GlobalVars.timer_memory < 0)
            {
                GlobalVars.timer_memory = GlobalVars.time_mem_min;
            }
            if (GlobalVars.timer_memory == 0)
            {
                if (!GlobalVars.reboot6)
                {
                   // if (GlobalVars.InternetIsActive)
                  //  {
                        Reboot(msg, bat);
                   //     GlobalVars.reboot6 = true;
                  //  }
                  //  else
                  //  {
                  //      GlobalVars.timer_memory = -100;
                  //  }
                }
            }
            GlobalVars.timer_memory = GlobalVars.timer_memory - 1;
        }


        public void СheckForMQTT()
        {
            //GlobalVars.mqttcheck = 0;
            try
            {
                /*
              //  bool code = GlobalVars.mqttClient.IsConnected;
                

                if (code == true)
                {
                    labelStatusInternetPing.Text = "MQTT ON";

                    //GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("OK"));
                   
                    //GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/UpTime", Encoding.UTF8.GetBytes("" + GlobalVars.upTime));


                }
                else if (code == false)
                {
                    //  labelStatusInternetPing.Text = "MQTT OFF";
                 //   MqttConnect();
                    if (GlobalVars.mqttClient.IsConnected == true) {
                 //   GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/init", Encoding.UTF8.GetBytes("1"));
                        labelTest.Text = "GOOD";
                   // GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("Reconnected"));
                   // GlobalVars.mqttClient.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("ON"));
                        }
                  
                
                }
                */


            }
            catch (Exception ex)
            {

            }

        }
        /*
        public void СheckForPing()
        {
            try
            {
                using (Ping ping = new System.Net.NetworkInformation.Ping())
                {
                    PingReply pingReply = null;
                    int timeout = 1000;
                    pingReply = ping.Send("8.8.8.8", timeout);

                    if (pingReply.Status == IPStatus.Success)
                    {
                        GlobalVars.InternetIsActive = true;
                        labelStatusInternetPing.Text = "OK";
                        labelStatusInternetPing.ForeColor = Color.Green;


                    }
                    else
                    {
                        _log.writeLogLine(" INTERNET OFF", "log");
                        labelStatusInternetPing.Text = "No access to the Internet";
                        labelStatusInternetPing.ForeColor = Color.Red;
                        GlobalVars.InternetIsActive = false;

                    }


                }

            }

            catch (Exception ex)
            {

                _error.writeLogLine(ex.Message + " INTERNET OFF", "error");
                labelStatusInternetPing.Text = "No access to the Internet";
                labelStatusInternetPing.ForeColor = Color.Red;
                GlobalVars.InternetIsActive = false;
            }

        }
        */
/*
      private void PingTimerTick(object sender, EventArgs e)
          {
           
            СheckForMQTT();

          }
*/
        private void InternetInactiveTimerTick(object sender, EventArgs e)
        {
            const string bat = "reboot_internet.bat";

            DontHaveInternetTimer.Enabled = false;
            try
            {
                if (GlobalVars.timer_inet < 0)
                {
                    GlobalVars.timer_inet = GlobalVars.time_lost_inet;
                }
                if (GlobalVars.timer_inet == 0)
                {
                    if (!GlobalVars.rebootDontHaveInternet)
                    {
                        if (!GlobalVars.InternetIsActive)
                        {
                            Process.Start(bat);
                        }
                        else
                        {
                            GlobalVars.timer_inet = -100;
                        }
                    }
                }
                GlobalVars.timer_inet = GlobalVars.timer_inet - 1;
            }
            finally
            {
                DontHaveInternetTimer.Enabled = false;
            }

        }

        private void FellOffTimerTick(object o, EventArgs e)
        {
            const string msg = "GPU fell, Reboot!";
            const string bat = "reboot_card.bat";

            FellOffGPUTimer.Enabled = false;
            try
            {
                if (GlobalVars.timer_t_card < 0)
                {
                    GlobalVars.timer_t_card = GlobalVars.time_count_GPU;
                }
                if (GlobalVars.timer_t_card == 0)
                {
                    if (!GlobalVars.IsRebootStarted)
                    {
                      //  if (GlobalVars.InternetIsActive)
                     //   {
                            Reboot(msg, bat);
                        //    GlobalVars.IsRebootStarted = true;
                     //   }
                     //   else
                     //   {
                     //       GlobalVars.timer_t_card = -100;
                     //   }
                    }
                }
                GlobalVars.timer_t_card = GlobalVars.timer_t_card - 1;
            }
            finally
            {
                FellOffGPUTimer.Enabled = false;
            }

        }

        private void GPULoadMin_Tick(object sender, EventArgs e)
        {
            const string msg = "GPU Load Min, Reboot!";
            const string bat = "reboot_load.bat";

            GPULoadMin.Enabled = false;
            try
            {
                if (GlobalVars.timer_load_gpu < 0)
                {
                    GlobalVars.timer_load_gpu = GlobalVars.time_load_GPU_min;
                }
                if (GlobalVars.timer_load_gpu == 0)
                {
                    if (!GlobalVars.rebootLoadGPU)
                    {
                       Reboot(msg, bat);
                    }
                }
                GlobalVars.timer_load_gpu = GlobalVars.timer_load_gpu - 1;
            }
            finally
            {
                GPULoadMin.Enabled = false;
            }
        }

        private void GetEWBF_ZcashTimer_Tick(object sender, EventArgs e)
        {
            GetEWBF_ZcashAPI();
        }

        private void GetEWBF_ZcashAPI()
        {
            throw new NotImplementedException();
        }

        

/*        
private void tbRigName_TextChanged(object sender, EventArgs e)
{
   GlobalVars.name = tbRigName.Text;
   GlobalVars._manager.WritePrivateString("main", "name", GlobalVars.name);
}

*/
/*
private void tbSecret_TextChanged(object sender, EventArgs e)
{
   GlobalVars.secret = tbSecret.Text;
   GlobalVars._manager.WritePrivateString("main", "secret", GlobalVars.secret);
}

    */
    /*
private void tbEmail_TextChanged(object sender, EventArgs e)
{
   GlobalVars.email = tbEmail.Text;
   GlobalVars._manager.WritePrivateString("main", "email", GlobalVars.email);
}

    */

        private void tbToken_TextChanged(object sender, EventArgs e)
        {
            GlobalVars.token = tbToken.Text;
            GlobalVars._manager.WritePrivateString("main", "token", GlobalVars.token);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            

            //bool code = GlobalVars.mqttClient.IsConnected;

            try
            {
                MqttStop();
                
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
            _log.writeLogLine("Informer is started ","log");
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
       


/*

    //async Task
    public void ReceiveOld(string payload, string topic)
    {

        if (topic == "devices/" +GlobalVars.token+ "/commands")
        {

           // Debug.WriteLine(message);
            var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
            string command = response.command;
            switch (command)
            {

                case "reboot":

                    Message("Informer Reboot from Allminer.ru!");
                    Process psiwer;
                    psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                    psiwer.Close();
                    break;

                case "settings":
                    try
                    {
                        //write timers to ini
                        GlobalVars.time_temp_min = response.Params.timers.temp_min;
                        GlobalVars._manager.WritePrivateString("main",nameof(GlobalVars.time_temp_min), Convert.ToString(response.Params.timers.temp_min));

                        GlobalVars.time_temp_max = response.Params.timers.temp_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_temp_max), Convert.ToString(response.Params.timers.temp_max));

                        GlobalVars.time_fan_min = response.Params.timers.fan_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_fan_min), Convert.ToString(response.Params.timers.fan_min));

                        GlobalVars.time_fan_max = response.Params.timers.fan_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_fan_max), Convert.ToString(response.Params.timers.fan_max));

                        GlobalVars.time_load_GPU_min = response.Params.timers.load_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_load_GPU_min), Convert.ToString(response.Params.timers.load_min));

                        GlobalVars.time_load_GPU_max = response.Params.timers.load_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_load_GPU_max), Convert.ToString(response.Params.timers.load_max));

                        GlobalVars.time_clock_min = response.Params.timers.clock_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_clock_min), Convert.ToString(response.Params.timers.clock_min));


                        GlobalVars.time_clock_max = response.Params.timers.clock_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_clock_max), Convert.ToString(response.Params.timers.clock_max));

                        GlobalVars.time_mem_min = response.Params.timers.mem_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_mem_min), Convert.ToString(response.Params.timers.mem_min));

                        GlobalVars.time_mem_max = response.Params.timers.mem_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_mem_max), Convert.ToString(response.Params.timers.mem_max));

                        GlobalVars.time_lost_gpu = response.Params.timers.lost_gpu;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_lost_gpu), Convert.ToString(response.Params.timers.lost_gpu));

                        GlobalVars.time_lost_inet = response.Params.timers.lost_inet;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_lost_inet), Convert.ToString(response.Params.timers.lost_inet));

                        //write reboots flag to ini

                        GlobalVars.reboots_temp_min = response.Params.reboots.temp_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_temp_min), Convert.ToString(response.Params.reboots.temp_min));

                        GlobalVars.reboots_temp_max = response.Params.reboots.temp_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_temp_max), Convert.ToString(response.Params.reboots.temp_max));

                        GlobalVars.reboots_fan_min = response.Params.reboots.fan_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_fan_min), Convert.ToString(response.Params.reboots.fan_min));

                        GlobalVars.reboots_fan_max = response.Params.reboots.fan_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_fan_max), Convert.ToString(response.Params.reboots.fan_max));

                        GlobalVars.reboots_load_min = response.Params.reboots.load_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_load_min), Convert.ToString(response.Params.reboots.load_min));

                        GlobalVars.reboots_load_max = response.Params.reboots.load_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_load_max), Convert.ToString(response.Params.reboots.load_max));

                        GlobalVars.reboots_clock_min = response.Params.reboots.clock_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_clock_min), Convert.ToString(response.Params.reboots.clock_min));

                        GlobalVars.reboots_clock_max = response.Params.reboots.clock_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_clock_max), Convert.ToString(response.Params.reboots.clock_max));

                        GlobalVars.reboots_mem_min = response.Params.reboots.mem_min;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_mem_min), Convert.ToString(response.Params.reboots.mem_min));

                        GlobalVars.reboots_mem_max = response.Params.reboots.mem_max;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_mem_max), Convert.ToString(response.Params.reboots.mem_max));

                        GlobalVars.reboots_lost_gpu = response.Params.reboots.lost_gpu;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_lost_gpu), Convert.ToString(response.Params.reboots.lost_gpu));

                        GlobalVars.reboots_lost_inet = response.Params.reboots.lost_inet;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_lost_inet), Convert.ToString(response.Params.reboots.lost_inet));

                        //write data_ranges to ini

                        GlobalVars.temp_min = response.Params.data_ranges.Temp[0];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.temp_min), Convert.ToString(response.Params.data_ranges.Temp[0]));

                        GlobalVars.temp_max = response.Params.data_ranges.Temp[1];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.temp_max), Convert.ToString(response.Params.data_ranges.Temp[1]));

                        GlobalVars.mem_min = response.Params.data_ranges.Mem[0];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.mem_min), Convert.ToString(response.Params.data_ranges.Mem[0]));

                        GlobalVars.mem_max = response.Params.data_ranges.Mem[1];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.mem_max), Convert.ToString(response.Params.data_ranges.Mem[1]));

                        GlobalVars.load_GPU_min = response.Params.data_ranges.Load[0];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.load_GPU_min), Convert.ToString(response.Params.data_ranges.Load[0]));

                        GlobalVars.load_GPU_max = response.Params.data_ranges.Load[1];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.load_GPU_max), Convert.ToString(response.Params.data_ranges.Load[1]));


                        GlobalVars.fan_min = response.Params.data_ranges.Fan[0];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.fan_min), Convert.ToString(response.Params.data_ranges.Fan[0]));

                        GlobalVars.fan_max = response.Params.data_ranges.Fan[1];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.fan_max), Convert.ToString(response.Params.data_ranges.Fan[1]));


                        GlobalVars.clock_min = response.Params.data_ranges.Clock[0];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.clock_min), Convert.ToString(response.Params.data_ranges.Clock[0]));

                        GlobalVars.clock_max = response.Params.data_ranges.Clock[1];
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.clock_max), Convert.ToString(response.Params.data_ranges.Clock[1]));

                        GlobalVars.name = response.Params.name;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.name), Convert.ToString(response.Params.name));
                        tbRigName.Text = GlobalVars.name;

                        GlobalVars.time_start = response.Params.interval;
                        GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_start), Convert.ToString(response.Params.interval));


                        SendDataTimer.Interval = response.Params.interval * 1000;



                        //SendDataTimer.Interval = 2 * 1000;


                    }
                    catch (Exception ex)
                    {
                        _error.writeLogLine("Receive:" + ex.Message, "error_settings");

                    }

                    break;

            }


        }

    */
    }


        

}




