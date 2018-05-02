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


public class PoolInfoResponse
{
    public string workers { get; set; }
    public bool status { get; set; }
    public List<Datum> data { get; set; }
    //nice
    public Result result { get; set; }
    public string method { get; set; }
}



