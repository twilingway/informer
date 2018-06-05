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
            //if (tokenMqtt)
           // {
            //    GlobalVars.mqttClient.DisconnectAsync();

           // }
          //  else
          //  {


                var options = new MqttClientOptionsBuilder()

                        .WithClientId(GlobalVars.token)
                        .WithTcpServer("allminer.ru", 1883)
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
                        .WithCredentials(GlobalVars.token, GlobalVars.token)
                        //.WithTls()
                        .WithCleanSession(true)
                        .Build();

               // var factory = new MqttFactory();

                GlobalVars.mqttClient = GlobalVars.factory.CreateMqttClient();



             //   CancellationTokenSource cts = new CancellationTokenSource();
            /*
                try
                {
                    Task t1 = FactorialAsync(cts.Token);
                    Task t2 = Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        cts.Cancel(); // отмена асинхронной операции
                });
                    await Task.WhenAll(t1, t2);
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    cts.Dispose();
                }

            */
                /*
                return Task.Run(() =>
                {
                    tokenMqtt.ThrowIfCancellationRequested();
                }, tokenMqtt);

                */
                // tokenMqtt.ThrowIfCancellationRequested();
                /*
                if (tokenMqtt.IsCancellationRequested)
                {
                    Debug.WriteLine("Операция прервана токеном");
                    return;
                }
                */

                try
                {
                    // Create TCP based options using the builder.



                    GlobalVars.mqttClient.ApplicationMessageReceived += (s, e) =>
                    {
                        Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                        Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                        Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                        Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                        CommandProcesser.onMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload), e.ApplicationMessage.Topic);

                    };

                    GlobalVars.mqttClient.Connected += async (s, e) =>
                    {

                        await GlobalVars.mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("devices/" + GlobalVars.token + "/commands").Build());
                    };


                    GlobalVars.mqttClient.Disconnected += async (s, e) =>
                    {
                        Debug.WriteLine("### DISCONNECTED FROM SERVER ###");

                        await Task.Delay(TimeSpan.FromSeconds(5));

                        //if (GlobalVars.mqttIsConnect)
                        // {
                        try
                        {
                            await GlobalVars.mqttClient.ConnectAsync(options);


                            var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + GlobalVars.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                            await GlobalVars.mqttClient.PublishAsync(message);

                        }
                        catch
                        {
                            Debug.WriteLine("### RECONNECTING FAILED ###");
                        }
                        //}
                    };


                    try
                    {
                        await GlobalVars.mqttClient.ConnectAsync(options);

                        var message = new MqttApplicationMessageBuilder()
                                   .WithTopic("devices/" + GlobalVars.token + "/init")
                                   .WithPayload("1")
                                   .WithAtMostOnceQoS()
                                   .WithRetainFlag()
                                   .Build();

                        await GlobalVars.mqttClient.PublishAsync(message);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                    }






                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }



           // }
        }

        static Task FactorialAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
               
             token.ThrowIfCancellationRequested();
                    
                
            }, token);
        }

    }
}
