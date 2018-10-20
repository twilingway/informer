using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informer.Sensors
{
    public class RebootTriggerAction : ITirggerAction
    {
        public string _msg;
        public string _batFileName;
        public string _token;
        public string _rig;
        public string _host;

        public RebootTriggerAction(string msg, string batFileName, string token, string rig, string host)
        {
            _msg = msg;
            _batFileName = batFileName;
            _token = token;
            _rig = rig;
            _host = host;
        }

        public void Action()
        {
            try
            {
                using (Http _http = new Http())
                {

                    _http.GetContent(
                       _host +
                       "/api.php?token=" + _token +
                       "&event=" + "reboot" +
                       "&reason=" + _rig + " " + _msg
                       );
                }
                Process.Start(_batFileName);
            }
            catch (Exception) { }
            finally{

            }
        }
    }
}
