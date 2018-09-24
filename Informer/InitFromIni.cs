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

        public static void onInitFromIni()
        {

            GlobalVars.autostart = 60;
            GlobalVars.versions = GlobalVars._manager.GetPrivateString("main", "version");
            GlobalVars.token = GlobalVars._manager.GetPrivateString("main", "token");
            GlobalVars.name = GlobalVars._manager.GetPrivateString("main", "name");

            try
            {
              
                GlobalVars.time_temp_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_temp_min"));
                GlobalVars.time_temp_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_temp_max"));

                GlobalVars.time_mem_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_mem_min"));
                GlobalVars.time_mem_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_mem_max"));

                GlobalVars.time_lost_inet = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_lost_inet"));
                GlobalVars.time_lost_gpu = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_lost_gpu"));

                GlobalVars.time_load_GPU_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_load_GPU_min"));
                GlobalVars.time_load_GPU_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_load_GPU_max"));

                GlobalVars.time_fan_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_fan_min"));
                GlobalVars.time_fan_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_fan_max"));

                GlobalVars.time_clock_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_clock_min"));
                GlobalVars.time_clock_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "time_clock_max"));

                GlobalVars.reboots_temp_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_temp_min"));
                GlobalVars.reboots_temp_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_temp_max"));

                GlobalVars.reboots_fan_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_fan_min"));
                GlobalVars.reboots_fan_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_fan_max"));

                GlobalVars.reboots_load_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_load_min"));
                GlobalVars.reboots_load_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_load_max"));

                GlobalVars.reboots_clock_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_clock_min"));
                GlobalVars.reboots_clock_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_clock_max"));

                GlobalVars.reboots_mem_min = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_mem_min"));
                GlobalVars.reboots_mem_max = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_mem_max"));

                GlobalVars.reboots_lost_gpu = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_lost_gpu"));
                GlobalVars.reboots_lost_inet = Convert.ToBoolean(GlobalVars._manager.GetPrivateString("main", "reboots_lost_inet"));


                GlobalVars.temp_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "temp_min"));
                GlobalVars.temp_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "temp_max"));

                GlobalVars.mem_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_min"));
                GlobalVars.mem_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "mem_max"));

                GlobalVars.load_GPU_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "load_GPU_min"));
                GlobalVars.load_GPU_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "load_GPU_max"));

                GlobalVars.fan_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "fan_min"));
                GlobalVars.fan_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", "fan_max"));

                GlobalVars.clock_min = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", nameof(GlobalVars.clock_min)));
                GlobalVars.clock_max = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", nameof(GlobalVars.clock_max)));


                GlobalVars.autostart = Convert.ToInt32(GlobalVars._manager.GetPrivateString("main", nameof(GlobalVars.autostart)));

             

            }
            catch (Exception e)
            {

               Debug.WriteLine("InitFromIni: " + e.Message);
            }

        }

    }
}
