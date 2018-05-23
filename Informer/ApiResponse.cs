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
        public int interval { get; set; }
        public string name { get; set; }
    }
    /*
    public partial class MqttGetSettings
    {
        public static MqttGetSettings FromJson(string json) => JsonConvert.DeserializeObject<MqttGetSettings>(json, Informer.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MqttGetSettings self) => JsonConvert.SerializeObject(self, Informer.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    */
}

