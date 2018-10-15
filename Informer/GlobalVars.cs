using System;
using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;
using MQTTnet.Client;
using MQTTnet;
using Informer;
using System.Windows.Forms;

public class GlobalVars
{
    public string host = "http://www.allminer.ru";

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

    public string load;
    public int counts;

    public int interval = 60;
    public int time_count_GPU;
    public int count_GPU = 0;
    public string clock;
    public string mem;


  
    public bool temp0 = false;
 
    public bool firsrun = true;
    public bool IsRebootStarted = false;
    public bool rebootDontHaveInternet = false;
  //  public bool rebootLoadGPU = false;
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
    public List<bool> problemPing = new List<bool>();

    public GlobalVars()
    {
        fullPath = Application.StartupPath.ToString();
        _manager = new INIManager(fullPath + "\\my.ini");
    }
}
