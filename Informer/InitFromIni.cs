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
              
                globalVars.time_temp_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_temp_min"));
                globalVars.time_temp_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_temp_max"));

                globalVars.time_mem_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_mem_min"));
                globalVars.time_mem_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_mem_max"));

                globalVars.time_lost_inet = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_lost_inet"));
                globalVars.time_lost_gpu = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_lost_gpu"));

                globalVars.time_load_GPU_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_load_GPU_min"));
                globalVars.time_load_GPU_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_load_GPU_max"));

                globalVars.time_fan_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_fan_min"));
                globalVars.time_fan_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_fan_max"));

                globalVars.time_clock_min = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_clock_min"));
                globalVars.time_clock_max = Convert.ToInt32(globalVars._manager.GetPrivateString("main", "time_clock_max"));

                globalVars.reboots_temp_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_temp_min"));
                globalVars.reboots_temp_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_temp_max"));

                globalVars.reboots_fan_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_fan_min"));
                globalVars.reboots_fan_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_fan_max"));

                globalVars.reboots_load_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_load_min"));
                globalVars.reboots_load_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_load_max"));

                globalVars.reboots_clock_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_clock_min"));
                globalVars.reboots_clock_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_clock_max"));

                globalVars.reboots_mem_min = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_mem_min"));
                globalVars.reboots_mem_max = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_mem_max"));

                globalVars.reboots_lost_gpu = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_lost_gpu"));
                globalVars.reboots_lost_inet = Convert.ToBoolean(globalVars._manager.GetPrivateString("main", "reboots_lost_inet"));


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
