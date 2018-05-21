﻿using System;
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
static class GlobalVars
{
    public static string host = "http://www.allminer.ru";
    public static string email;
    public static string secret;
   // public static string worker;
    public static string name;
    public static string token;
    public static string upTime;
    public static string json_send;
    public static string versions;
   // public static string stat;
   // public static string pool;
   // public static string hash;
    public static string wallet;
    public static string card;
    public static string temp;
    public static string fan;
    public static int mqttcheck;
    public static int temp_max;
    public static int temp_min;
    public static int time_temp_min;
    public static int time_temp_max;
    public static int time_start;
    public static int start_timestamp;
    public static int timeOnline = -1;
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
    public static int fan_max;
    public static int fan_min;
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
    public static int timer_clock = -100;
    public static int timer_memory = -100;
    public static int timer_inet = -100;
    public static int timer_t_card = -100;
    public static int timer_load_gpu = -100;
    public static int time_fan_min;
    public static int time_fan_max;
    public static int time_clock;
    public static int time_memory;
    public static int core_clock;
    public static int memory;
    public static int count_GPU;
    public static int time_count_GPU;
    public static int time_internet;
    public static int load_GPU;
    public static int time_load_GPU;
    public static string clock;
    public static string mem;


    public static bool reboot1 = false;
    public static bool reboot2 = false;
    public static bool reboot3 = false;
    public static bool reboot4 = false;
    public static bool reboot5 = false;
    public static bool reboot6 = false;
    public static bool IsRebootStarted = false;
    public static bool rebootDontHaveInternet = false;
    public static bool rebootLoadGPU = false;
    public static bool InternetIsActive = false;
    public static string[] miners = { "ccminer.exe", "ethminer.exe", "excavator.exe", "nheqminer.exe", "sgminer.exe", "xmr-stak-cpu.exe", "NsGpuCNMiner.exe", "EthDcrMiner64.exe", "ZecMiner64.exe", "miner.exe", "Optiminer.exe", "prospector.exe" };
    public static Dictionary<int, List<string>> gpuList;

    //public static bool nice;
    //итд
}
