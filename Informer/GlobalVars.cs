using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
using MQTTnet.Client;
using MQTTnet;
using Informer;
using System.Windows.Forms;

public class GlobalVars
{
    public string host = "http://www.allminer.ru";
    public Timers Timer;
    public Reboots Reboots;
    public Data_ranges Ranges;

    //public string name;
    public string token;
    public string upTime;
    public string versions;
    public string card;
    public string temp;
    public string fan;
    public int autostart = 60;
    public int start_timestamp;
    public int timeOnline = 0;
    public int reload_temp_min_file;
    public int reload_time_min_file;
    public string link;
    public string pathreload;
    public string filename;
    public string dir;
    public string pathreload2;
    public string filename2;
    public string dir2;
    public string reload_file;
    public string reboot_temp_max;
    public string reboot_temp_min;
    public string load;
    public int counts;

    public string reboot_max_fan;
    public string reboot_min_fan;
    public string reboot_clock;
    public string reboot_memory;
    public string reboot_GPU;
    public string reboot_load_GPU;
    public string reboot_internet;
    public int timer_r_min = -100;
    public int timer_t_min = -100;
    public int timer_t_max = -100;
    public int timer_fan_max = -100;
    public int timer_fan_min = -100;
    public int timer_clock_min = -100;
    public int timer_clock_max = -100;
    public int timer_memory_min = -100;
    public int timer_memory_max = -100;
    public int timer_inet = -100;
    public int timer_gpu_lost = -100;
    public int timer_load_gpu_min = -100;
    public int timer_load_gpu_max = -100;

    public int interval = 60;


    public int time_count_GPU;


    public int count_GPU = 0;

    public string clock;
    public string mem;


    public bool reboot1 = false;
    public bool reboot2 = false;
    public bool reboot3 = false;
    public bool reboot4 = false;
    public bool reboot5 = false;
    public bool coreMax = false;
    public bool reboot6 = false;
    public bool memMax = false;
    public bool temp0 = false;
    public bool gpu_lost = false;
    public bool firsrun = true;
    public bool IsRebootStarted = false;
    public bool rebootDontHaveInternet = false;
    public bool rebootLoadGPU = false;
    public bool InternetIsActive = false;
    public bool mqttIsConnect = false;
    public int pingCount;
    public bool ping = false;

    public string[] miners = { "ccminer.exe", "ethminer.exe", "excavator.exe", "nheqminer.exe", "sgminer.exe", "xmr-stak-cpu.exe", "NsGpuCNMiner.exe", "EthDcrMiner64.exe", "ZecMiner64.exe", "miner.exe", "Optiminer.exe", "prospector.exe" };

    public List<Dictionary<string, string>> gpuList = new List<Dictionary<string, string>>();

    public INIManager _manager;
    public string fullPath;
    public MqttFactory factory = new MqttFactory();
    public IMqttClient client;

    public bool tokenMqtt = false;
    public Computer _pc = new Computer();

    public List<bool> problemPing = new List<bool>();

    public GlobalVars()
    {
        fullPath = Application.StartupPath.ToString();
        _manager = new INIManager(fullPath + "\\my.ini");
    }
}
