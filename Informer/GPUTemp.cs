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
                GlobalVars._pc.CPUEnabled = true;
                //_pc.Open();
                GlobalVars._pc.GPUEnabled = true;

                GlobalVars._pc.Open();

                // List<String> gpusList = new List<string>();
                //  GlobalVars.gpuList = new Dictionary<int, List<string>>();
                GlobalVars.gpusList.Clear();
                GlobalVars.gpuList.Clear();
                
                GlobalVars.card = "";
                GlobalVars.temp = "";
                GlobalVars.fan = "";
                GlobalVars.load = "";
                GlobalVars.clock = "";
                GlobalVars.mem = "";
                GlobalVars.counts = 0;
                foreach (var hard in GlobalVars._pc.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
                {

                    hard.Update();


                    if (hard.HardwareType == HardwareType.GpuAti || hard.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                    {

                        GlobalVars.counts = GlobalVars.counts + 1;
                        GlobalVars.card += hard.Name + ",";
                        GlobalVars.gpusList.Add(hard.Name);

                        foreach (var sensor in hard.Sensors)//ИДЕМ по сенсорам
                        {


                            if (sensor.SensorType == SensorType.Clock)
                            {//ЧАСТОТЫ



                                if (sensor.Name == "GPU Core")//ЯДРО
                                {

                                    GlobalVars.clock += sensor.Value.GetValueOrDefault() + ";";
                                    GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));


                                }



                                if (hard.HardwareType == HardwareType.GpuAti)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }

                                }
                                else if (hard.HardwareType == HardwareType.GpuNvidia)
                                {
                                    if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                                    {
                                        GlobalVars.mem += sensor.Value.GetValueOrDefault() + ";";
                                        GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));

                                    }
                                }
                                else
                                {

                                }



                            }
                            else if (sensor.SensorType == SensorType.Temperature)//Температура
                            {


                                GlobalVars.temp += sensor.Value.GetValueOrDefault() + ",";
                                GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));

                            }
                            else if (sensor.SensorType == SensorType.Control)// FAN
                            {

                                GlobalVars.fan += sensor.Value.GetValueOrDefault() + ",";
                                GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));


                            }


                            else if (sensor.SensorType == SensorType.Load)//LOAD
                            {
                                if (sensor.Name == "GPU Core")
                                {
                                    GlobalVars.load += sensor.Value.GetValueOrDefault() + ",";
                                    GlobalVars.gpusList.Add(Convert.ToString(sensor.Value.GetValueOrDefault()));



                                }
                                else if (sensor.Name == "GPU Memory Controller")
                                {

                                }

                            }

                         //   GlobalVars.gpuList.Add(GlobalVars.counts, GlobalVars.gpusList);
                        }


                    }

                }



                //bool status = await GPUSensorsStatus.GetGPUStatus(status);

            }
            catch(Exception e)
            {
                Debug.WriteLine(e);

            }

            /*
            return await Task.Run(() =>
            {
                bool result = true;
                
                for (int i = 1; i <= x; i++)
                {
                    result *= i;
                }
                
                return result;
            
            });

        */
        /*
            int num = 5;

            int result = await FactorialAsync(num);

    */
        }


        static Task<int> FactorialAsync(int x)
        {
            int result = 1;

            return Task.Run(() =>
            {
              //  for (int i = 1; i <= x; i++)
              //  {
              //      result *= i;
              //  }
                return result;
            });
        }

    }
}
        
    
