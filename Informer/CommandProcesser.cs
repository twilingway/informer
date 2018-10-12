using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Informer
{
    public class CommandProcesser
    {
        ApiResponse apiResponse;

        public CommandProcesser(ApiResponse apiResponse)
        {
            this.apiResponse = apiResponse;
        }

        public ApiResponse GetApiResponse()
        {
            return apiResponse;
        }
        public void onMessage(string payload, string topic)
        {
            string token = apiResponse.Params.Token;
            string version = apiResponse.Params.Version;


            if (topic == "devices/" + apiResponse.Params.Token + "/commands")
            { 
                var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
                string command = response.Command;
                response.Params.Token = token;
                response.Params.Version = version;
                Debug.WriteLine("COMMAND: "+ command);
                switch (command)
                {
                    case "reboot":
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;

                    case "settings":
                        try
                        {
                            Debug.WriteLine("SETTINGS");
                            response.Save();
                            Debug.WriteLine(apiResponse.Params.Data_ranges.Temp[0]);
                            apiResponse = response;
                            Debug.WriteLine(apiResponse.Params.Data_ranges.Temp[0]);
                            Debug.WriteLine("#############" + response.Params.Data_ranges.Temp[0]);
                           
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("############# CommandProcesser " + ex);
                        }

                        break;


                    case "interval":
                        try
                        {
                            response.Save();
                            apiResponse = response;
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


