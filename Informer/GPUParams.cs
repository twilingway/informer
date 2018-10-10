using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Informer
{
    class GPUParams
    {
        Computer PC;
        public string[] Name { get; private set; }
        public int[] Temperature { get; private set; }
        public int[] FanSpeed { get; private set; }
        public int[] Clock { get; private set; }
        public int[] Memory { get; private set; }
        public int[] Load { get; private set; }
        public int CountGPU { get; private set; } = 0;

        public GPUParams(Computer pc)
        {
            PC = pc;
            foreach (var hardware in PC.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
            {
                hardware.Update();
                if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                {
                    CountGPU++;
                }
            }

            Name = new string[CountGPU];
            Temperature = new int[CountGPU];
            FanSpeed = new int[CountGPU];
            Clock = new int[CountGPU];
            Memory = new int[CountGPU];
            Load = new int[CountGPU];
        }

        public void UpdateParams(SensorForDanger[] dangers)
        {
            foreach (var hardware in PC.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
            {
                hardware.Update();
                if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                {
                    foreach (var sensor in hardware.Sensors)//ИДЕМ по сенсорам
                    {
                        foreach (var danger in dangers)
                        {
                            if (danger.IsTargetSensor(sensor.Name, sensor.SensorType))
                            {
                                danger.AddSensor(Convert.ToInt32(sensor.Value.GetValueOrDefault()));
                            }
                        }
                    }
                }
            }

            foreach (var danger in dangers)
            {
                danger.UpdateValue();
            }
        }

        public void SetParams()
        {
            int step = 0;
            foreach (var hardware in PC.Hardware)// ВЫБИРАЕМ ЖЕЛЕЗО
            {
                hardware.Update();

                if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)//КАРТЫ
                {
                    Name[step] = hardware.Name;

                    foreach (var sensor in hardware.Sensors)//ИДЕМ по сенсорам
                    {
                        if (sensor.SensorType == SensorType.Clock)
                        {//ЧАСТОТЫ
                            if (sensor.Name == "GPU Core")//ЯДРО
                            {
                                Clock[step] = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                            }

                            if (sensor.Name == "GPU Memory")//ПАМЯТЬ
                            {
                                Memory[step] = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                            }
                        }

                        if (sensor.SensorType == SensorType.Temperature)//Температура
                        {
                            if (sensor.Name == "GPU Core")//ЯДРО
                            {
                                Temperature[step] = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                            }
                            //_temperature[i] = sensor.Value.GetValueOrDefault();
                        }
                        if (sensor.SensorType == SensorType.Load)//LOAD
                        {
                            if (sensor.Name == "GPU Core")
                            {
                                Load[step] = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                            }
                        }
                        if (sensor.SensorType == SensorType.Control)// FAN
                        {
                            FanSpeed[step] = Convert.ToInt32(sensor.Value.GetValueOrDefault());
                        }
                    }
                }
                step++;
            }
        }

        public void GetParams()
        {
            for (int i = 0; i < Name.Length; i++)
            {
                Debug.WriteLine(Name[i] + " " + Load[i] + " " + Temperature[i] + " " + FanSpeed[i] + " " + Clock[i] + " " + Memory[i]);
            }
        }

        public static void GetGPU(GlobalVars globalVars, Computer PC)
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
        
    
