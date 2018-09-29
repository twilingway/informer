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
        public static async Task RunAsync(GlobalVars globalVars,ApiResponse settings)
        {
            if (globalVars.mqttIsConnect == false)
            {
                try
                {
                    var options = new MqttClientOptionsBuilder()

                            .WithClientId(settings.Params.token)
                            .WithTcpServer("allminer.ru", 1883)
                            .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
                            .WithCredentials(settings.Params.token, settings.Params.token)
                            //.WithTls()
                            .WithCleanSession(true)
                            .Build();


                    globalVars.client = globalVars.factory.CreateMqttClient();


                    // Create TCP based options using the builder.
                    globalVars.client.ApplicationMessageReceived += (s, e) =>
                        {
                            Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                            Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                            Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                            Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                            Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                            CommandProcesser.onMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload), e.ApplicationMessage.Topic,globalVars,settings);

                        };

                    globalVars.client.Connected += async (s, e) =>
                   {
                       try
                       {
                           await globalVars.client.SubscribeAsync(new TopicFilterBuilder().
                           WithTopic("devices/" + settings.Params.token + "/commands").
                           Build());
                       }
                       catch (Exception ex)
                       {
                           Debug.WriteLine("client.SubscribeAsync: " + ex);
                       }
                   };


                    globalVars.client.Disconnected += async (s, e) =>
                    {
                        Debug.WriteLine("### DISCONNECTED FROM SERVER ###");
                        globalVars.mqttIsConnect = false;
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    };



                    try
                    {
                        await globalVars.client.ConnectAsync(options);
                        globalVars.mqttIsConnect = true;
                        globalVars.firsrun = false;
                       // globalVars._manager.WritePrivateString("main", "token", globalVars.token);
                        var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + settings.Params.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                        await globalVars.client.PublishAsync(message);


                    }
                    catch (MQTTnet.Adapter.MqttConnectingFailedException ex )
                    {
                        globalVars.mqttIsConnect = false;
                        globalVars.firsrun = false;
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                        globalVars.mqttIsConnect = false;
                        globalVars.firsrun = false;
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
