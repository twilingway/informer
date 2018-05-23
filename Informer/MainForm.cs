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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Security;


namespace Informer
{
    public partial class MainForm : Form
    {


        private Computer _pc;
        private static Http _http = new Http();
        private LogFile _log,_error;
        private INIManager _manager;
        List<String> gpusList = new List<string>();

        private Form f2;
        static MqttClient client = new MqttClient("allminer.ru", int.Parse("1883"), false, MqttSslProtocols.None, null, null);


      
        public MainForm()
        {

            GlobalVars.gpuList = new Dictionary<int, List<string>>();
            
          

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            }
            

            InitializeComponent();
            string fullPath = Application.StartupPath.ToString();
            _manager = new INIManager(fullPath + "\\my.ini");
            _manager.WritePrivateString("main", "version", "1.3.9");
            Process psiwer;
            psiwer = Process.Start("cmd", @"/c taskkill /f /im launcher_informer.exe");
            psiwer.Close();
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
                  
        }

        

        public TimeSpan UpTime
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


        private void BtStopClick(object sender, EventArgs e)
        {

            bool code = client.IsConnected;
            //            client.Publish("Pi/LEDControl2", Encoding.UTF8.GetBytes("DontConnected" + 0));



            //  MessageBox.Show(this, Convert.ToString(code), "Message", MessageBoxButtons.OK, MessageBoxIcon.Question);
            try
            {
                if (code == true)
                {
                    //  System.Threading.Thread.Sleep(200);
                    //  client.Publish("Pi/LEDControl2", Encoding.UTF8.GetBytes("Connected" + 0));
                    System.Threading.Thread.Sleep(200);
                    client.Disconnect();
                    //  base.OnClosed(e);
                }
                else if (code == false)
                {
                    //   MessageBox.Show(this, "Connect Fail", "Message", MessageBoxButtons.OK, MessageBoxIcon.Question);

                }
            }
            catch (Exception ex)
            {
                _error.writeLogLine(ex.Message, "error");
            }

         
            
           
            
           // App.Current.Shutdown();
           

            GetTempretureTimer.Enabled = false;
            AutoStartTimer.Enabled = false;
            AutoStartTimer.Stop();
            PingInternetTimer.Stop();
            timer2.Stop();
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
            _log.writeLogLine("Informer stopped","log");
            if (string.IsNullOrEmpty(GlobalVars.token))
            {
              //  tbEmail.ReadOnly = false;
              //  tbSecret.ReadOnly = false;
              //  tbRigName.ReadOnly = false;
            }
            tbToken.ReadOnly = false;

            

            Message("Informer Stopped!");

        }
        public void InitFromIni()
        {
           
            string email          = _manager.GetPrivateString("main", "email");
            string secret         = _manager.GetPrivateString("main", "secret");
            
            string name = _manager.GetPrivateString("main", "name");
            string worker;
            if (string.IsNullOrEmpty(name))
            {
                worker = _manager.GetPrivateString("main", "worker");
                name = worker;
            }
            string token          = _manager.GetPrivateString("main", "token");
            string version        = _manager.GetPrivateString("main", "version");
           

            string reboot_temp_max     = _manager.GetPrivateString("main", "reboot_temp_max");
            string temp_max = _manager.GetPrivateString("main", "temp_max");
            string time_temp_max = _manager.GetPrivateString("main", "time_temp_max");

            string reboot_temp_min = _manager.GetPrivateString("main", "reboot_temp_min");
            string temp_min = _manager.GetPrivateString("main", "temp_min");
            string time_temp_min = _manager.GetPrivateString("main", "time_temp_min");

            string reboot_max_fan = _manager.GetPrivateString("main", "reboot_max_fan");
            string fan_max = _manager.GetPrivateString("main", "fan_max");
            string time_fan_max = _manager.GetPrivateString("main", "time_fan_max");

            string reboot_min_fan = _manager.GetPrivateString("main", "reboot_min_fan");
            string fan_min = _manager.GetPrivateString("main", "fan_min");
            string time_fan_min = _manager.GetPrivateString("main", "time_fan_min");

            string reload_file = _manager.GetPrivateString("main", "reload_file");
            string reload_temp_min_file = _manager.GetPrivateString("main", "reload_temp_min_file");
            string reload_time_min_file = _manager.GetPrivateString("main", "reload_time_min_file");
            string dir = _manager.GetPrivateString("main", "dir");
            string filename = _manager.GetPrivateString("main", "filename");
            string path = _manager.GetPrivateString("main", "path");
            string dir2 = _manager.GetPrivateString("main", "dir2");
            string filename2 = _manager.GetPrivateString("main", "filename2");
            string path2 = _manager.GetPrivateString("main", "path2");

            string reboot_clock   = _manager.GetPrivateString("main", "reboot_clock");
            string clock = _manager.GetPrivateString("main", "clock");
            string time_clock = _manager.GetPrivateString("main", "time_clock");

            string reboot_memory  = _manager.GetPrivateString("main", "reboot_memory");
            string memory = _manager.GetPrivateString("main", "memory");
            string time_memory = _manager.GetPrivateString("main", "time_memory");

            string reboot_GPU = _manager.GetPrivateString("main", "reboot_GPU");
            string count_GPU = _manager.GetPrivateString("main", "count_GPU");
            string time_count_GPU = _manager.GetPrivateString("main", "time_count_GPU");

            string reboot_load_GPU = _manager.GetPrivateString("main", "reboot_load_GPU");
            string load_GPU = _manager.GetPrivateString("main", "load_GPU");
            string time_load_GPU = _manager.GetPrivateString("main", "time_load_GPU");
            
            string reboot_internet = _manager.GetPrivateString("main", "reboot_internet");
            string time_internet = _manager.GetPrivateString("main", "time_internet");

            string time_start        = _manager.GetPrivateString("main", "time_start");

            string stat = _manager.GetPrivateString("main", "stat");
            string pool = _manager.GetPrivateString("main", "pool");
            string wallet = _manager.GetPrivateString("main", "wallet");

            //****Перезагрузка температура максимум - НАЧАЛО
            //проверка секбокса
            if (string.IsNullOrEmpty(reboot_temp_max))
            {
                reboot_temp_max = "0";
                _manager.WritePrivateString("main", "reboot_temp_max", reboot_temp_max);
            }
            GlobalVars.reboot_temp_max = reboot_temp_max;

            //температура максимум
            if (string.IsNullOrEmpty(temp_max))
            {
                temp_max = "91";
                _manager.WritePrivateString("main", "temp_max", temp_max);
            }
            GlobalVars.temp_max = Convert.ToInt32(temp_max);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_temp_max))
            {
                time_temp_max = "306";
                _manager.WritePrivateString("main", "time_temp_max", time_temp_max);
            }
            GlobalVars.time_temp_max = Convert.ToInt32(time_temp_max);
            //****Перезагрузка температура максимум - КОНЕЦ

 //----------------------------****************----------------------------

            //****Перезагрузка температура минимум - НАЧАЛО
            if (string.IsNullOrEmpty(reboot_temp_min))
            //проверка чекбокса
            {
                reboot_temp_min = "0";
                _manager.WritePrivateString("main", "reboot_temp_min", reboot_temp_min);
            }
            GlobalVars.reboot_temp_min = reboot_temp_min;
            //температура минимум
            if (string.IsNullOrEmpty(temp_min))
            {
                temp_min = "41";
                _manager.WritePrivateString("main", "temp_min", temp_min);
            }
            GlobalVars.temp_min = Convert.ToInt32(temp_min);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_temp_min))
            {
                time_temp_min = "305";
                _manager.WritePrivateString("main", "time_temp_min", time_temp_min);
            }
            GlobalVars.time_temp_min = Convert.ToInt32(time_temp_min);
            //****Перезагрузка температура минимум - КОНЕЦ

//----------------------------****************----------------------------

            //****Максимальные обороты НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_max_fan))
            {
                reboot_max_fan = "0";
                _manager.WritePrivateString("main", "reboot_max_fan", reboot_max_fan);
            }
            GlobalVars.reboot_max_fan = reboot_max_fan;
            //обороты в %
            if (string.IsNullOrEmpty(fan_max))
            {
                fan_max = "100";
                _manager.WritePrivateString("main", "fan_max", fan_max);
            }
            GlobalVars.fan_max = Convert.ToInt32(fan_max);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_fan_max))
            {
                time_fan_max = "298";
                _manager.WritePrivateString("main", "time_fan_max", time_fan_max);
            }
            GlobalVars.time_fan_max = Convert.ToInt32(time_fan_max);

            //****Максимальные обороты - КОНЕЦ 

//----------------------------****************----------------------------

            //****Минимальные Обороты - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_min_fan))
            {
                reboot_min_fan = "0";
                _manager.WritePrivateString("main", "reboot_min_fan", reboot_min_fan);
            }
            GlobalVars.reboot_min_fan = reboot_min_fan;

            //обороты в %
            if (string.IsNullOrEmpty(fan_min))
            {
                fan_min = "10";
                _manager.WritePrivateString("main", "fan_min", fan_min);
            }
            GlobalVars.fan_min = Convert.ToInt32(fan_min);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_fan_min))
            {
                time_fan_min = "297";
                _manager.WritePrivateString("main", "time_fan_min", time_fan_min);
            }
            GlobalVars.time_fan_min = Convert.ToInt32(time_fan_min);
            //****Минимальные Обороты - КОНЕЦ

//----------------------------****************----------------------------

            //**Перезагрузка файла -- НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reload_file))
            {
                reload_file = "0";
                _manager.WritePrivateString("main", "reload_file", reload_file);
            }
            GlobalVars.reload_file = reload_file;
            //температура минимум
            if (string.IsNullOrEmpty(reload_temp_min_file))
            {
                reload_temp_min_file = "40";
                _manager.WritePrivateString("main", "reload_temp_min_file", reload_temp_min_file);
            }
            GlobalVars.reload_temp_min_file = Convert.ToInt32(reload_temp_min_file);
            //время перезагрузки
            if (string.IsNullOrEmpty(reload_time_min_file))
            {
                reload_time_min_file = "303";
                _manager.WritePrivateString("main", "reload_time_min_file", reload_time_min_file);
            }
            GlobalVars.reload_time_min_file = Convert.ToInt32(reload_time_min_file);

            GlobalVars.dir = dir;
            GlobalVars.filename = filename;
            GlobalVars.pathreload = path;

            GlobalVars.dir2 = dir2;
            GlobalVars.filename2 = filename2;
            GlobalVars.pathreload2 = path2;

            //**Перезагрузка файла -- КОНЕЦ
            //----------------------------****************----------------------------

            //****Частота Ядра - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_clock))
            {
                reboot_clock = "0";
                _manager.WritePrivateString("main", "reboot_clock", reboot_clock);
            }
            GlobalVars.reboot_clock = reboot_clock;
            //частота ядра
            if (string.IsNullOrEmpty(clock))
            {
                clock = "500";
                _manager.WritePrivateString("main", "clock", clock);
            }
            GlobalVars.core_clock = Convert.ToInt32(clock);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_clock))
            {
                time_clock = "295";
                _manager.WritePrivateString("main", "time_clock", time_clock);
            }
            GlobalVars.time_clock = Convert.ToInt32(time_clock);
            //****Частота Ядра - КОНЕЦ

//----------------------------****************----------------------------

            //****Частоты памяти - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_memory))
            {
                reboot_memory = "0";
                _manager.WritePrivateString("main", "reboot_memory", reboot_memory);
            }
            GlobalVars.reboot_memory = reboot_memory;
            //частота
            if (string.IsNullOrEmpty(memory))
            {
                memory = "499";
                _manager.WritePrivateString("main", "memory", memory);
            }
            GlobalVars.memory = Convert.ToInt32(memory);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_memory))
            {
                time_memory = "301";
                _manager.WritePrivateString("main", "time_memory", time_memory);
            }
            GlobalVars.time_memory = Convert.ToInt32(time_memory);
            //****Частоты памяти - КОНЕЦ

 //----------------------------****************----------------------------


            //****Отвалом Карты - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_GPU)) {
                reboot_GPU = "0";
                _manager.WritePrivateString("main", "reboot_card", reboot_GPU);
            }
            GlobalVars.reboot_GPU = reboot_GPU;
                //колличество карт
            if (string.IsNullOrEmpty(count_GPU)) {
                count_GPU = "1";
                _manager.WritePrivateString("main", "count_GPU", count_GPU);
            }
            GlobalVars.count_GPU = Convert.ToInt32(count_GPU);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_count_GPU)) {
                time_count_GPU = "300";
                _manager.WritePrivateString("main", "time_count_GPU", time_count_GPU);
            }
            GlobalVars.time_count_GPU = Convert.ToInt32(time_count_GPU);
            //****Отвалом Карты - КОНЕЦ

            //----------------------------****************----------------------------

            //****Загрузка GPU - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_load_GPU))
            {
                reboot_load_GPU = "0";
                _manager.WritePrivateString("main", "reboot_load_GPU", reboot_load_GPU);
            }
            GlobalVars.reboot_load_GPU = reboot_load_GPU;
            //загрузка GPU в процентах
            if (string.IsNullOrEmpty(load_GPU))
            {
                load_GPU = "80";
                _manager.WritePrivateString("main", "load_GPU", load_GPU);
            }
            GlobalVars.load_GPU = Convert.ToInt32(load_GPU);
            //время перезагрузки
            if (string.IsNullOrEmpty(time_load_GPU))
            {
                time_load_GPU = "180";
                _manager.WritePrivateString("main", "time_load_GPU", time_load_GPU);
            }
            GlobalVars.time_load_GPU = Convert.ToInt32(time_load_GPU);


            //****Загрузка GPU - КОНЕЦ

            //----------------------------****************----------------------------

            //****НЕТУ ИНТЕРНЕТА - НАЧАЛО
            //проверка чекбокса
            if (string.IsNullOrEmpty(reboot_internet)) {
                reboot_internet = "0";
                _manager.WritePrivateString("main", "reboot_internet", reboot_internet);
            }
            GlobalVars.reboot_internet = reboot_internet;//yt nen dsasds ignore line)
                // время перезагрузки
            if (string.IsNullOrEmpty(time_internet)) {
                time_internet = "299";
                _manager.WritePrivateString("main", "time_internet", time_internet);
            }
            GlobalVars.time_internet = Convert.ToInt32(time_internet);
            //****НЕТУ ИНТЕРНЕТА КОНЕЦ


//----------------------------****************----------------------------

            //****Запуска программы через - НАЧАЛО
            if (string.IsNullOrEmpty(time_start))
            {
                time_start = "60";
                _manager.WritePrivateString("main", "time_start", time_start);
            }
            GlobalVars.time_start = Convert.ToInt32(time_start);

            //****Запуска программы через - КОНЕЦ

//----------------------------****************----------------------------

            bool start = false;
            GlobalVars.email = email;
            GlobalVars.name = name;
            GlobalVars.secret = secret;
            GlobalVars.token = token;
            GlobalVars.versions = version;

            
            
         
        //    tbToken.Text = GlobalVars.token;
//пока неиспользуется            
           // GlobalVars.stat = stat;
           // GlobalVars.pool = pool;
           // GlobalVars.wallet = wallet;
            //**
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(name) && string.IsNullOrEmpty(token))
            {
                tbEmail.Text = GlobalVars.email;
                tbSecret.Text = GlobalVars.secret;
                tbRigName.Text = GlobalVars.name;
                tbEmail.ReadOnly = true;
                tbSecret.ReadOnly = true;
                tbRigName.ReadOnly = true;
                start = true;
                
            }

            if (!string.IsNullOrEmpty(token))
            {
                start = true;
                tbEmail.ReadOnly = true;
                tbSecret.ReadOnly = true;
                tbRigName.ReadOnly = true;
                tbToken.ReadOnly = true;
                tbRigName.Text = name;
                tbToken.Text = token;
            }

            if ((string.IsNullOrEmpty(email) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(name)) && string.IsNullOrEmpty(token))
            {
                start = false;
                tbEmail.ReadOnly = true;
                tbSecret.ReadOnly = true;
                tbRigName.ReadOnly = true;
                
            }
            
                        
            if (start)
            {
                timer2.Interval = GlobalVars.time_start * 1000;
                timer2.Enabled = true;
                AutoStartTimer.Enabled = true;
                TimeWorkTimer.Enabled = true;
            }
           

        }

        private void GetTempretureTimerTick(object sender, EventArgs e)
        {
            try
            {
                btStart.Enabled = false;
                timer2.Enabled = false;
                AutoStartTimer.Enabled = false;
                btStop.Visible = true;
                gpu_temp();

                            
                
            }
            catch (Exception ex)
            {
               _error.writeLogLine(ex.Message + "timer GetTempretureTimerTick", "error");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
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
            SendData();
            Hide();
            
            
        }

        private void AutoStart_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GlobalVars.token))
            {
                tbToken.ReadOnly = true;
                timer2.Start();
                AutoStartTimer.Start();
                GlobalVars.time_start = GlobalVars.time_start - 1;
                btStart.Text = MyStrings.btStart + "(" + GlobalVars.time_start.ToString() + ")";

            }
            else {
                /*
                if (string.IsNullOrEmpty(GlobalVars.email))
                {
                    timer2.Stop();
                    AutoStartTimer.Stop();
                    MessageBox.Show("Введите EMAIL!");

                }
                else if (string.IsNullOrEmpty(GlobalVars.secret))
                {
                    timer2.Stop();
                    AutoStartTimer.Stop();
                    MessageBox.Show("Введите SECRET KEY!");

                }
                else if (string.IsNullOrEmpty(GlobalVars.name))
                {
                    timer2.Stop();
                    AutoStartTimer.Stop();
                    MessageBox.Show("Задайте имя ригу!");

                }
                else
                {
                
                }
                */
                timer2.Start();
                AutoStartTimer.Start();
                GlobalVars.time_start = GlobalVars.time_start - 1;
                btStart.Text = MyStrings.btStart +"(" + GlobalVars.time_start.ToString() + ")";
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
                                if (loadmin <= GlobalVars.load_GPU)
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
                                if (clockmin < GlobalVars.core_clock)
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
                                if (memorymin < GlobalVars.memory)
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
                        labelStatusReloadFile.ForeColor= Color.Green;
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
                _error.writeLogLine(ex.Message + "get parameters sensors", "error");
            }
        }
        public void SendData()
        {

                           
                try
                {

                    if (!string.IsNullOrEmpty(GlobalVars.token))
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
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/name", Encoding.UTF8.GetBytes(p));
                                    break;
                                case 1:
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/temp", Encoding.UTF8.GetBytes(p));
                                    break;
                                case 2:
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/core", Encoding.UTF8.GetBytes(p));
                                    break;
                                case 3:
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/memory", Encoding.UTF8.GetBytes(p));
                                    break;
                                case 4:
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/load", Encoding.UTF8.GetBytes(p));
                                    break;
                                case 5:
                                    client.Publish("devices/" + GlobalVars.token + "/gpu" + i + "/fan", Encoding.UTF8.GetBytes(p));
                                    break;




                            }
                            
                            // Console.WriteLine(p.Name);
                            labelTest.Text += " " + p;
                            j++;


                        }
                       

                         //   client.Publish("Pi/LEDControl2", Encoding.UTF8.GetBytes("SEND: " + labelTest.Text));
                        
                        labelTest.Text = labelTest.Text + "\n";
                        i++;
                        
                    }
                    */
                    
                    
                    GlobalVars.json_send = _http.GetContent(GlobalVars.host +
                        "/api.php?token=" + GlobalVars.token +
                        "&gpu=" + GlobalVars.card +
                        "&temp=" + GlobalVars.temp +
                        "&fan=" + GlobalVars.fan +
                        "&start_timestamp=" + GlobalVars.start_timestamp.ToString() +
                        "&v=" + GlobalVars.versions +
                        "&load=" + GlobalVars.load +
                        "&clock=" + GlobalVars.clock +
                        "&mem=" + GlobalVars.mem +
                        "&upTime=" + GlobalVars.upTime
                       
                       );
                    

                    client.Publish("devices/" + GlobalVars.token + "/data", Encoding.UTF8.GetBytes("token="+ GlobalVars.token +
                        "&gpu=" + GlobalVars.card +
                        "&temp=" + GlobalVars.temp +
                        "&fan=" + GlobalVars.fan +
                        "&start_timestamp=" + GlobalVars.start_timestamp.ToString() +
                        "&v=" + GlobalVars.versions +
                        "&load=" + GlobalVars.load +
                        "&clock=" + GlobalVars.clock +
                        "&mem=" + GlobalVars.mem +
                        "&upTime=" + GlobalVars.upTime
                                                ));
                    //  _log.writeLogLine("Отправка на сайт С токеном и получение ответа " + GlobalVars.json_send, "log");



                }
                    else if (string.IsNullOrEmpty(GlobalVars.token))
                    {
                    /*
                        GlobalVars.json_send = _http.GetContent(GlobalVars.host +
                        "/api.php?email=" + GlobalVars.email +
                        "&secret=" + GlobalVars.secret +
                        "&worker=" + GlobalVars.name +
                        "&gpu=" + GlobalVars.card +
                        "&temp=" + GlobalVars.temp +
                        "&fan=" + GlobalVars.fan +
                        "&start_timestamp=" + GlobalVars.start_timestamp.ToString() +
                        "&v=" + GlobalVars.versions +
                        "&load=" + GlobalVars.load +
                        "&clock=" + GlobalVars.clock +
                        "&mem=" + GlobalVars.mem +
                        // "&pool=" + GlobalVars.pool +
                        "&hash=" + "417");
                        //  _log.writeLogLine("Отправка на сайт БЕЗ токена и получение ответа " + GlobalVars.json_send, "log");

                    */
                    }
                    //GlobalVars.json_send = "{"token":"73b41e62c748693a46d5bd6603dc51a0","settings":{"interval":60},"message":"Use token endpoint"}";



                    if (!string.IsNullOrWhiteSpace(GlobalVars.json_send))
                    {


                        //var interval = JsonConvert.DeserializeObject<ApiResponse>(File.ReadAllText("json.json"));
                        var response = JsonConvert.DeserializeObject<ApiResponse>(GlobalVars.json_send);
                   // MessageBox.Show(this, GlobalVars.json_send, "Message", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    int test = response.settings.interval;
                        string test2 = test.ToString();

                        // _log.writeLogLine("Интервал " + test2, "log");

                        SendDataTimer.Interval = response.settings.interval * 1000;




                        if (GlobalVars.token != response.token && !string.IsNullOrEmpty(response?.token))
                        {

                            // _log.writeLogLine("Токен получен " + GlobalVars.json_send, "log");
                            //  var token = JsonConvert.DeserializeObject<ApiResponse>(GlobalVars.json_send);
                            GlobalVars.token = response.token;
                            _manager.WritePrivateString("main", "token", GlobalVars.token);
                            tbToken.Text = GlobalVars.token;

                        }

                        if (GlobalVars.name != response.settings.name && !string.IsNullOrEmpty(response.settings?.name))
                        {

                            // _log.writeLogLine("Токен получен " + GlobalVars.json_send, "log");
                            //  var token = JsonConvert.DeserializeObject<ApiResponse>(GlobalVars.json_send);
                            GlobalVars.name = response.settings.name;
                            _manager.WritePrivateString("main", "name", GlobalVars.name);
                            tbRigName.Text = GlobalVars.name;

                        }

                        InformationLabel.Text = "Authorization OK";
                        InformationLabel.ForeColor = Color.Green;
                        /*
                        if (GlobalVars.json_send == "Auth failed")
                        {
                            MessageBox.Show("Auth failed! неверный токен");
                        }
                        */
                    }
                    else
                    {
                        if (GlobalVars.InternetIsActive == true)
                        {

                            //MessageBox.Show("Auth failed! Possibly incorrect token!");
                            // _error.writeLogLine(ex.Message, "error");
                            // SendDataTimer.Interval = 300 * 1000;
                            InformationLabel.Text = "Authorization failed";
                            InformationLabel.ForeColor = Color.Red;

                        }

                    }







                }
                catch (Exception ex)
                {

                    _error.writeLogLine(ex.Message + "function send() Send to site " + GlobalVars.json_send, "error");

                }
                                   
        }

        
        


        public void СheckForNewVersion()
        {
            if (GlobalVars.InternetIsActive == true)
            {
                try
                {
                    string v = _manager.GetPrivateString("main", "version");
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
                            _error.writeLogLine(ex.Message, "error");
                        }
                    }

                }
                catch (Exception ex)
                {
                    _error.writeLogLine(ex.Message, "error");
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
            labelTimeWork2.Text = ("Days: " +d.ToString()+ " Hours: " +h.ToString()+ " Minute: " +min.ToString());
        }

        public void Reboot(string msg, string bat)
        {
            try
            {
                // GetTempretureTimer.Enabled = false;
                // timer2.Enabled = false;
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
                _error.writeLogLine(ex.Message, "error");
            }
        }


        public void Message(string msg)
        {
            try
            {
                _http.GetContent(
                    GlobalVars.host +
                    "/api.php?token=" + GlobalVars.token +
                    "&event=" + "message" +
                    "&reason=" + GlobalVars.name + " " + msg
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

                _log.writeLogLine("Message " + msg, "log");
            }

            catch (Exception ex)
            {
                _error.writeLogLine(ex.Message, "error");
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
                    "/api.php?email=" + GlobalVars.email +
                    "&secret=" + GlobalVars.secret +
                    "&worker=" + GlobalVars.name + 
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
                _error.writeLogLine(ex.Message, "error");
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



        private void MqttConnect()

        {
            try
            {
                
                bool code2 = client.IsConnected;
                if (code2 == true)
                {
                    labelTest.Text = Convert.ToString(code2) + " " + GlobalVars.mqttcheck;
                    GlobalVars.mqttcheck = 0;
                    //пушь
                }
                else if(code2 == false)
                {
                    client.ProtocolVersion = MqttProtocolVersion.Version_3_1;

                    //byte code = client.Connect(Guid.NewGuid().ToString(), "aleksei", "256eb460f1", false, 3);
                    //byte code = client.Connect(GlobalVars.token, "aleksei", "256eb460f1", false, 3);
                    byte code = client.Connect(GlobalVars.token, GlobalVars.token, GlobalVars.token, false,3);
                    if (code == 0)
                    {

                        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                        client.Subscribe(new string[] { "devices/" + GlobalVars.token + "/data", "devices/" + GlobalVars.token + "/commands" }, new byte[] { 0, 0 });

                        labelStatusInternetPing.Text = "MQTT ON";
                        labelTest.Text = Convert.ToString(code2) + Convert.ToString(code);
                    }

                    else if (code != 0 && GlobalVars.mqttcheck <=3)
                    {
                        labelStatusInternetPing.Text = "MQTT OFF Connect Fail";
                        GlobalVars.mqttcheck++;
                        labelTest.Text = Convert.ToString(code2) + " " + Convert.ToString(code) + " " + GlobalVars.mqttcheck;
                    }
                    
                }
            }

            catch (Exception)
            {
                labelStatusInternetPing.Text = "MQTT OFF Wrong Format";
               
            }
        

        }

        private void BtStartClick(object sender, EventArgs e)
        {

            MqttConnect();
            
            GlobalVars.start_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            //labelTest.Text = GlobalVars.start_timestamp.ToString();
            string email       = tbEmail.Text;
            string secret      = tbSecret.Text;
            string name = tbRigName.Text;
            string token = tbToken.Text;

            if (!string.IsNullOrEmpty(token))
            {
                _manager.WritePrivateString("main", "token", token);
                GetTempretureTimer.Enabled = true;
                PingInternetTimer.Start();
                SendDataTimer.Enabled = true;
                btStart.Enabled = false;
                btStop.Visible = true;
                AutoStartTimer.Enabled = false;

                GlobalVars.token = token;
                GlobalVars.name = name;
                gpu_temp();
               
                _log.writeLogLine("Informer is started ", "log");
                Message("Informer Started!");
                SendData();

                GlobalVars.timeOnline = 0;
               // InformationLabel.Text = "Запущен";
               // InformationLabel.ForeColor = Color.Green;
                tbEmail.ReadOnly = true;
                tbSecret.ReadOnly = true;
                tbRigName.ReadOnly = true;
                tbToken.ReadOnly = true;
                
            }
            else {

                if (string.IsNullOrEmpty(token) && (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(name)))
                {


                    _manager.WritePrivateString("main", "token", token);
                    GetTempretureTimer.Enabled = true;

                    SendDataTimer.Enabled = true;
                    btStart.Enabled = false;
                    btStop.Visible = true;
                    AutoStartTimer.Enabled = false;

                    GlobalVars.token = token;
                    GlobalVars.name = name;
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
                    MessageBox.Show("Enter the token!");

                }


            }
              
            InitFromIni();
        }

        private void BtnOpenSettingsFormClick(object sender, EventArgs e)
        {
            timer2.Stop();
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
            СheckForMQTT();
            SendData();
            
        }

        

        private void GpuCoreMinHzTimerTick(object sender, EventArgs e)
        {
            const string msg = "Core Min , Reboot!";
            const string bat = "reboot_clock.bat";
            if (GlobalVars.timer_clock < 0)
            {
                GlobalVars.timer_clock = GlobalVars.time_clock;
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
                GlobalVars.timer_memory = GlobalVars.time_memory;
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

                bool code = client.IsConnected;
                GlobalVars.upTime= UpTime.ToString(@"dd\.hh\:mm\:ss");

                if (code == true)
                {
                    labelStatusInternetPing.Text = "MQTT OFF";
                    client.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("OK"));
                    client.Publish("devices/" + GlobalVars.token + "/UpTime", Encoding.UTF8.GetBytes("" + GlobalVars.upTime));


                }
                else if (code == false)
                {
                    //  labelStatusInternetPing.Text = "MQTT OFF";
                    MqttConnect();
                    if (client.IsConnected == true) {
                        labelTest.Text = "GOOD";
                    client.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("Reconnected"));
                    client.Publish("devices/" + GlobalVars.token + "/MQTT_PING", Encoding.UTF8.GetBytes("ON"));
                        }
                  

                }



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

      private void PingTimerTick(object sender, EventArgs e)
          {
           
            СheckForMQTT();

          }

        private void InternetInactiveTimerTick(object sender, EventArgs e)
        {
            const string bat = "reboot_internet.bat";

            DontHaveInternetTimer.Enabled = false;
            try
            {
                if (GlobalVars.timer_inet < 0)
                {
                    GlobalVars.timer_inet = GlobalVars.time_internet;
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
                    GlobalVars.timer_load_gpu = GlobalVars.time_load_GPU;
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

        

        
private void tbRigName_TextChanged(object sender, EventArgs e)
{
   GlobalVars.name = tbRigName.Text;
   _manager.WritePrivateString("main", "name", GlobalVars.name);
}

private void tbSecret_TextChanged(object sender, EventArgs e)
{
   GlobalVars.secret = tbSecret.Text;
   _manager.WritePrivateString("main", "secret", GlobalVars.secret);
}

private void tbEmail_TextChanged(object sender, EventArgs e)
{
   GlobalVars.email = tbEmail.Text;
   _manager.WritePrivateString("main", "email", GlobalVars.email);
}

        private void tbToken_TextChanged(object sender, EventArgs e)
        {
            GlobalVars.token = tbToken.Text;
            _manager.WritePrivateString("main", "token", GlobalVars.token);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            bool code = client.IsConnected;

            try
            {
                if (code == true)
                {

                    System.Threading.Thread.Sleep(200);
                    client.Disconnect();
                    Properties.Settings.Default.Language = cbLocalize.SelectedValue.ToString();
                    Properties.Settings.Default.Save();
                    this.ShowInTaskbar = false;
                    Application.Exit();


                }
                else if (code == false)
                {
                    Properties.Settings.Default.Language = cbLocalize.SelectedValue.ToString();
                    Properties.Settings.Default.Save();
                    this.ShowInTaskbar = false;
                    Application.Exit();

                }
            }
            catch (Exception ex)
            {
                _error.writeLogLine(ex.Message, "error");
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

        Action<string, string> ReceiveAction;
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            ReceiveAction = Receive;
            try
            {
                this.BeginInvoke(ReceiveAction, Encoding.UTF8.GetString(e.Message), e.Topic);
            }
            catch { };
        }

        void Receive(string message, string topic)
        {

            if (topic == "devices/" +GlobalVars.token+ "/commands")
            {

                var response = JsonConvert.DeserializeObject<ApiResponse>(message);
                string command = response.command;
                switch (command)
                {

                    case "reboot":
                     
                        Message("Informer Reboot from Allminer.ru!");
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;
                }

               
            }
            

        }


    }
}
