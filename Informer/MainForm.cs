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
using MQTTnet.Implementations;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MqttClientConnectedEventArgs = MQTTnet.Client.MqttClientConnectedEventArgs;
using MqttClientDisconnectedEventArgs = MQTTnet.Client.MqttClientDisconnectedEventArgs;
using System.Threading;

namespace Informer
{
    public partial class MainForm : Form
    {


        private Computer _pc;
        private static Http _http = new Http();
        private LogFile _log, _error;
        //List<String> gpusList = new List<string>();

        private Form f2;





        public MainForm()
        {


            GlobalVars.gpuList = new Dictionary<int, List<string>>();

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            }


            InitializeComponent();



            try
            {
                GlobalVars._manager.WritePrivateString("main", "version", "1.3.9");
            }
            catch (Exception e)
            {
                _error.writeLogLine("Write version: " + e.Message, "error");
            }

            MyIDProcess();
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

            //Инициализация компонентов
            InitFromIni.onInitFromIni();


            f2 = new SettingsForm();


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
                NextAutoStart.Interval = GlobalVars.autostart * 1000;
                NextAutoStart.Enabled = true;
                AutoStartTimer.Enabled = true;
                TimeWorkTimer.Enabled = true;
            }


        }

        private void MyIDProcess()
        {
            try
            {
                int myId = Process.GetCurrentProcess().Id;
                Debug.WriteLine("My ID: " + myId);
                Process.GetProcesses().Where(p => p.ProcessName == "Informer" && p.Id != myId).Count(p => { p.Kill(); return true; });

                /*
                string[] pr_names = { "Informer" };


                foreach (Process ps in Process.GetProcesses())
                {
                    if (pr_names.Contains(ps.ProcessName) && ps.Id != myId)
                    {
                
                        ps.Kill();
                    }

                


                    //(new string[] { "ps1", "ps2" }).SelectMany(name => Process.GetProcessesByName(name)).Count(p => { p.Kill(); return true; });


                    //  (new string[] { "ps1", "ps2" }).SelectMany(name => Process.GetProcessesByName(name)).ToList().ForEach(p => p.Kill());
                }
                */
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }  
          
            
          


          
        }

        private void BtStopClick(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVars.mqttClient.IsConnected)
                {
                    GlobalVars.mqttClient.DisconnectAsync();

                }
            }
            catch (Exception ex) {


            }
            /*
            try
            {
                MqttConnect.RunAsync(GlobalVars.cancelTokenSource.Token);
                GlobalVars.cancelTokenSource.Cancel();
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                GlobalVars.cancelTokenSource.Dispose();
            }
            */
            //            MqttConnect.RunAsync(GlobalVars.tokenMqtt);

            _log.writeLogLine("Informer stopped", "log");
            Message("Informer Stopped!");

            //SendDataTimer.
            SendDataTimer.Enabled = false;
            GetTempretureTimer.Enabled = false;
            GlobalVars.mqttIsConnect = false;



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
            GPUFanMinTimer.Enabled = false;
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
            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            if (GlobalVars.mqttClient.IsConnected && GlobalVars.mqttIsConnect == false)
            {
                _log.writeLogLine("Informer is started ", "log");
                Message("Informer Started!");
                GlobalVars.token = tbToken.Text;
                GlobalVars._manager.WritePrivateString("main", "token", tbToken.Text);
                SendDataTimer.Enabled = true;
                GPUTemp.GetGPU();
                //gpu_temp();


                // PingInternetTimer.Start();

                btStart.Enabled = false;
                btStop.Visible = true;
                AutoStartTimer.Enabled = false;

                GlobalVars.timeOnline = 0;
                tbToken.ReadOnly = true;
                tbRigName.Text = GlobalVars.name;
                GlobalVars.mqttIsConnect = true;
                InformationLabel.Text = MyStrings.labelInformationAuthorizationOK;
                InformationLabel.ForeColor = Color.Green;
                labelTest.Text = Convert.ToString(GlobalVars.gpuList.Count);
            }
            else if (!GlobalVars.mqttClient.IsConnected)
            {
                InformationLabel.Text = MyStrings.labelInformationAuthorizationFailed;
                InformationLabel.ForeColor = Color.Red;
                GPUTemp.GetGPU();
                MqttConnect.RunAsync();
                GlobalVars.mqttIsConnect = false;

            }
            

            try
            {



                btStart.Enabled = false;
                NextAutoStart.Enabled = false;
                AutoStartTimer.Enabled = false;
                btStop.Visible = true;
                GPUTemp.GetGPU();
                labelTest.Text = Convert.ToString(GlobalVars.gpusList.Count);
                GlobalVars.gpuList.Add(GlobalVars.gpusList.Count, GlobalVars.gpusList);
                GpuStatus();
                //gpu_temp();

            }
            catch (Exception ex)
            {
                _error.writeLogLine("GetTempTimer: " + ex.Message, "error");
            }

            //GPUTemp.GetGPU();
            
            
        }




        private void NextAutoStart_Tick(object sender, EventArgs e)
        {

            MqttConnect.RunAsync();
            GetTempretureTimer.Enabled = true;

            //SendDataTimer.Enabled = true;
            AutoStartTimer.Enabled = false;
            btStop.Visible = true;
            btStart.Enabled = false;
            GlobalVars.timeOnline = 0;
            tbEmail.ReadOnly = true;
            tbSecret.ReadOnly = true;
            tbRigName.ReadOnly = true;
            tbToken.ReadOnly = true;
            InformationLabel.Text = "Запущен";
            InformationLabel.ForeColor = Color.Green;
            //  GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            Hide();


        }

        private void AutoStart_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GlobalVars.token))
            {
                tbToken.ReadOnly = true;
                NextAutoStart.Start();
                AutoStartTimer.Start();
                GlobalVars.autostart = GlobalVars.autostart - 1;
                btStart.Text = MyStrings.btStart + "(" + GlobalVars.autostart.ToString() + ")";

            }
            else {

                NextAutoStart.Start();
                AutoStartTimer.Start();
                GlobalVars.autostart = GlobalVars.autostart - 1;
                btStart.Text = MyStrings.btStart + "(" + GlobalVars.autostart.ToString() + ")";
            }

        }


/*
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
                GlobalVars.gpusList.Clear();
                GlobalVars.gpuList.Clear();
                foreach (var hard in _pc.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
                {

                    hard.Update();


                    if (hard.HardwareType == HardwareType.GpuAti || hard.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                    {

                        GlobalVars.counts = GlobalVars.counts + 1;
                        GlobalVars.card += hard.Name + ",";
                        GlobalVars.gpusList.Add(hard.Name);

                        foreach (var sensor in hard.Sensors)//ИДЕМ по сенсорам
                        {


                            if (sensor.SensorType == SensorType.Clock)
                            {//ЧАСТОТЫ



                                if (sensor.Name == "GPU Core")//ЯДРО
                                {

                                    GlobalVars.clock += sensor.Value.GetValueOrDefault() + ";";
                                    clockk1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());

                                    GlobalVars.gpusList.Add(Convert.ToString(clockk1));

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
                                        GlobalVars.gpusList.Add(Convert.ToString(mem1));

                                    }

                                }
                                else if (hard.HardwareType == HardwareType.GpuNvidia)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        mem1 = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                                        GlobalVars.gpusList.Add(Convert.ToString(mem1));
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
                                GlobalVars.gpusList.Add(Convert.ToString(temp1));

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
                                GlobalVars.gpusList.Add(Convert.ToString(fan1));
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
                                    GlobalVars.gpusList.Add(Convert.ToString(load1));


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

                        GlobalVars.gpuList.Add(GlobalVars.counts, GlobalVars.gpusList);
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

            */
        /*
                public static async Task SendData()
                {

                }
        */


        public void GpuStatus()
        {

            int i = 0;
            foreach (KeyValuePair<int, List<String>> keyValue in GlobalVars.gpuList)
            {
               // labelTest.Text = "";
                int j = 0;
                foreach (String p in keyValue.Value)
                {

                    switch (j)
                    {
                        case 0:
                            
                          
                            break;
                        case 1:
                           
                            //temp min
                            if (GlobalVars.reboots_temp_min == false)
                            {
                               // if (labelStatusTempMin.Text != MyStrings.labelEvent)
                               // {
                                    labelStatusTempMin.Text = MyStrings.labelEvent;
                                    labelStatusTempMin.ForeColor = Color.Blue;
                                    labelCounterTempMin.Visible = false;
                                    GlobalVars.timer_t_min = -100;
                                //  }

                            }
                            else if (GlobalVars.reboots_temp_min == true)
                            {
                                if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.temp_min))
                                {

                                    GPUTempMinTimer.Enabled = true;

                                    labelStatusTempMin.Text = MyStrings.labelStatusTempMin;
                                    labelStatusTempMin.ForeColor = Color.Red;

                                    labelCounterTempMin.Visible = true;
                                    labelCounterTempMin.Text = GlobalVars.timer_t_min.ToString();
                                    labelCounterTempMin.ForeColor = Color.Red;


                                }
                                else if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.temp_min))
                                {
                                    GPUTempMinTimer.Enabled = false;
                                    labelCounterTempMin.Visible = false;
                                    labelStatusTempMin.Text = MyStrings.labelStatusTempOK;
                                    labelStatusTempMin.ForeColor = Color.Green;
                                    GlobalVars.timer_t_min = -100;

                                }



                            }

                            //temp max
                            if (GlobalVars.reboots_temp_max == false)
                            {
                               // if (labelStatusTempMax.Text != MyStrings.labelEvent)
                              //  {
                                    labelStatusTempMax.Text = MyStrings.labelEvent;
                                    labelStatusTempMax.ForeColor = Color.Blue;
                                    labelCounterTempMax.Visible = false;
                                    GlobalVars.timer_t_max = -100;
                                //     }


                            }
                            else if (GlobalVars.reboots_temp_max == true)
                            {
                                if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.temp_max))
                                {

                                    GPUTempMaxTimer.Enabled = true;

                                    labelStatusTempMax.Text = MyStrings.labelStatusTempMax;
                                    labelStatusTempMax.ForeColor = Color.Red;

                                    labelCounterTempMax.Visible = true;
                                    labelCounterTempMax.Text = GlobalVars.timer_t_max.ToString();
                                    labelCounterTempMax.ForeColor = Color.Red;


                                }

                                else if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.temp_max))
                                {
                                    GPUTempMaxTimer.Enabled = false;
                                    labelCounterTempMax.Visible = false;
                                    labelStatusTempMax.Text = MyStrings.labelStatusTempOK;
                                    labelStatusTempMax.ForeColor = Color.Green;
                                    GlobalVars.timer_t_max = -100;

                                }


                            }
                           

                            break;
                        case 2:
                           // mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/core", Encoding.UTF8.GetBytes(p));
                            break;
                        case 3:
                           // mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/memory", Encoding.UTF8.GetBytes(p));
                            break;
                        case 4:
                           // mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/load", Encoding.UTF8.GetBytes(p));
                            break;
                        case 5:

                            //fan min

                            if (GlobalVars.reboots_fan_min == false)
                            {
                                labelStatusFanMin.Visible = true;
                                labelStatusFanMin.Text = MyStrings.labelEvent;
                                labelStatusFanMin.ForeColor = Color.Blue;
                                labelCounterFanMin.Visible = false;
                                GlobalVars.timer_fan_min = -100;

                            }
                            else if (GlobalVars.reboots_fan_min == true)
                            {
                                if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.fan_min))
                                {

                                    GPUFanMinTimer.Enabled = true;


                                    labelStatusFanMin.Text = MyStrings.labelStatusFanMin;
                                    labelStatusFanMin.ForeColor = Color.Red;

                                    labelCounterFanMin.Visible = true;
                                    labelCounterFanMin.Text = GlobalVars.timer_fan_min.ToString();
                                    labelCounterFanMin.ForeColor = Color.Red;


                                }

                                else if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.fan_min))
                                {

                                    GPUFanMinTimer.Enabled = false;
                                    labelCounterFanMin.Visible = false;
                                    labelStatusFanMin.Text = MyStrings.labelStatusFanOK;
                                    labelStatusFanMin.ForeColor = Color.Green;
                                    GlobalVars.timer_t_min = -100;
                                }



                            }

                            //fan max
                            if (GlobalVars.reboots_fan_max == false)
                            {
                                labelStatusFanMax.Visible = true;
                                labelCounterFanMax.Visible = false;
                                labelStatusFanMax.Text = MyStrings.labelEvent;
                                labelStatusFanMax.ForeColor = Color.Blue;
                                GlobalVars.timer_fan_max = -100;

                            }
                            else if (GlobalVars.reboots_fan_max == true)
                            {


                                if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.fan_max))
                                {

                                    GPUFanMaxTimer.Enabled = true;

                                    labelStatusFanMax.Text = MyStrings.labelStatusFanMax;
                                    labelStatusFanMax.ForeColor = Color.Red;

                                    labelCounterFanMax.Visible = true;
                                    labelCounterFanMax.Text = GlobalVars.timer_fan_max.ToString();
                                    labelCounterFanMax.ForeColor = Color.Red;


                                }
                                else if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.fan_max))
                                {

                                    GPUFanMaxTimer.Enabled = false;
                                    labelCounterFanMax.Visible = false;
                                    labelStatusFanMax.Text = MyStrings.labelStatusFanOK;
                                    labelStatusFanMax.ForeColor = Color.Green;
                                    GlobalVars.timer_fan_max = -100;

                                }


                            }


                            break;

                    }

                    // Console.WriteLine(p.Name);
                   // labelTest.Text += " " + p;
                    j++;


                }


                labelTest.Text = labelTest.Text + " 1234\n";
                i++;

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
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/name", Encoding.UTF8.GetBytes(p));
                                break;
                            case 1:
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/temp", Encoding.UTF8.GetBytes(p));
                                break;
                            case 2:
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/core", Encoding.UTF8.GetBytes(p));
                                break;
                            case 3:
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/memory", Encoding.UTF8.GetBytes(p));
                                break;
                            case 4:
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/load", Encoding.UTF8.GetBytes(p));
                                break;
                            case 5:
                                mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/fan", Encoding.UTF8.GetBytes(p));
                               // mqttClient.Publish("devices/" + GlobalVars.token + "/common/uptime/" + i + "/fan", Encoding.UTF8.GetBytes(p));
                                break;




                        }

                        // Console.WriteLine(p.Name);
                        labelTest.Text += " " + p;
                        j++;


                    }


                     //   mqttClient.Publish("Pi/LEDControl2", Encoding.UTF8.GetBytes("SEND: " + labelTest.Text));

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

                await GlobalVars.mqttClient.PublishAsync(send_data);

                

            }
            catch (MQTTnet.Exceptions.MqttCommunicationException ex)
            {
               // MqttConnect();
             //   Debug.WriteLine("Send data: " + ex.Message + GlobalVars.json_send);

            }
            catch (Exception ex)
            {

              //  Debug.WriteLine("Send data: " + ex.Message + GlobalVars.json_send);
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
         
            labelTimeWork2.Text = ("Days: " + d.ToString() + " Hours: " + h.ToString() + " Minute: " + min.ToString());
          
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


       




        private void BtStartClick(object sender, EventArgs e)
        {
            

            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;


            if (!string.IsNullOrEmpty(tbToken.Text))
            {
                GlobalVars.token = tbToken.Text;
                tbToken.ReadOnly = true;
                MqttConnect.RunAsync();
                GetTempretureTimer.Enabled = true;
              
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
                
                }
              
            }
            GlobalVars.timer_t_max = GlobalVars.timer_t_max - 1;
        }

        //timer temp min
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
                  
                        Reboot(msg, bat);
                  
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
               
                   
                }
            }
            GlobalVars.timer_fan_min = GlobalVars.timer_fan_min - 1;
        }

        private void SendDataTimerTick(object sender, EventArgs e)
        {

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
                  
                        Reboot(msg, bat);
                  
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
                  
                        Reboot(msg, bat);
                  
                }
            }
            GlobalVars.timer_memory = GlobalVars.timer_memory - 1;
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

        private void labelStatusTempMax_Click(object sender, EventArgs e)
        {

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





        /*
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

       */



    }




}




