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
class LogFile
{
    private StreamWriter sw;


    public LogFile(string path)
    {
        try
        {
            using (sw = new System.IO.StreamWriter(path + ".log", true, Encoding.UTF8))
            {
                sw.Flush();
                sw.Close();
            }


        }
        catch (System.IO.IOException e)
        {
            System.Windows.Forms.MessageBox.Show(e.ToString());
        }
    }
    ~LogFile()
    {

         sw.Close();
    }

    public void writeLogLine(string line, string path)
    {
        try
        {
            using (sw = new System.IO.StreamWriter(path + ".log", true, Encoding.UTF8))
            {
                DateTime presently = DateTime.Now;
                line = presently.ToString() + " - " + line;
                sw.WriteLine(line);
                sw.Flush();
                sw.Close();
            }
        }
        catch (System.IO.IOException e)
        {
            System.Windows.Forms.MessageBox.Show(e.ToString());
        }

    }
}