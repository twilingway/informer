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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;

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
        // public Settings settings { get; set; }
        //public RootObject root { get; set; }
        public string token { get; set; }
        public Settings settings { get; set; }
        //public string name { get; set; }
        public string message { get; set; }

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



}

