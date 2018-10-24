using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Informer.Sensors;

/// <summary>
/// Класс представляет собой описание ответа от allminer.ru
/// </summary>
/// 
namespace Informer
{
    public class ApiResponse
    {
      
        public Params Params { get; set; }
        public string Command { get; set; }
       
        public void Save()
        {
            var state = JsonConvert.SerializeObject(this);
            var file = File.Open("state.json", FileMode.Create);
            try
            {
                using (StreamWriter writter = new StreamWriter(file))
                {
                    writter.WriteLine(state);
                    Debug.WriteLine("SAVE OK:" + state);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SAVE WRONG:" + e.Message);
            }
        }

        public ApiResponse Load()
        {
            try
            {
                using (StreamReader sr = new StreamReader("state.json"))
                {
                    var state = sr.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<ApiResponse>(state);
                    Debug.WriteLine("***************** " + response.Params.Version);
                    return response;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("&&&&&&&&&&&&&&&" + e.Message);
                return this;
            }
        }
    }


    public class Params
    {
        public Timers Timers { get; set; }
        public Reboots Reboots { get; set; }
        public Data_ranges Data_ranges { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; } = 60;
        public string Token { get; set; }
        public string Version { get; set; } = "1.3.9";

        public void Update(OHMMonitoringSystem monitoringSystem)
        {
            var triggers = monitoringSystem.GetTriggers();

            for (int i = 0; i < triggers.Count; i++)
            {
                if (GetReboot(i))
                {
                    triggers[i].Enable();
                }
                else
                {
                    triggers[i].Disable();
                }

                triggers[i].WaitTimeInMilleseconds = (GetTimer(i) * 1000f);

                if (triggers[i].Sensor is MultiplyHardwareRangeSensor)
                {
                    ((MultiplyHardwareRangeSensor)triggers[i].Sensor).UpdateRange(GetRanges(i));
                }
            }
        }
//{"params":{"timers":{"temp_min":230,"temp_max":250,"mem_min":299,"mem_max":299,"lost_inet":266,"lost_gpu":299,"load_min":299,"load_max":299,
//"fan_min":280,"fan_max":280,"clock_min":299,"clock_max":250,"autostart":25},"reboots":{"temp_min":true,"temp_max":true,"mem_min":true,"mem_max":true,"lost_inet":true,"lost_gpu":false,"load_min":true,"load_max":true,"fan_min":true,"fan_max":true,"clock_min":true,"clock_max":true},"name":"TwilingWay3","interval":60,"data_ranges":{"temp":[40,38],"mem":[4247,4249],"load":[0,100],"fan":[32,80],"clock":[300,2000]

        public bool GetReboot(int i)
        {
            switch (i)
            {
                case 0: //temp Min
                    return Reboots.temp_min;
                case 1:
                    return Reboots.temp_max;
                case 2:
                    return Reboots.mem_min;
                case 3:
                    return Reboots.mem_max;
                case 4:
                    return Reboots.lost_inet;
                case 5:
                    return Reboots.lost_gpu;
                case 6:
                    return Reboots.load_min;
                case 7:
                    return Reboots.load_max;
                case 8:
                    return Reboots.fan_min;
                case 9:
                    return Reboots.fan_max;
                case 10:
                    return Reboots.clock_min;
                case 11:
                    return Reboots.clock_max;
                default:
                    return false;
                  
            }

        }

        public int GetTimer(int i)
        {
            switch (i)
            {
                case 0: //temp Min
                    return Timers.temp_min;
                case 1:
                    return Timers.temp_max;
                case 2:
                    return Timers.mem_min;
                case 3:
                    return Timers.mem_max;
                case 4:
                    return Timers.lost_inet;
                case 5:
                    return Timers.lost_gpu;
                case 6:
                    return Timers.load_min;
                case 7:
                    return Timers.load_max;
                case 8:
                    return Timers.fan_min;
                case 9:
                    return Timers.fan_max;
                case 10:
                    return Timers.clock_min;
                case 11:
                    return Timers.clock_max;
                case 12:
                    return Timers.autostart;
                default:
                    return 300;
                 

            }
        }

//        "data_ranges":{"temp":[40,38],"mem":[4247,4249],"load":[0,100],"fan":[32,80],"clock":[300,2000]

        public int[] GetRanges(int i)
        {
            switch (i)
            {
                case 0: //temp Min
                    return Data_ranges.Temp;
                case 1:
                    return Data_ranges.Mem;
                case 2:
                    return Data_ranges.Load;
                case 3:
                    return Data_ranges.Fan;
                case 4:
                    return Data_ranges.Clock;
                default:
                    int[] @default = {0, 10000};
                    return @default;
            }
        }
    }

    public class Timers
    {
        public int temp_min { get; set; } = 300;
        public int temp_max { get; set; } = 300;
        public int fan_min { get; set; } = 300;
        public int fan_max { get; set; } = 300;
        public int load_min { get; set; } = 300;
        public int load_max { get; set; } = 300;
        public int clock_min { get; set; } = 300;
        public int clock_max { get; set; } = 300;
        public int mem_min { get; set; } = 300;
        public int mem_max { get; set; } = 300;
        public int lost_gpu { get; set; } = 300;
        public int lost_inet { get; set; } = 300;
        public int autostart { get; set; } = 300;
    }

    public class Reboots
    {
        public bool temp_min { get; set; } = false;
        public bool temp_max { get; set; } = false;
        public bool fan_min { get; set; } = false;
        public bool fan_max { get; set; } = false;
        public bool load_min { get; set; } = false;
        public bool load_max { get; set; } = false;
        public bool clock_min { get; set; } = false;
        public bool clock_max { get; set; } = false;
        public bool mem_min { get; set; } = false;
        public bool mem_max { get; set; } = false;
        public bool lost_gpu { get; set; } = false;
        public bool lost_inet { get; set; } = false;
       
    }

    public class Data_ranges
    {
        public int[] Temp { get; set; } = { 0, 100 };
        public int[] Fan { get; set; } = { 0, 100 };
        public int[] Load { get; set; } = { 0, 100 };
        public int[] Clock { get; set; } = { 0, 5000 };
        public int[] Mem { get; set; } = { 0, 10000 };
    }
}

