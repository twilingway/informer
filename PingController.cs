using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Informer
{
    public class PingController
    {
        public event EventHandler<PingCompletedEventArgs> PingCompleted = delegate { };

        public void ExecuteAsyncPing(IPAddress address)
        {
            AutoResetEvent waiter = new AutoResetEvent(false);
            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(ping_PingCompleted);
            ping.SendAsync(address, waiter);
            waiter.WaitOne();
        }

        void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingCompleted(this, e);
            ((AutoResetEvent)e.UserState).Set();
        }
    }
}
