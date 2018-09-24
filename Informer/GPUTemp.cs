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
             //public static async Task GetGPU()
        {
            try
            {
                //GlobalVars._pc = new Computer();
                /*
                if (GlobalVars.temp0 == true) {
                    GlobalVars._pc.Close();
                    GlobalVars._pc = null;
                    GlobalVars._pc = new Computer();
                    GlobalVars._pc.CPUEnabled = true;
                    GlobalVars._pc.GPUEnabled = true;
                    GlobalVars._pc.Open();

                }
                */
                // List<String> gpusList = new List<string>();
                //  GlobalVars.gpuList = new Dictionary<int, List<string>>();
                GlobalVars.gpuList.Clear();
                //GlobalVars.gpuParams.Clear();
                
                GlobalVars.card = "";
                GlobalVars.temp = "";
                GlobalVars.fan = "";
                GlobalVars.load = "";
                GlobalVars.clock = "";
                GlobalVars.mem = "";
                int count = 0;
               // for (int i = 0; i < 22; i++)
              //  {
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
                                    //GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));


                                    }



                                    if (hard.HardwareType == HardwareType.GpuAti)
                                    {
                                        if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                        {
                                            GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        //     GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));
                                        GlobalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }

                                    }
                                    else if (hard.HardwareType == HardwareType.GpuNvidia)
                                    {
                                        if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                        {
                                     //   Debug.WriteLine(sensor.Value.GetValueOrDefault());
                                       
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        //     GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));
                                        GlobalVars.gpuList[count].Add("memory", Convert.ToString(sensor.Value.GetValueOrDefault()));

                                        }
                                    }
                                   


                                }
                                else if (sensor.SensorType == SensorType.Temperature)//Температура
                                {

                                
                                    GlobalVars.temp += sensor.Value.GetValueOrDefault() + ",";
                                //     GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));
                                GlobalVars.gpuList[count].Add("temp", Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }

                                else if (sensor.SensorType == SensorType.Load)//LOAD
                                {
                                    if (sensor.Name == "GPU Core")
                                    {

                                    
                                    GlobalVars.load += sensor.Value.GetValueOrDefault() + ",";
                                    // GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));
                                    GlobalVars.gpuList[count].Add("load", Convert.ToString(sensor.Value.GetValueOrDefault()));



                                    }

                                }

                                else if (sensor.SensorType == SensorType.Control)// FAN
                                {

                               
                                GlobalVars.fan += sensor.Value.GetValueOrDefault() + ",";
                                //    GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));
                                GlobalVars.gpuList[count].Add("fan", Convert.ToString(sensor.Value.GetValueOrDefault()));


                            }






                        
                            }


                        count = count + 1;
                        GlobalVars.counts = count;
                        // GlobalVars.gpuList.Add(GlobalVars.gpuParams);
                        // GlobalVars.gpuParams.Clear();
                    }

                        


                    }



                //for end
                // }

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
        
    
