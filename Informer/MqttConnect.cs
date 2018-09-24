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


                    GlobalVars.client = GlobalVars.factory.CreateMqttClient();


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
                       try
                       {
                           await GlobalVars.client.SubscribeAsync(new TopicFilterBuilder().WithTopic("devices/" + GlobalVars.token + "/commands").Build());
                       }
                       catch (Exception ex)
                       {
                           Debug.WriteLine("client.SubscribeAsync: " + ex);
                       }
                   };


                    GlobalVars.client.Disconnected += async (s, e) =>
                    {
                        Debug.WriteLine("### DISCONNECTED FROM SERVER ###");
                        GlobalVars.mqttIsConnect = false;
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    };



                    try
                    {
                        await GlobalVars.client.ConnectAsync(options);
                        GlobalVars.mqttIsConnect = true;
                        GlobalVars.firsrun = false;
                        GlobalVars._manager.WritePrivateString("main", "token", GlobalVars.token);
                        var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + GlobalVars.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                        await GlobalVars.client.PublishAsync(message);


                    }
                    catch (MQTTnet.Adapter.MqttConnectingFailedException ex )
                    {
                        GlobalVars.mqttIsConnect = false;
                        GlobalVars.firsrun = false;
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                        GlobalVars.mqttIsConnect = false;
                        GlobalVars.firsrun = false;
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("### EXCEPTION ###" + exception);
                }


            }
                     
        }
       

    }
}
