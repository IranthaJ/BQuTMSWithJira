using System;
using System.Configuration;

namespace BQuTMSWithJira
{
    class CheckServerConnection
    {
        //check where stablish net connection at application startup
        public bool CheckConnection()
        {
            //  return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            try
            {
                //////////////check connction
                //  System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient(ConfigurationManager.AppSettings["sip"], Convert.ToInt32(ConfigurationManager.AppSettings["sport"]));
                System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient(UserConfig.configlist[3], Convert.ToInt32(UserConfig.configlist[4]));
                clnt.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
