using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace Informer
{
    public class MqttConnect
    {
        public async Task RunAsync(GlobalVars globalVars,ApiResponse apiResponse, CommandProcesser commandProcesser)
        {
            if (globalVars.mqttIsConnect == false)
            {
                try
                {
                    var options = new MqttClientOptionsBuilder()

                            .WithClientId(apiResponse.Params.Token)
                            .WithTcpServer("allminer.ru", 1883)
                            .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
                            .WithCredentials(apiResponse.Params.Token, apiResponse.Params.Token)
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

                            commandProcesser.onMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload), e.ApplicationMessage.Topic);
                         };

                    globalVars.client.Connected += async (s, e) =>
                   {
                       try
                       {
                           await globalVars.client.SubscribeAsync(new TopicFilterBuilder().
                           WithTopic("devices/" + apiResponse.Params.Token + "/commands").
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
                                   .WithTopic("devices/" + apiResponse.Params.Token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                        await globalVars.client.PublishAsync(message);
                    }
                    catch (MQTTnet.Adapter.MqttConnectingFailedException ex )
                    {
                        Debug.WriteLine("### MqttConnectingFailedException ###" + Environment.NewLine + ex);
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
