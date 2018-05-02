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
//class nanopul
public class Datum
{
    public string id { get; set; }
    public double hashrate { get; set; }
    public int lastShare { get; set; }
    public int rating { get; set; }
}
