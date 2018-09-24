using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer
{

    public partial class GPUTemp
    {
        public static void GetGPU()
        {
            try
            {
                GlobalVars.gpuList.Clear();
                GlobalVars.card = "";
                GlobalVars.temp = "";
                GlobalVars.fan = "";
                GlobalVars.load = "";
                GlobalVars.clock = "";
                GlobalVars.mem = "";
                int count = 0;
             
                    foreach (var hard in GlobalVars._pc.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
                    {

                        hard.Update();


                        if (hard.HardwareType == HardwareType.GpuAti || hard.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                        {

                        
                        GlobalVars.card += hard.Name + ",";
                        GlobalVars.gpuList.Add(new Dictionary<string, string>());
                        GlobalVars.gpuList[count].Add("name", hard.Name);
                        
                        foreach (var sensor in hard.Sensors)//ИДЕМ по сенсорам
                            {


                                if (sensor.SensorType == SensorType.Clock)
                                {//ЧАСТОТЫ


                                    if (sensor.Name == "GPU Core")//ЯДРО
                                    {
                                    GlobalVars.gpuList[count].Add("core", Convert.ToString(sensor.Value.GetValueOrDefault()));
                                    GlobalVars.clock += sensor.Value.GetValueOrDefault() + ";";
                                    

                                    }



                                    if (hard.HardwareType == HardwareType.GpuAti)
                                    {
                                        if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                        {
                                            GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                       GlobalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }

                                    }
                                    else if (hard.HardwareType == HardwareType.GpuNvidia)
                                    {
                                        if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                        {
                                     
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        GlobalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                        }
                                    }
                                   


                                }
                                else if (sensor.SensorType == SensorType.Temperature)//Температура
                                {

                                
                                    GlobalVars.temp += sensor.Value.GetValueOrDefault() + ",";
                                GlobalVars.gpuList[count].Add("temp", Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }

                                else if (sensor.SensorType == SensorType.Load)//LOAD
                                {
                                    if (sensor.Name == "GPU Core")
                                    {
                                    GlobalVars.load += sensor.Value.GetValueOrDefault() + ",";
                                    GlobalVars.gpuList[count].Add("load", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }
                                }

                                else if (sensor.SensorType == SensorType.Control)// FAN
                                {
                                GlobalVars.fan += sensor.Value.GetValueOrDefault() + ",";
                                 GlobalVars.gpuList[count].Add("fan", Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }

                            }


                        count = count + 1;
                        GlobalVars.counts = count;
                    }
                    }

                if (GlobalVars.count_GPU == 0)
                {
                    GlobalVars.count_GPU = GlobalVars.counts;
                }


            }
            catch(Exception e)
            {
                
                Debug.WriteLine(e);

            }
        }
    }
}
        
    
