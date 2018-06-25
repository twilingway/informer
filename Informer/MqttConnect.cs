using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Informer
{
    public static class MqttConnect
    {


       
        public static async Task RunAsync()
        {
            Debug.WriteLine(GlobalVars.mqttIsConnect);
            if (GlobalVars.mqttIsConnect == false)
            {

                try
                {

                    var options = new MqttClientOptionsBuilder()

                            .WithClientId(GlobalVars.token)
                            .WithTcpServer("allminer.ru", 1883)
                            .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
                            .WithCredentials(GlobalVars.token, GlobalVars.token)
                            //.WithTls()
                            .WithCleanSession(true)
                            .Build();

                    // var factory = new MqttFactory();
                    //var factory = new MqttFactory();

                    GlobalVars.client = GlobalVars.factory.CreateMqttClient();


                    //client = factory.CreateMqttClient();

                    // Create TCP based options using the builder.



                    GlobalVars.client.ApplicationMessageReceived += (s, e) =>
                        {
                            Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                            Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                            Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                            Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                            Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                            CommandProcesser.onMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload), e.ApplicationMessage.Topic);

                        };

                    GlobalVars.client.Connected += async (s, e) =>
                   {
                      
                           await GlobalVars.client.SubscribeAsync(new TopicFilterBuilder().WithTopic("devices/" + GlobalVars.token + "/commands").Build());
                   };


                    GlobalVars.client.Disconnected += async (s, e) =>
                    {
                        Debug.WriteLine("### DISCONNECTED FROM SERVER ###");
                        GlobalVars.mqttIsConnect = false;
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        
                        /*

                        try
                        {
                            await client.ConnectAsync(options);
                            GlobalVars.mqttIsConnect = true;

                            var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + GlobalVars.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                            await client.PublishAsync(message);

                        }
                        catch
                        {
                            Debug.WriteLine("### RECONNECTING FAILED ###");
                        }
                        */
                    };



                    try
                    {
                       // GlobalVars.mqttIsConnect = false;
                        await GlobalVars.client.ConnectAsync(options);
                        GlobalVars.mqttIsConnect = true;
                        MainForm.Message("Connected");

                        var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + GlobalVars.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                        await GlobalVars.client.PublishAsync(message);


                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                        GlobalVars.mqttIsConnect = false;
                    }




                    /*
                    while (true)
                    {
                       // Console.ReadLine();

                       // await client.SubscribeAsync(new TopicFilter("test", MqttQualityOfServiceLevel.AtMostOnce));
                       // await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("devices/" + GlobalVars.token + "/commands").Build());
                        var message = new MqttApplicationMessageBuilder()
                                      .WithTopic("devices/" + GlobalVars.token + "/init")
                                      .WithPayload("1")
                                      .WithAtMostOnceQoS()
                                      .WithRetainFlag()
                                      .Build();

                        await client.PublishAsync(message);
                    }
                    */
                    /*
                    if (client.IsConnected)
                    {
                        GlobalVars.mqttIsConnect = true;
                    }
                    else if (!client.IsConnected)
                    {
                        GlobalVars.mqttIsConnect = false;
                    }
                    */
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("### EXCEPTION ###" + exception);
                }


            }
            else if (GlobalVars.mqttIsConnect == true)
            {
               
            }
          
        }
       

    }
}
