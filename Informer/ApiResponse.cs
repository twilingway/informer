using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

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
       
        public void Save(ApiResponse settings)
        {
            var state = JsonConvert.SerializeObject(settings);
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
                return null;
            }
        }
    }
    

    public class Params
    {
        public Timers Timers { get; set; }
        public Reboots Reboots { get; set; }
        public Data_ranges Data_ranges { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
    }

    public class Timers
    {
        public int temp_min { get; set; }
        public int temp_max { get; set; }
        public int fan_min { get; set; }
        public int fan_max { get; set; }
        public int load_min { get; set; }
        public int load_max { get; set; }
        public int clock_min { get; set; }
        public int clock_max { get; set; }
        public int mem_min { get; set; }
        public int mem_max { get; set; }
        public int lost_gpu { get; set; }
        public int lost_inet { get; set; }
        public int autostart { get; set; }
    }

    public class Reboots
    {
        public bool temp_min { get; set; }
        public bool temp_max { get; set; }
        public bool fan_min { get; set; }
        public bool fan_max { get; set; }
        public bool load_min { get; set; }
        public bool load_max { get; set; }
        public bool clock_min { get; set; }
        public bool clock_max { get; set; }
        public bool mem_min { get; set; }
        public bool mem_max { get; set; }
        public bool lost_gpu { get; set; }
        public bool lost_inet { get; set; }
    }

    public class Data_ranges
    {
        public int[] Temp { get; set; }
        public int[] Fan { get; set; }
        public int[] Load { get; set; }
        public int[] Clock { get; set; }
        public int[] Mem { get; set; }
    }
}

