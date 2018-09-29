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

        public static void onMessage(string payload, string topic, GlobalVars globalVars,ApiResponse settings)
        {

            if (topic == "devices/" + settings.Params.token + "/commands")
            {

                // Debug.WriteLine(message);
                var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
                string command = response.Params.command;
                switch (command)
                {

                    case "reboot":
                        MainForm.Message("Informer Reboot from Allminer.ru!", globalVars,settings);
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;

                    case "settings":
                        try
                        { 
                            //response.Params.timers = response.Params.timers;
                            settings.Params.timers = response.Params.timers;
                            settings.Params.reboots = response.Params.reboots;
                            settings.Params.data_ranges = response.Params.data_ranges;
                            globalVars.Save(response);
                            Debug.WriteLine("#############" + response.Params.timers.temp_min);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("############# CommandProcesser " + ex);
                        }

                        break;


                    case "interval":
                        try
                        {
                            settings.Params.interval = response.Params.interval;
                        }
                        catch (Exception ex)
                        {
                          //  Debug.WriteLine("Interval " + ex);
                        }

                        break;
                }
            }
        }
    }
}


