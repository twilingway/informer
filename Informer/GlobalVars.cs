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
using System.Net;
//using HardwareMonitor.Hardware;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
using MQTTnet.Client;
using MQTTnet;
using System.Threading;
//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;
static class GlobalVars
{
    public static string host = "http://www.allminer.ru";



    public static string name;
    public static string token;
    public static string upTime;
    //public static string json_send;
    //public static string mqttsetparams;
    public static string versions;
    //public static string wallet;
    public static string card;
    public static string temp;
    public static string fan;
    //public static int mqttcheck = 0;




    public static int autostart = 60;
    public static int start_timestamp;
    public static int timeOnline = 0;
    public static int reload_temp_min_file;
    public static int reload_time_min_file;
    public static string link;
    public static string pathreload;
    public static string filename;
    public static string dir;
    public static string pathreload2;
    public static string filename2;
    public static string dir2;
    public static string reload_file;
    public static string reboot_temp_max;
    public static string reboot_temp_min;
    public static string load;
    public static int counts;

    public static string reboot_max_fan;
    public static string reboot_min_fan;
    public static string reboot_clock;
    public static string reboot_memory;
    public static string reboot_GPU;
    public static string reboot_load_GPU;
    public static string reboot_internet;
    public static int timer_r_min = -100;
    public static int timer_t_min = -100;
    public static int timer_t_max = -100;
    public static int timer_fan_max = -100;
    public static int timer_fan_min = -100;
    public static int timer_clock_min = -100;
    public static int timer_clock_max = -100;
    public static int timer_memory_min = -100;
    public static int timer_memory_max = -100;
    public static int timer_inet = -100;
    public static int timer_gpu_lost = -100;
    public static int timer_load_gpu_min = -100;
    public static int timer_load_gpu_max = -100;

    public static int time_temp_min;
    public static int time_temp_max;
    public static int time_fan_min;
    public static int time_fan_max;
    public static int time_load_GPU_min;
    public static int time_load_GPU_max;
    public static int time_clock_min;
    public static int time_clock_max;
    public static int time_mem_min;
    public static int time_mem_max;
    public static int time_lost_gpu;
    public static int time_lost_inet;


    public static bool reboots_temp_min;
    public static bool reboots_temp_max;
    public static bool reboots_fan_min;
    public static bool reboots_fan_max;
    public static bool reboots_load_min;
    public static bool reboots_load_max;
    public static bool reboots_clock_min;
    public static bool reboots_clock_max;
    public static bool reboots_mem_min;
    public static bool reboots_mem_max;
    public static bool reboots_lost_gpu;
    public static bool reboots_lost_inet;


    public static int temp_max;
    public static int temp_min;
    public static int mem_min;
    public static int mem_max;
    public static int load_GPU_min;
    public static int load_GPU_max;
    public static int fan_min;
    public static int fan_max;
    public static int clock_min;
    public static int clock_max;

    public static int interval = 60;


    public static int time_count_GPU;


    public static int count_GPU = 0;




    public static string clock;
    public static string mem;


    public static bool reboot1 = false;
    public static bool reboot2 = false;
    public static bool reboot3 = false;
    public static bool reboot4 = false;
    public static bool reboot5 = false;
    public static bool coreMax = false;
    public static bool reboot6 = false;
    public static bool memMax = false;
    public static bool temp0 = false;
    public static bool firsrun = true;
    public static bool IsRebootStarted = false;
    public static bool rebootDontHaveInternet = false;
    public static bool rebootLoadGPU = false;
    public static bool InternetIsActive = false;
    public static bool mqttIsConnect = false;
    public static int pingCount;
    public static bool ping = false;

    public static string[] miners = { "ccminer.exe", "ethminer.exe", "excavator.exe", "nheqminer.exe", "sgminer.exe", "xmr-stak-cpu.exe", "NsGpuCNMiner.exe", "EthDcrMiner64.exe", "ZecMiner64.exe", "miner.exe", "Optiminer.exe", "prospector.exe" };
    public static Dictionary<int, List<string>> gpuList = new Dictionary<int, List<string>>();
  
    public static string fullPath = Application.StartupPath.ToString();
    public static INIManager _manager = new INIManager(fullPath + "\\my.ini");
  
    public static MqttFactory factory = new MqttFactory();
    public static IMqttClient client;
   // public static CancellationTokenSource cancelTokenSource;
    public static bool tokenMqtt = false;
    public static Computer _pc = new Computer();
    public static List<String> gpusList = new List<string>();
    public static List<bool> problemPing = new List<bool>();
  

    //public static bool nice;
    //итд
}
