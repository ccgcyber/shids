using System;
using System.Collections.Generic;
using System.Text;

namespace netTest
{
    class Program
    {
        static void Main(string[] args)
        {
            shidsWebServer.ReportIncident x = new netTest.shidsWebServer.ReportIncident();
            x.CallReportIncident("2013-05-30 11:11:11", "192.168.198.10", "1", "desc." + DateTime.Now.ToLongDateString());  
        }
    }
}
