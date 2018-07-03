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
using System.Globalization;

namespace Informer
{
    public partial class MainForm : Form
    {

        private static List<string> hosts = new List<string>();
       // private Computer _pc;
        private static Http _http = new Http();
        private LogFile _log, _error;
        //List<String> gpusList = new List<string>();

        private Form f2;





        public MainForm()
        {


           // GlobalVars.gpuList = new Dictionary<int, List<string>>();

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

            // _pc = new Computer();
            GlobalVars._pc.CPUEnabled = true;
            GlobalVars._pc.GPUEnabled = true;
            GlobalVars._pc.Open();
            //_pc.Close();

            //GlobalVars._pc.Close();
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
               // Debug.WriteLine("My ID: " + myId);
                Process.GetProcesses()
                    .Where(p => p.ProcessName == "Informer" && p.Id != myId)
                    .Count(p => { p.Kill(); return true; });

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
                Debug.WriteLine("KILL Informer: " + e);
            }  
          
           
          
        }

        private void BtStopClick(object sender, EventArgs e)
        {
            /*
            try
            {
                if (GlobalVars.mqttClient.IsConnected)
                {
                    GlobalVars.mqttClient.DisconnectAsync();

                }
            }
            catch (Exception ex) {


            }
           
            */

            _log.writeLogLine("Informer stopped", "log");
            Message("Informer Stopped!");


            GlobalVars.firsrun = true;

            GPUStatusTimer.Enabled = false;
            GetTempretureTimer.Enabled = false;
            PingTimer.Enabled = false;
            MqttConnectTimer.Enabled = false;
            GlobalVars.mqttIsConnect = false;



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
            GPUMemMinTimer.Enabled = false;
            FellOffGPUTimer.Enabled = false;
            InformationLabel.Text = MyStrings.labelInformationStop;
            InformationLabel.ForeColor = Color.Gray;
            GlobalVars.timer_t_max = -100;
            GlobalVars.timer_t_min = -100;
            GlobalVars.timer_fan_max = -100;
            GlobalVars.timer_fan_min = -100;
            GlobalVars.timer_r_min = -100;
            GlobalVars.timer_clock_min = -100;
            GlobalVars.timer_clock_max = -100;
            GlobalVars.timer_memory_min = -100;
            GlobalVars.timer_memory_max = -100;
            GlobalVars.timer_gpu_lost = -100;
            GlobalVars.timer_load_gpu_min = -100;
            GlobalVars.timer_load_gpu_max = -100;
            GlobalVars.timer_inet = -100;
            tbToken.ReadOnly = false;

        }





        async private void GetTempretureTimerTick(object sender, EventArgs e)
        {
           // GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            if (GlobalVars.mqttIsConnect == true)
            {
                             
             //   GlobalVars.token = tbToken.Text;
             //   GlobalVars._manager.WritePrivateString("main", "token", tbToken.Text);
             
                btStart.Enabled = false;
                btStop.Visible = true;

                AutoStartTimer.Enabled = false;
               
               // GlobalVars.timeOnline = 0;
                tbToken.ReadOnly = true;
                tbRigName.Text = GlobalVars.name;
             
             //   labelTest.Text = Convert.ToString(GlobalVars.gpuList.Count);
            }

            

            try
            {
                btStart.Enabled = false;
                NextAutoStart.Enabled = false;
                AutoStartTimer.Enabled = false;
                btStop.Visible = true;
                GPUTemp.GetGPU();
               

             //   labelTest.Text = Convert.ToString(GlobalVars.gpusList.Count);


            }
            catch (Exception ex)
            {
                _error.writeLogLine("GetTempTimer: " + ex.Message, "error");
            }

            await Task.Delay(1000);
           
            
            
        }




        private void NextAutoStart_Tick(object sender, EventArgs e)
        {
            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            GlobalVars.token = tbToken.Text;
            SendDataTimer.Enabled = true;
            GPUStatusTimer.Enabled = true;
            GetTempretureTimer.Enabled = true;
            PingTimer.Enabled = true;
            AutoStartTimer.Enabled = false;
            btStop.Visible = true;
            btStart.Enabled = false;
           // GlobalVars.timeOnline = 0;
            tbRigName.ReadOnly = true;
            tbToken.ReadOnly = true;
            if (!string.IsNullOrWhiteSpace(GlobalVars.name))
            {
                Message("Informer Started!");
            }

            MqttConnectTimer.Enabled = true;
            InformationLabel.Visible = true;
            InformationLabel.Text = MyStrings.labelStatusStarted;
            InformationLabel.ForeColor = Color.Green;
            Hide();
            
            
        }

        private void AutoStart_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(GlobalVars.token))
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


        public void GpuStatus()
        {

            try
            {

                labelTempMin.Text = "TEMP MIN(" + GlobalVars.temp_min + "):";
                labelTempMax.Text = "TEMP MAX(" + GlobalVars.temp_max + "):";
                labelFanMin.Text = "FAN MIN(" + GlobalVars.fan_min + "):";
                labelFanMax.Text = "FAN MAX(" + GlobalVars.fan_max + "):";
                labelLoadMin.Text = "LOAD MIN(" + GlobalVars.load_GPU_min + "):";
                labelLoadMax.Text = "LOAD MAX(" + GlobalVars.load_GPU_max + "):";
                labelClockMin.Text = "CLOCK MIN(" + GlobalVars.clock_min + "):";
                labelClockMax.Text = "CLOCK MAX(" + GlobalVars.clock_max + "):";
                labelMemoryMin.Text = "MEMORY MIN(" + GlobalVars.mem_min + "):";
                labelMemoryMax.Text = "MEMORY MAX(" + GlobalVars.mem_max + "):";
                labelFellOffGPU.Text = "GPU LOST(" + GlobalVars.count_GPU + "):";

                //int i = 0;
                GlobalVars.tempMinCount = 0;
                int tempMaxCount = 0;
                int fanMinCount = 0;
                int fanMaxCount = 0;
                int loadMinCount = 0;
                int loadMaxCount = 0;
                int clockMinCount = 0;
                int clockMaxCount = 0;
                int memoryMinCount = 0;
                int memoryMaxCount = 0;
                labelTest.Text = "";
                
                foreach (KeyValuePair<int, List<String>> keyValue in GlobalVars.gpuList)
                {

                    string test2 = GlobalVars.gpuList[0][0];

                    int test = keyValue.Key;
                    labelTest.Text = labelTest.Text + ": " + test;

                    int j = 0;
                    foreach (String p in keyValue.Value)
                    {
                           
                        switch (j)
                        {

                            case 0:
                                //name    
                                Debug.WriteLine("keyValue.Key: ");
                                break;
                            case 1:


                                //temp min
                                if (GlobalVars.reboots_temp_min == false)
                                {

                                    labelStatusTempMin.Text = MyStrings.labelEvent;
                                    labelStatusTempMin.ForeColor = Color.Blue;
                                    labelCounterTempMin.Visible = false;
                                    GlobalVars.timer_t_min = -100;


                                }
                                else if (GlobalVars.reboots_temp_min == true)
                                {
                                    Debug.WriteLine("keyValue.Key: "+ keyValue.Value[1]);
                                    if (Convert.ToInt32(p) < Convert.ToInt32(GlobalVars.temp_min) && Convert.ToInt32(p) != 0)
                                    {
                                        GPUTempMinTimer.Enabled = true;
                                        GlobalVars.temp0 = false;
                                        labelStatusTempMin.Text = MyStrings.labelStatusTempMin;
                                        labelStatusTempMin.ForeColor = Color.Red;

                                        labelCounterTempMin.Visible = true;
                                        labelCounterTempMin.Text = GlobalVars.timer_t_min.ToString();
                                        labelCounterTempMin.ForeColor = Color.Red;
                                        GlobalVars.tempMinCount--;
                                    }
                                    else if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.temp_min) && Convert.ToInt32(p) != 0)
                                    {

                                        GlobalVars.tempMinCount++;
                                    }
                                    if (Convert.ToInt32(p) == 0)
                                    {
                                        GlobalVars.temp0 = true;
                                        //  Debug.WriteLine("Temp0: " + Convert.ToInt32(p));
                                        OHMTimer.Enabled = true;
                                    }
                                    // Debug.WriteLine("Temp0ON: " + Convert.ToInt32(p));
                                    if (GlobalVars.tempMinCount == GlobalVars.count_GPU && GlobalVars.tempMinCount != 0)
                                    {
                                        
                                        OHMTimer.Enabled = false;
                                        GlobalVars.temp0 = false;

                                        GPUTempMinTimer.Enabled = false;
                                        labelCounterTempMin.Visible = false;
                                        labelStatusTempMin.Visible = Enabled;
                                        labelStatusTempMin.Text = MyStrings.labelStatusTempOK;
                                        labelStatusTempMin.ForeColor = Color.Green;
                                        GlobalVars.timer_t_min = -100;
                                    }

                                    //labelTest.Text = Convert.ToString(GlobalVars.tempMinCount) +" "+ Convert.ToString(GlobalVars.gpuList.Count);
                                    //labelTest.Text = labelTest.Text + " " + Convert.ToInt32(p) + keyValue.Value[1];




                                }

                                //temp max
                                if (GlobalVars.reboots_temp_max == false)
                                {

                                    labelStatusTempMax.Text = MyStrings.labelEvent;
                                    labelStatusTempMax.ForeColor = Color.Blue;
                                    labelCounterTempMax.Visible = false;
                                    GlobalVars.timer_t_max = -100;

                                }
                                else if (GlobalVars.reboots_temp_max == true)
                                {
                                    if (Convert.ToInt32(p) > Convert.ToInt32(GlobalVars.temp_max))
                                    {

                                        GPUTempMaxTimer.Enabled = true;

                                        labelStatusTempMax.Text = MyStrings.labelStatusTempMax;
                                        labelStatusTempMax.ForeColor = Color.Red;

                                        labelCounterTempMax.Visible = true;
                                        labelCounterTempMax.Text = GlobalVars.timer_t_max.ToString();
                                        labelCounterTempMax.ForeColor = Color.Red;
                                        tempMaxCount--;


                                    }

                                    else if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.temp_max))
                                    {
                                        
                                        tempMaxCount++;

                                    }

                                    if (tempMaxCount == GlobalVars.count_GPU)
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
                                //core
                                try
                                {
                                    IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "," };
                                    double bc = double.Parse(p, formatter);

                                    //Debug.WriteLine("PPPP " + Math.Floor(b));
                                    int pc = Convert.ToInt32(Math.Floor(bc));

                                    //Debug.WriteLine("PPPP " + pp);

                                    if (GlobalVars.reboots_clock_min == true)
                                    {

                                        if (Convert.ToInt32(pc) < GlobalVars.clock_min)
                                        {
                                            GPUCoreMinTimer.Enabled = true;


                                            labelStatusClockMin.Text = MyStrings.labelStatusClock;
                                            labelStatusClockMin.ForeColor = Color.Red;

                                            labelCounterClockMin.Visible = true;
                                            labelCounterClockMin.Text = GlobalVars.timer_clock_min.ToString();
                                            labelCounterClockMin.ForeColor = Color.Red;
                                            clockMinCount--;

                                        }
                                        else if (Convert.ToInt32(pc) >= GlobalVars.clock_min)
                                        {
                                            clockMinCount++;
                                            
                                        }

                                        if (clockMinCount == GlobalVars.count_GPU)
                                        {
                                            GPUCoreMinTimer.Enabled = false;
                                            labelCounterClockMin.Visible = false;
                                            labelStatusClockMin.Text = MyStrings.labelStatusOK;
                                            labelStatusClockMin.ForeColor = Color.Green;
                                            GlobalVars.timer_clock_min = -100;
                                        }



                                    }
                                    else if (GlobalVars.reboots_clock_min == false)
                                    {
                                        labelStatusClockMin.Visible = true;
                                        labelStatusClockMin.Text = MyStrings.labelEvent;
                                        labelStatusClockMin.ForeColor = Color.Blue;
                                        labelCounterClockMin.Visible = false;
                                        GlobalVars.timer_clock_min = -100;
                                        GPUCoreMinTimer.Enabled = false;



                                    }



                                    if (GlobalVars.reboots_clock_max == true)
                                    {
                                        if (Convert.ToInt32(pc) > Convert.ToInt32(GlobalVars.clock_max))
                                        {
                                            GPUCoreMaxTimer.Enabled = true;


                                            labelStatusClockMax.Text = MyStrings.labelStatusClockMax;
                                            labelStatusClockMax.ForeColor = Color.Red;

                                            labelCounterClockMax.Visible = true;
                                            labelCounterClockMax.Text = GlobalVars.timer_clock_max.ToString();
                                            labelCounterClockMax.ForeColor = Color.Red;
                                            clockMaxCount--;

                                        }
                                        else if (Convert.ToInt32(pc) <= Convert.ToInt32(GlobalVars.clock_max))
                                        {
                                            clockMaxCount++;

                                        }

                                        if (clockMaxCount == GlobalVars.count_GPU)
                                        {
                                            GPUCoreMaxTimer.Enabled = false;
                                            labelCounterClockMax.Visible = false;
                                            labelStatusClockMax.Text = MyStrings.labelStatusOK;
                                            labelStatusClockMax.ForeColor = Color.Green;
                                            GlobalVars.timer_clock_max = -100;
                                        }

                                    }
                                    else if (GlobalVars.reboots_clock_max == false)
                                    {

                                        labelStatusClockMax.Visible = true;
                                        labelStatusClockMax.Text = MyStrings.labelEvent;
                                        labelStatusClockMax.ForeColor = Color.Blue;
                                        labelCounterClockMax.Visible = false;
                                        GlobalVars.timer_clock_max = -100;
                                        GPUCoreMaxTimer.Enabled = false;

                                    }
                                }
                                catch (Exception x)
                                {

                                    Debug.WriteLine("Core:" + x);
                                }


                                break;
                            case 3:
                                // mqttClient.Publish("devices/" + GlobalVars.token + "/gpus/" + i + "/memory", Encoding.UTF8.GetBytes(p));
                                // memory
                                try
                                {
                                    IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "," };
                                    double bm = double.Parse(p, formatter);

                                    //Debug.WriteLine("PPPP " + Math.Floor(b));
                                    int pm = Convert.ToInt32(Math.Floor(bm));

                                    //Debug.WriteLine("PPPP " + pp);


                                    if (GlobalVars.reboots_mem_min == true)
                                    {
                                        if (Convert.ToInt32(pm) < Convert.ToInt32(GlobalVars.mem_min))
                                        {
                                            GPUMemMinTimer.Enabled = true;


                                            labelStatusMemoryMin.Text = MyStrings.labelStatusMemoryMin;
                                            labelStatusMemoryMin.ForeColor = Color.Red;

                                            labelCounterMemoryMin.Visible = true;
                                            labelCounterMemoryMin.Text = GlobalVars.timer_memory_min.ToString();
                                            labelCounterMemoryMin.ForeColor = Color.Red;
                                            memoryMinCount--;

                                        }
                                        else if (Convert.ToInt32(pm) >= Convert.ToInt32(GlobalVars.mem_min))
                                        {
                                            memoryMinCount++;
                                           


                                        }
                                        if (memoryMinCount == GlobalVars.count_GPU)
                                        {
                                            GPUMemMinTimer.Enabled = false;
                                            labelCounterMemoryMin.Visible = false;
                                            labelStatusMemoryMin.Text = MyStrings.labelStatusOK;
                                            labelStatusMemoryMin.ForeColor = Color.Green;
                                            GlobalVars.timer_memory_min = -100;

                                        }


                                    }
                                    else if (GlobalVars.reboots_mem_min == false)
                                    {

                                        labelStatusMemoryMin.Visible = true;
                                        labelStatusMemoryMin.Text = MyStrings.labelEvent;
                                        labelStatusMemoryMin.ForeColor = Color.Blue;
                                        labelCounterMemoryMin.Visible = false;
                                        GlobalVars.timer_memory_min = -100;
                                        GPUMemMinTimer.Enabled = false;

                                    }

                                    if (GlobalVars.reboots_mem_max == true)
                                    {
                                        if (Convert.ToInt32(pm) > Convert.ToInt32(GlobalVars.mem_max))
                                        {
                                            GPUMemMaxTimer.Enabled = true;


                                            labelStatusMemoryMax.Text = MyStrings.labelStatusMemoryMax;
                                            labelStatusMemoryMax.ForeColor = Color.Red;

                                            labelCounterMemoryMax.Visible = true;
                                            labelCounterMemoryMax.Text = GlobalVars.timer_memory_max.ToString();
                                            labelCounterMemoryMax.ForeColor = Color.Red;
                                            memoryMaxCount--;

                                        }
                                        else if (Convert.ToInt32(pm) <= Convert.ToInt32(GlobalVars.mem_max))
                                        {
                                            memoryMaxCount++;
                                        }

                                        if (memoryMaxCount == GlobalVars.count_GPU)
                                        {
                                            GPUMemMaxTimer.Enabled = false;
                                            labelCounterMemoryMax.Visible = false;
                                            labelStatusMemoryMax.Text = MyStrings.labelStatusOK;
                                            labelStatusMemoryMax.ForeColor = Color.Green;
                                            GlobalVars.timer_memory_max = -100;

                                        }

                                    }
                                    else if (GlobalVars.reboots_mem_max == false)
                                    {

                                        labelStatusMemoryMax.Visible = true;
                                        labelStatusMemoryMax.Text = MyStrings.labelEvent;
                                        labelStatusMemoryMax.ForeColor = Color.Blue;
                                        labelCounterMemoryMax.Visible = false;
                                        GlobalVars.timer_memory_max = -100;
                                        GPUMemMaxTimer.Enabled = false;
                                    }


                                   
                                }
                                catch (Exception ex)
                                {

                                    Debug.WriteLine("Memory:" + ex);
                                }
                                break;
                            case 4:

                                //gpu load min

                                if (GlobalVars.reboots_load_min == true)
                                {
                                    if (Convert.ToInt32(p) < Convert.ToInt32(GlobalVars.load_GPU_min))
                                    {
                                        GPULoadMinTimer.Enabled = true;


                                        labelStatusLoadMin.Text = MyStrings.labelStatusLoadGPU;
                                        labelStatusLoadMin.ForeColor = Color.Red;

                                        labelCounterLoadMin.Visible = true;
                                        labelCounterLoadMin.Text = GlobalVars.timer_load_gpu_min.ToString();
                                        labelCounterLoadMin.ForeColor = Color.Red;
                                        loadMinCount--;

                                    }
                                    else if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.load_GPU_min))
                                    {

                                        loadMinCount++;
                                        

                                    }

                                    if (loadMinCount == GlobalVars.count_GPU)
                                    {
                                        GPULoadMinTimer.Enabled = false;
                                        labelCounterLoadMin.Visible = false;
                                        labelStatusLoadMin.Text = MyStrings.labelStatusOK;
                                        labelStatusLoadMin.ForeColor = Color.Green;
                                        GlobalVars.timer_load_gpu_min = -100;

                                    }

                                }
                                else if (GlobalVars.reboots_load_min == false)
                                {
                                    labelStatusLoadMin.Visible = true;
                                    labelStatusLoadMin.Text = MyStrings.labelEvent;
                                    labelStatusLoadMin.ForeColor = Color.Blue;
                                    labelCounterLoadMin.Visible = false;
                                    GlobalVars.timer_load_gpu_min = -100;
                                    GPULoadMinTimer.Enabled = false;

                                }

                                if (GlobalVars.reboots_load_max == true)
                                {
                                    if (Convert.ToInt32(p) > Convert.ToInt32(GlobalVars.load_GPU_max))
                                    {
                                        GPULoadMaxTimer.Enabled = true;


                                        labelStatusLoadMax.Text = MyStrings.labelStatusLoadGPUMax;
                                        labelStatusLoadMax.ForeColor = Color.Red;

                                        labelCounterLoadMax.Visible = true;
                                        labelCounterLoadMax.Text = GlobalVars.timer_load_gpu_max.ToString();
                                        labelCounterLoadMax.ForeColor = Color.Red;
                                        loadMaxCount--;
                                    }
                                    else if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.load_GPU_max))
                                    {
                                        loadMaxCount++;
                                        


                                    }

                                    if (loadMaxCount == GlobalVars.count_GPU)
                                    {
                                        GPULoadMaxTimer.Enabled = false;
                                        labelCounterLoadMax.Visible = false;
                                        labelStatusLoadMax.Text = MyStrings.labelStatusOK;
                                        labelStatusLoadMax.ForeColor = Color.Green;
                                        GlobalVars.timer_load_gpu_max = -100;

                                    }

                                }

                                else if (GlobalVars.reboots_load_max == false)
                                {
                                    labelStatusLoadMax.Visible = true;
                                    labelStatusLoadMax.Text = MyStrings.labelEvent;
                                    labelStatusLoadMax.ForeColor = Color.Blue;
                                    labelCounterLoadMax.Visible = false;
                                    GlobalVars.timer_load_gpu_max = -100;
                                    GPULoadMaxTimer.Enabled = false;


                                }

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
                                    if (Convert.ToInt32(p) < Convert.ToInt32(GlobalVars.fan_min))
                                    {

                                        GPUFanMinTimer.Enabled = true;


                                        labelStatusFanMin.Text = MyStrings.labelStatusFanMin;
                                        labelStatusFanMin.ForeColor = Color.Red;

                                        labelCounterFanMin.Visible = true;
                                        labelCounterFanMin.Text = GlobalVars.timer_fan_min.ToString();
                                        labelCounterFanMin.ForeColor = Color.Red;
                                        fanMinCount--;

                                    }

                                    else if (Convert.ToInt32(p) >= Convert.ToInt32(GlobalVars.fan_min))
                                    {
                                        fanMinCount++;
                                       
                                    }
                                    if (fanMinCount == GlobalVars.count_GPU)
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


                                    if (Convert.ToInt32(p) > Convert.ToInt32(GlobalVars.fan_max))
                                    {

                                        GPUFanMaxTimer.Enabled = true;

                                        labelStatusFanMax.Text = MyStrings.labelStatusFanMax;
                                        labelStatusFanMax.ForeColor = Color.Red;

                                        labelCounterFanMax.Visible = true;
                                        labelCounterFanMax.Text = GlobalVars.timer_fan_max.ToString();
                                        labelCounterFanMax.ForeColor = Color.Red;
                                        fanMaxCount--;


                                    }
                                    else if (Convert.ToInt32(p) <= Convert.ToInt32(GlobalVars.fan_max))
                                    {
                                        fanMaxCount++;
                                       

                                    }

                                    if (fanMaxCount == GlobalVars.count_GPU)
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

                        
                        j++;


                    }


                }
                //no inet
                if (GlobalVars.reboots_lost_inet == false)
                {
                    labelStatusInternet.Visible = true;
                    labelCounterInternet.Visible = false;
                    labelStatusInternet.Text = MyStrings.labelEvent;
                    labelStatusInternet.ForeColor = Color.Blue;
                    GlobalVars.timer_fan_max = -100;

                }
                else if (GlobalVars.reboots_lost_inet == true)
                {

                    if (GlobalVars.mqttIsConnect == false && GlobalVars.ping == false && GlobalVars.firsrun == false)
                    {

                        DontHaveInternetTimer.Enabled = true;

                        labelStatusInternet.Text = MyStrings.labelStatusInternet;
                        labelStatusInternet.ForeColor = Color.Red;

                        labelCounterInternet.Visible = true;
                        labelCounterInternet.Text = GlobalVars.timer_inet.ToString();
                        labelCounterInternet.ForeColor = Color.Red;
                    }


                    else if (GlobalVars.mqttIsConnect == true || GlobalVars.ping == true)
                    {

                        DontHaveInternetTimer.Enabled = false;
                        labelCounterInternet.Visible = false;
                        labelStatusInternet.Text = MyStrings.labelStatusOK;
                        labelStatusInternet.ForeColor = Color.Green;
                        GlobalVars.timer_inet = -100;

                    }


                }
                //autorization
                if (GlobalVars.mqttIsConnect == false && GlobalVars.firsrun == false)
                {
                    InformationLabel.Text = MyStrings.labelInformationAuthorizationFailed;
                    InformationLabel.ForeColor = Color.Red;
                }
                else if (GlobalVars.mqttIsConnect == true && GlobalVars.firsrun == false)
                {
                    InformationLabel.Text = MyStrings.labelInformationAuthorizationOK;
                    InformationLabel.ForeColor = Color.Green;
                }


                // gpu lost
                if (GlobalVars.reboots_lost_gpu == false)
                {
                    labelStatusGPULost.Visible = true;
                    labelCounterGPULost.Visible = false;
                    labelStatusGPULost.Text = MyStrings.labelEvent;
                    labelStatusGPULost.ForeColor = Color.Blue;
                    GlobalVars.timer_gpu_lost = -100;

                }
                else if (GlobalVars.reboots_lost_gpu == true && GlobalVars.count_GPU > 0)
                {

                    if (GlobalVars.count_GPU > GlobalVars.counts || GlobalVars.temp0 == true)
                    {
                        if (FellOffGPUTimer.Enabled == false)
                        {
                            GlobalVars.timer_gpu_lost = -100;
                        }

                        FellOffGPUTimer.Enabled = true;
                        labelCounterGPULost.Visible = true;
                        labelCounterGPULost.ForeColor = Color.Red;
                        labelCounterGPULost.Text = GlobalVars.timer_gpu_lost.ToString();
                        labelStatusGPULost.Text = MyStrings.labelStatusFellOffGPU;
                        labelStatusGPULost.ForeColor = Color.Red;



                    }
                    else if (GlobalVars.count_GPU == GlobalVars.counts && GlobalVars.temp0 == false)
                    {


                        FellOffGPUTimer.Enabled = false;
                        labelCounterGPULost.Visible = false;

                        labelStatusGPULost.Text = MyStrings.labelStatusOK; ;
                        labelStatusGPULost.ForeColor = Color.Green;
                        GlobalVars.timer_gpu_lost = -100;

                    }


                }
            }
            catch (Exception e) {

                Debug.WriteLine("GpuStatus: " + e);
            }

        }


        //public void SendData()
        public static async Task SendData()
        {
            
            if (GlobalVars.mqttIsConnect == true)
            {
                try
                {




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

                    

                    GlobalVars.upTime = UpTime.ToString(@"dd\.hh\:mm\:ss");
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

                    await GlobalVars.client.PublishAsync(send_data);
                    Debug.WriteLine("SendData" + send_data.Payload.Length);



                }
                catch (MQTTnet.Exceptions.MqttCommunicationException ex)
                {
                    // MqttConnect();
                    Debug.WriteLine("Send data MqttCommunicationException: " + ex.Message);

                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Send data Ex: " + ex.Message);

                }


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

        async private void UptimeTimerTick(object sender, EventArgs e)
        {
            GlobalVars.timeOnline = GlobalVars.timeOnline + 1;
            int min = GlobalVars.timeOnline;
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
                    GlobalVars.host +
                    "/api.php?token=" + GlobalVars.token +
                    "&event=" + "reboot" +
                    "&reason="  + GlobalVars.name + " " + msg
                   
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
       // public static TimeSpan
        public static void Message(string msg)
        {
            
            
            try
            {
                _http.GetContent(
                    GlobalVars.host +
                    "/api.php?token=" + GlobalVars.token +
                    "&event=" + "message" +
                    "&reason=" + GlobalVars.name + " " + msg
                    );

              //  _log.writeLogLine("Message " + msg, "log");
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


            if (!string.IsNullOrWhiteSpace(tbToken.Text))
            {

                MqttConnectTimer.Enabled = true;
                GetTempretureTimer.Enabled = true;
                GPUStatusTimer.Enabled = true;
                SendDataTimer.Enabled = true;
                GlobalVars.token = tbToken.Text;
                tbToken.ReadOnly = true;
                //   Action<string> asyn = new Action<string>(Pinger);
                
                
                PingTimer.Enabled = true;
                
                
                if (!string.IsNullOrWhiteSpace(GlobalVars.name))
                {
                    Message("Informer Started!");
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

        async private void TempretureTimerTick(object sender, EventArgs e)
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
            await Task.Delay(1);
        }

        //timer temp min
       async private void LowTempretureTimerTick(object sender, EventArgs e)
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
            await Task.Delay(1);
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

        async private void FanMaxTimerTick(object sender, EventArgs e)
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
            await Task.Delay(1);
        }

        async private void FanMinTimerTick(object sender, EventArgs e)
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
            await Task.Delay(1);
        }

        private async void SendDataTimerTick(object sender, EventArgs e)
        {
            if (GlobalVars.mqttIsConnect == true)
            {
                if (GlobalVars.interval > 0)
                {
                    SendDataTimer.Interval = GlobalVars.interval * 1000;
                }
             //   Debug.WriteLine("Interval: " + SendDataTimer.Interval);
                await SendData();
            }
            
        }

        

        async private void GpuCoreMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Min , Reboot!";
            const string bat = "reboot_clock.bat";
            if (GlobalVars.timer_clock_min < 0)
            {
                GlobalVars.timer_clock_min = GlobalVars.time_clock_min;
            }
            if (GlobalVars.timer_clock_min == 0)
            {
                if (!GlobalVars.reboot5)
                {
                  
                        Reboot(msg, bat);
                  
                }
            }
            GlobalVars.timer_clock_min = GlobalVars.timer_clock_min - 1;
            await Task.Delay(1);
        }

       async private void GpuCoreMaxHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Max , Reboot!";
            const string bat = "reboot_clock_max.bat";
            if (GlobalVars.timer_clock_max < 0)
            {
                GlobalVars.timer_clock_max = GlobalVars.time_clock_max;
            }
            if (GlobalVars.timer_clock_max == 0)
            {
                if (!GlobalVars.coreMax)
                {

                    Reboot(msg, bat);

                }
            }
            GlobalVars.timer_clock_max = GlobalVars.timer_clock_max - 1;
            await Task.Delay(1);
        }


       async private void GpuMemoryMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Memory Min, Reboot!";
            const string bat = "reboot_memory.bat";
            if (GlobalVars.timer_memory_min < 0)
            {
                GlobalVars.timer_memory_min = GlobalVars.time_mem_min;
            }
            if (GlobalVars.timer_memory_min == 0)
            {
                if (!GlobalVars.reboot6)
                {
                  
                        Reboot(msg, bat);
                  
                }
            }
            GlobalVars.timer_memory_min = GlobalVars.timer_memory_min - 1;
            await Task.Delay(1);
        }


        async private void GPUMemoryMaxHzTimer_Tick(object sender, EventArgs e)
        {
            const string msg = "Memory Max, Reboot!";
            const string bat = "reboot_memory_max.bat";
            try
            {
                if (GlobalVars.timer_memory_max < 0)
                {
                    GlobalVars.timer_memory_max = GlobalVars.time_mem_max;
                }
                if (GlobalVars.timer_memory_max == 0)
                {
                    if (!GlobalVars.memMax)
                    {

                        Reboot(msg, bat);

                    }
                }
                GlobalVars.timer_memory_max = GlobalVars.timer_memory_max - 1;
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
                /*
                if (GlobalVars.temp0 == true)
                {
                    GlobalVars._pc.Close();
                    GlobalVars._pc = null;
                    GlobalVars._pc = new Computer();
                    GlobalVars._pc.CPUEnabled = true;
                    GlobalVars._pc.GPUEnabled = true;
                    GlobalVars._pc.Open();

                }
                */
                
                

                GlobalVars.timer_inet = GlobalVars.timer_inet - 1;
                
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
                if (GlobalVars.timer_gpu_lost < 0)
                {
                    GlobalVars.timer_gpu_lost = GlobalVars.time_lost_gpu;
                }
                if (GlobalVars.timer_gpu_lost == 0)
                {
                    if (!GlobalVars.IsRebootStarted)
                    {
                        
                        Reboot(msg, bat);
                        
                    }
                }
                GlobalVars.timer_gpu_lost = GlobalVars.timer_gpu_lost - 1;
               
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

          
            if (GlobalVars.timer_load_gpu_min < 0)
                {
                    GlobalVars.timer_load_gpu_min = GlobalVars.time_load_GPU_min;
                }
                if (GlobalVars.timer_load_gpu_min == 0)
                {
                    if (!GlobalVars.rebootLoadGPU)
                    {
                       Reboot(msg, bat);
                    }
                }
                GlobalVars.timer_load_gpu_min = GlobalVars.timer_load_gpu_min - 1;
           
           
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
                if (GlobalVars.timer_load_gpu_max < 0)
                {
                    GlobalVars.timer_load_gpu_max = GlobalVars.time_load_GPU_min;
                }
                if (GlobalVars.timer_load_gpu_max == 0)
                {
                    if (!GlobalVars.rebootLoadGPU)
                    {
                        Reboot(msg, bat);
                    }
                }
                GlobalVars.timer_load_gpu_max = GlobalVars.timer_load_gpu_max - 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GPU Load Max " + ex);
            }
            await Task.Delay(1);
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {

            GlobalVars.pingCount = 0;
            // В переменную hosts записываем все рабочие станции из файла
            hosts = getComputersListFromTxtFile("pinglist.txt");
            // Создаём Action типизированный string, данный Action будет запускать функцию Pinger
            Action<string> asyn = new Action<string>(Pinger);
            // Для каждой рабочей станции запускаем Pinger'а
            hosts.ForEach(p =>
            {
                asyn.Invoke(p);
            });


            if (GlobalVars.pingCount >= 3)
            {

                GlobalVars.ping = false;

            }
            else if (GlobalVars.pingCount < 3)
            {

                GlobalVars.ping = true;
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
            catch (System.IO.FileNotFoundException e)
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
        async private static void Pinger(string hostAdress)
        {
            // Создаём экземпляр класса Ping
            Ping png = new Ping();
            try
            {
                // Пингуем рабочую станцию hostAdress
                PingReply pr = await png.SendPingAsync(hostAdress);
                // List<bool> problemPing = new List<bool>();

                // то такую машину заносим в список
                if (pr.Status != IPStatus.Success)
                {
                    //  GlobalVars.ping = true;
                    GlobalVars.pingCount = GlobalVars.pingCount + 1;
                   // Debug.WriteLine("PING WRONG! " + hostAdress + " " + GlobalVars.pingCount);
                }

                else if (pr.Status == IPStatus.Success)
                {
                    //  GlobalVars.ping = true;
                    GlobalVars.pingCount = GlobalVars.pingCount - 1;
                   // Debug.WriteLine("PING OK! " + hostAdress + " " + GlobalVars.pingCount);
                }
                
                // Записываем в файл все проблемные машины
                //  writeProblemComputersToFile("D:\\problemsWithAdminShare.txt", problemComputersList);
            }
            catch (Exception e)
            {
                GlobalVars.pingCount = GlobalVars.pingCount + 1;
                Debug.WriteLine("Возникла ошибка! " + hostAdress + " " + GlobalVars.pingCount + " Ex " +e.Message);
                GlobalVars.ping = false;
            }
        }

        async void MqttConnectTimer_Tick(object sender, EventArgs e)
        {

            await MqttConnect.RunAsync();

        }

        async private void OHMTimer_Tick(object sender, EventArgs e)
        {

            GlobalVars._pc.Close();
            GlobalVars._pc = null;
            GlobalVars._pc = new Computer();
            GlobalVars._pc.CPUEnabled = true;
            GlobalVars._pc.GPUEnabled = true;
            GlobalVars._pc.Open();

            await Task.Delay(500);


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




