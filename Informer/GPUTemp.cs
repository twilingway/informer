using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Informer
{

    public partial class GPUTemp
    {
        public static void GetGPU(GlobalVars globalVars,Computer PC)
        {
            try
            {
                globalVars.gpuList.Clear();
                globalVars.card = "";
                globalVars.temp = "";
                globalVars.fan = "";
                globalVars.load = "";
                globalVars.clock = "";
                globalVars.mem = "";
                int count = 0;

                foreach (var hard in PC.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
                {

                    hard.Update();


                    if (hard.HardwareType == HardwareType.GpuAti || hard.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                    {
                        globalVars.card += hard.Name + ",";
                        globalVars.gpuList.Add(new Dictionary<string, string>());
                        globalVars.gpuList[count].Add("name", hard.Name);

                        foreach (var sensor in hard.Sensors)//ИДЕМ по сенсорам
                        {
                            if (sensor.SensorType == SensorType.Clock)
                            {//ЧАСТОТЫ

                                if (sensor.Name == "GPU Core")//ЯДРО
                                {
                                    globalVars.gpuList[count].Add("core", Convert.ToString(sensor.Value.GetValueOrDefault()));
                                    globalVars.clock += sensor.Value.GetValueOrDefault() + ";";
                                }


                                if (hard.HardwareType == HardwareType.GpuAti)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        globalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        globalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));
                                    }
                                }
                                else if (hard.HardwareType == HardwareType.GpuNvidia)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        globalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        globalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }
                                }



                            }
                            else if (sensor.SensorType == SensorType.Temperature)//Температура
                            {


                                globalVars.temp += sensor.Value.GetValueOrDefault() + ",";
                                globalVars.gpuList[count].Add("temp", Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }

                            else if (sensor.SensorType == SensorType.Load)//LOAD
                            {
                                if (sensor.Name == "GPU Core")
                                {
                                    globalVars.load += sensor.Value.GetValueOrDefault() + ",";
                                    globalVars.gpuList[count].Add("load", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                }
                            }

                            else if (sensor.SensorType == SensorType.Control)// FAN
                            {
                                globalVars.fan += sensor.Value.GetValueOrDefault() + ",";
                                globalVars.gpuList[count].Add("fan", Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }

                        }


                        count = count + 1;
                        globalVars.counts = count;
                    }
                }

                if (globalVars.count_GPU == 0)
                {
                    globalVars.count_GPU = globalVars.counts;
                }


            }
            catch (Exception e)
            {

                Debug.WriteLine(e);

            }
        }
    }
}
        
    
