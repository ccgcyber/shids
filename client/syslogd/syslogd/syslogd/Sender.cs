using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Net;
using MySql.Data.MySqlClient;

namespace syslogd
{
    public class Sender
    {
        public enum PriorityType { Emergency, Alert, Critical, Error, Warning, Notice, Informational, Debug }
        private static UdpClient udp;
        private static ASCIIEncoding ascii = new ASCIIEncoding();
        private static string sysLogIpAddress;

        private Sender()
        {

        }

        public static string SysLogIpAddress
        {
            get { return sysLogIpAddress; }
            set { sysLogIpAddress = value; }
        }

        public static void Send(string ipAddress, string body)
        {
            udp = new UdpClient(ipAddress, Constant.PORT);
            byte[] rawMsg;
            //string[] strParams = { priority.ToString() + ": ", time.ToString("MMM dd HH:mm:ss "), source, body };
            rawMsg = ascii.GetBytes(body);//string.Concat(strParams));
            udp.Send(rawMsg, rawMsg.Length);
            udp.Close();
            udp = null;
        }
    }
}