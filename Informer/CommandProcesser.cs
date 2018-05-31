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
      
        public static void onMessage(string payload, string topic)
        {

            if (topic == "devices/" + GlobalVars.token + "/commands")
            {

                // Debug.WriteLine(message);
                var response = JsonConvert.DeserializeObject<ApiResponse>(payload);
                string command = response.command;
                switch (command)
                {

                    case "reboot":

                        //Message("Informer Reboot from Allminer.ru!");
                        Process psiwer;
                        psiwer = Process.Start("cmd.exe", "/c shutdown /r /f /t 0");
                        psiwer.Close();
                        break;

                    case "settings":
                        try
                        {
                            //write timers to ini
                            GlobalVars.time_temp_min = response.Params.timers.temp_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_temp_min), Convert.ToString(response.Params.timers.temp_min));

                            GlobalVars.time_temp_max = response.Params.timers.temp_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_temp_max), Convert.ToString(response.Params.timers.temp_max));

                            GlobalVars.time_fan_min = response.Params.timers.fan_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_fan_min), Convert.ToString(response.Params.timers.fan_min));

                            GlobalVars.time_fan_max = response.Params.timers.fan_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_fan_max), Convert.ToString(response.Params.timers.fan_max));

                            GlobalVars.time_load_GPU_min = response.Params.timers.load_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_load_GPU_min), Convert.ToString(response.Params.timers.load_min));

                            GlobalVars.time_load_GPU_max = response.Params.timers.load_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_load_GPU_max), Convert.ToString(response.Params.timers.load_max));

                            GlobalVars.time_clock_min = response.Params.timers.clock_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_clock_min), Convert.ToString(response.Params.timers.clock_min));


                            GlobalVars.time_clock_max = response.Params.timers.clock_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_clock_max), Convert.ToString(response.Params.timers.clock_max));

                            GlobalVars.time_mem_min = response.Params.timers.mem_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_mem_min), Convert.ToString(response.Params.timers.mem_min));

                            GlobalVars.time_mem_max = response.Params.timers.mem_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_mem_max), Convert.ToString(response.Params.timers.mem_max));

                            GlobalVars.time_lost_gpu = response.Params.timers.lost_gpu;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_lost_gpu), Convert.ToString(response.Params.timers.lost_gpu));

                            GlobalVars.time_lost_inet = response.Params.timers.lost_inet;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_lost_inet), Convert.ToString(response.Params.timers.lost_inet));

                            //write reboots flag to ini

                            GlobalVars.reboots_temp_min = response.Params.reboots.temp_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_temp_min), Convert.ToString(response.Params.reboots.temp_min));

                            GlobalVars.reboots_temp_max = response.Params.reboots.temp_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_temp_max), Convert.ToString(response.Params.reboots.temp_max));

                            GlobalVars.reboots_fan_min = response.Params.reboots.fan_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_fan_min), Convert.ToString(response.Params.reboots.fan_min));

                            GlobalVars.reboots_fan_max = response.Params.reboots.fan_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_fan_max), Convert.ToString(response.Params.reboots.fan_max));

                            GlobalVars.reboots_load_min = response.Params.reboots.load_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_load_min), Convert.ToString(response.Params.reboots.load_min));

                            GlobalVars.reboots_load_max = response.Params.reboots.load_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_load_max), Convert.ToString(response.Params.reboots.load_max));

                            GlobalVars.reboots_clock_min = response.Params.reboots.clock_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_clock_min), Convert.ToString(response.Params.reboots.clock_min));

                            GlobalVars.reboots_clock_max = response.Params.reboots.clock_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_clock_max), Convert.ToString(response.Params.reboots.clock_max));

                            GlobalVars.reboots_mem_min = response.Params.reboots.mem_min;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_mem_min), Convert.ToString(response.Params.reboots.mem_min));

                            GlobalVars.reboots_mem_max = response.Params.reboots.mem_max;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_mem_max), Convert.ToString(response.Params.reboots.mem_max));

                            GlobalVars.reboots_lost_gpu = response.Params.reboots.lost_gpu;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_lost_gpu), Convert.ToString(response.Params.reboots.lost_gpu));

                            GlobalVars.reboots_lost_inet = response.Params.reboots.lost_inet;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.reboots_lost_inet), Convert.ToString(response.Params.reboots.lost_inet));

                            //write data_ranges to ini

                            GlobalVars.temp_min = response.Params.data_ranges.Temp[0];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.temp_min), Convert.ToString(response.Params.data_ranges.Temp[0]));

                            GlobalVars.temp_max = response.Params.data_ranges.Temp[1];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.temp_max), Convert.ToString(response.Params.data_ranges.Temp[1]));

                            GlobalVars.mem_min = response.Params.data_ranges.Mem[0];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.mem_min), Convert.ToString(response.Params.data_ranges.Mem[0]));

                            GlobalVars.mem_max = response.Params.data_ranges.Mem[1];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.mem_max), Convert.ToString(response.Params.data_ranges.Mem[1]));

                            GlobalVars.load_GPU_min = response.Params.data_ranges.Load[0];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.load_GPU_min), Convert.ToString(response.Params.data_ranges.Load[0]));

                            GlobalVars.load_GPU_max = response.Params.data_ranges.Load[1];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.load_GPU_max), Convert.ToString(response.Params.data_ranges.Load[1]));


                            GlobalVars.fan_min = response.Params.data_ranges.Fan[0];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.fan_min), Convert.ToString(response.Params.data_ranges.Fan[0]));

                            GlobalVars.fan_max = response.Params.data_ranges.Fan[1];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.fan_max), Convert.ToString(response.Params.data_ranges.Fan[1]));


                            GlobalVars.clock_min = response.Params.data_ranges.Clock[0];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.clock_min), Convert.ToString(response.Params.data_ranges.Clock[0]));

                            GlobalVars.clock_max = response.Params.data_ranges.Clock[1];
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.clock_max), Convert.ToString(response.Params.data_ranges.Clock[1]));

                            GlobalVars.name = response.Params.name;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.name), Convert.ToString(response.Params.name));

                            //tbRigName.Text = GlobalVars.name;

                            GlobalVars.time_start = response.Params.interval;
                            GlobalVars._manager.WritePrivateString("main", nameof(GlobalVars.time_start), Convert.ToString(response.Params.interval));


                            //SendDataTimer.Interval = response.Params.interval * 1000;



                            //SendDataTimer.Interval = 2 * 1000;


                        }
                        catch (Exception ex)
                        {

                            //  _error.writeLogLine("Receive:" + ex.Message, "error_settings");

                        }

                        break;
                
                }


            }


        }



    }




}

