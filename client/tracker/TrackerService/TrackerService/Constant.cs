using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Collections;
using System.Management;
using System.Security.Cryptography;
using System.IO;

namespace TrackerService
{
    class Constant
    {
        public const string SERVICE_DESC = "S HIDS Client Tracker Service";

        public const string EVENT_LOG_NAME = "S HIDS Client Tracker";
        public const string EVENT_LOG_TYPE = "Application";

        public const string BB_STATUS_AGENT_RECEIVED = "2";
        public const string BB_STATUS_AGENT_SENT_RESULT = "3";
        public const string BB_STATUS_AGENT_ERROR = "4";


        public static string readUrl()
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(@"s_hids.url", !System.IO.File.Exists(@"s_hids.url")))
            {
                return file.ReadLine();
            }
        }

        public static string performWebReq(string url)
        {
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }


        public static string getSHA1(string filePath)
        {
            SHA1Managed sha = new SHA1Managed();
            byte[] checksum = sha.ComputeHash(ReadFile(filePath));
            return BitConverter.ToString(checksum).Replace("-", string.Empty);

        }
        private static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    int length = (int)fileStream.Length;
                    buffer = new byte[length];
                    int count;
                    int sum = 0;


                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;
                }
                finally
                {
                    fileStream.Close();
                }

                return buffer;
            }
        }
        public static string getChildProcess(int processID)
        {
            string childProcess = string.Empty;
            String myQuery = string.Format("select * from win32_process where ParentProcessId={0}", processID);
            ManagementScope mScope = new ManagementScope(string.Format(@"\\{0}\root\cimv2", "localhost"), null);
            mScope.Connect();
            if (mScope.IsConnected)
            {
                ObjectQuery objQuery = new ObjectQuery(myQuery);
                using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(mScope, objQuery))
                {
                    using (ManagementObjectCollection result = objSearcher.Get())
                    {
                        childProcess += string.Format("Child process count : {0}\n", result.Count);
                        if (result.Count > 0)
                        {
                            foreach (ManagementObject item in result)
                            {
                                childProcess += string.Format(
                                "Process Name: {0}\n\tProcess ID: {1}\n\tFile path: {2}.\n",
                                item["Name"],
                                item["ProcessId"],
                                item["ExecutablePath"]);
                            }
                        }
                    }
                }
            }
            return childProcess;

        }

        public static string getOwnerName(int processId)
        {
            string OwnerSID = String.Empty;
            string processname = String.Empty;
            try
            {
                //ObjectQuery sq = new ObjectQuery
                //    ("Select * from Win32_Process Where ProcessID = '" + processId + "'");
                //ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
                //if (searcher.Get().Count == 0)
                //    return OwnerSID;
                //foreach (ManagementObject oReturn in searcher.Get())
                //{
                //    string[] o = new String[2];
                //    oReturn.InvokeMethod("GetOwner", (object[])o);

                //    processname = (string)oReturn["Name"];
                //    string[] sid = new String[1];
                //    oReturn.InvokeMethod("GetOwnerSid", (object[])sid);
                //    OwnerSID = sid[0];
                //    return OwnerSID;
                //}
                ObjectQuery x = new ObjectQuery("Select * From Win32_Process where ProcessID='" + processId + "'");
                ManagementObjectSearcher mos = new ManagementObjectSearcher(x);
                foreach (ManagementObject mo in mos.Get())
                {
                    string[] s = new string[2];
                    mo.InvokeMethod("GetOwner", (object[])s);
                    return s[0].ToString();
                }
            }
            catch
            {
                return OwnerSID;
            }
            return OwnerSID;
        }

        public static string getNetStat()
        {
            string op = null;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netstat",
                    Arguments = "-nboa",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                op += proc.StandardOutput.ReadLine() + "\n\r";
            }
            return op;
        }

        public static string getFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static string extractNetStatData(string filePath)
        {
            string netStatOp = getNetStat();
            string fn = getFileName(filePath);
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
            //{
            //    file.WriteLine(string.Format("[{0}][**{1}**]", fn,netStatOp ));
            //}

            foreach (var item in netStatOp.Split(new string[] { "\n\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (item.ToLower().Contains(fn.ToLower()))
                {
                    return netStatOp;

                }
            }
            return "NONE";
        }

        public static double getFileSizeMB(string filePath)
        {
            double len = new FileInfo(filePath).Length;
            return (len / 1024) / 1024;
        }

        public static string readFile(string filePath)
        {
            byte[] returnValue = null;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {

                    returnValue = br.ReadBytes((int)fileStream.Length);

                }
            }
            return Convert.ToBase64String(returnValue,
                         0,
                         returnValue.Length);
        }
        public static string getTime()
        {
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}",
                    DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"),
                    DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("000"));
        }

    }
}
