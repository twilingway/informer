using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Informer
{
    public static class CommandProcesser
    {

        public static void onMessage(string payload, string topic, ApiResponse apiResponse)
        {

            if (topic == "devices/" + apiResponse.Params.Token + "/commands")
            {
                
                // Debug.WriteLine(message);
                var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
                string command = response.Command;
                response.Params.Token = apiResponse.Params.Token;
                response.Params.Version = apiResponse.Params.Version;
                Debug.WriteLine("COMMAND: "+ command);
                switch (command)
                {

                    case "reboot":
                      //  MainForm.Message("Informer Reboot from Allminer.ru!", settings);
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;

                    case "settings":
                        try
                        {
                            Debug.WriteLine("SETTINGS");
                            //response.Params.timers = response.Params.timers;
                            apiResponse.Params = response.Params;
                           // settings.Params.reboots = response.Params.reboots;
                           // settings.Params.data_ranges = response.Params.data_ranges;
                            apiResponse.Save(response);
                            Debug.WriteLine("#############" + response.Params.Timers.temp_min);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("############# CommandProcesser " + ex);
                        }

                        break;


                    case "interval":
                        try
                        {
                            apiResponse.Params.Interval = response.Params.Interval;
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


