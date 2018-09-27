using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer
{
    public static class InitFromIni
    {
        public static void onInitFromIni(GlobalVars globalVars)
        {

            globalVars.autostart = 60;
            globalVars.versions = globalVars._manager.GetPrivateString("main", "version");
            globalVars.token = globalVars._manager.GetPrivateString("main", "token");
            globalVars.name = globalVars._manager.GetPrivateString("main", "name");

            try
            {
              
                globalVars.Timer.temp_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_temp_min"));
                globalVars.Timer.temp_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_temp_max"));

                globalVars.Timer.mem_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_mem_min"));
                globalVars.Timer.mem_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_mem_max"));

                globalVars.Timer.lost_inet = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_lost_inet"));
                globalVars.Timer.lost_gpu = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_lost_gpu"));

                globalVars.Timer.load_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_load_GPU_min"));
                globalVars.Timer.load_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_load_GPU_max"));

                globalVars.Timer.fan_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_fan_min"));
                globalVars.Timer.fan_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_fan_max"));

                globalVars.Timer.clock_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_clock_min"));
                globalVars.Timer.clock_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_clock_max"));

                globalVars.Reboots.temp_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_temp_min"));
                globalVars.Reboots.temp_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_temp_max"));

                globalVars.Reboots.fan_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_fan_min"));
                globalVars.Reboots.fan_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_fan_max"));

                globalVars.Reboots.load_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_load_min"));
                globalVars.Reboots.load_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_load_max"));

                globalVars.Reboots.clock_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_clock_min"));
                globalVars.Reboots.clock_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_clock_max"));

                globalVars.Reboots.mem_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_mem_min"));
                globalVars.Reboots.mem_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_mem_max"));

                globalVars.Reboots.lost_gpu = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_lost_gpu"));
                globalVars.Reboots.lost_inet = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_lost_inet"));


                globalVars.temp_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "temp_min"));
                globalVars.temp_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "temp_max"));

                globalVars.mem_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "mem_min"));
                globalVars.mem_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "mem_max"));

                globalVars.load_GPU_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "load_GPU_min"));
                globalVars.load_GPU_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "load_GPU_max"));

                globalVars.fan_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "fan_min"));
                globalVars.fan_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "fan_max"));

                globalVars.clock_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", nameof(globalVars.clock_min)));
                globalVars.clock_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", nameof(globalVars.clock_max)));


                globalVars.autostart = Convert.ToInt32(globalVars._manager.GetPrivateString("main", nameof(globalVars.autostart)));

             

            }
            catch (Exception e)
            {

               Debug.WriteLine("InitFromIni: " + e.Message);
            }

        }

    }
}
