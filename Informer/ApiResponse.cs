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
       // public string test { get; set; }
        public Params Params { get; set; }
        public string Command { get; set; }
       
        public void Save()
        {
            var state = JsonConvert.SerializeObject(this);
            //var state = settings;
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

        public bool GetReboot(int i)
        {
            switch (i)
            {
                case 0: //temp Min
                    return Reboots.temp_min;
                    break;
                case 1:
                    return Reboots.temp_max;
                default:
                    return false;
                    break;
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
                    break;
                default:
                    return 300;
                    break;

            }
        }

        public int[] GetRanges(int i)
        {
            switch (i)
            {
                case 0: //temp Min
                    return Data_ranges.Temp;
                    break;
                case 1:
                    return Data_ranges.Fan;
                default:
                    int[] @default = {0, 100};
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

