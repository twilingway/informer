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
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Класс представляет собой описание ответа от allminer.ru
/// </summary>
/// 
namespace Informer
{
    public class ApiResponse
    {
        /// <summary>
        /// Настройки Informer-а хранящиеся на сервере
        /// </summary>

        public string token { get; set; }
        public Settings settings { get; set; }
      
        public string message { get; set; }

      //  [JsonProperty("params")]
        public Params Params { get; set; }

      //  [JsonProperty("commands")]
        public string command { get; set; }
    }




    public class Settings
    {
        /// <summary
        /// Интервал в секундах, как часто отправлять данные
        /// </summary>
        public int interval { get; set; }
        public string name { get; set; }
        /// <summary>
        /// </summary>

    }
    /*
    public partial class MqttGetSettings
    {
        [JsonProperty("params")]
        public Params Params { get; set; }

        [JsonProperty("commands")]
        public string Commands { get; set; }
    }
    */

    public partial class Params
    {
        public Timers timers { get; set; }
        public Reboots reboots { get; set; }
        public Data_ranges data_ranges { get; set; }
        public string name { get; set; }
        public int interval { get; set; }

    }

    public class Timers
    {

        public int temp_min { get; set; }
        public int temp_max { get; set; }
        public int fan_min { get; set; }
        public int interval { get; set; }
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

