using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer
{
    public static class CommandProcesser
    {

        public static void onMessage(string payload, string topic, GlobalVars globalVars)
        {

            if (topic == "devices/" + globalVars.token + "/commands")
            {

                // Debug.WriteLine(message);
                var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
                string command = response.command;
                switch (command)
                {

                    case "reboot":

                        MainForm.Message("Informer Reboot from Allminer.ru!", globalVars);
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;

                    case "settings":
                        try
                        {
                            globalVars.Ranges = response.Params.data_ranges;
                            globalVars.Timer = response.Params.timers;
                            globalVars.Reboots = response.Params.reboots;
                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine("CommandProcesser " + ex);

                        }

                        break;


                    case "interval":
                        try
                        {
                            globalVars.interval = response.Params.interval;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Interval " + ex);
                        }

                        break;
                }
            }
        }
    }
}


