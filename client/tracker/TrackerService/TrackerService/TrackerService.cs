using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Collections; 

namespace TrackerService
{
    public partial class TrackerService : ServiceBase
    {
        Timer timer = new Timer();
        static string url = null;

        public TrackerService()
        {
            InitializeComponent();
            this.EventLog.Source = Constant.EVENT_LOG_NAME;
            this.EventLog.Log = Constant.EVENT_LOG_TYPE;
            if (!EventLog.SourceExists(Constant.EVENT_LOG_NAME))
                EventLog.CreateEventSource(Constant.EVENT_LOG_NAME, Constant.EVENT_LOG_TYPE);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                url = Constant.readUrl();
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Interval = 30000 ;// 60;            
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                processTrackerRequest();
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }
            
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }


        private static void processTrackerRequest()
        {
            string trackerReqURL = url + "/getBlackBoardData.php?sip=" + LocalIPAddress();
            string updateUrl = url + "/updateBBDataAgentResponse.php?";
            string text = Constant.performWebReq(trackerReqURL);

            if (text != string.Empty)
            {
                try
                {
                    string[] sigs = text.Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                    Hashtable sigList = new Hashtable();
                    string[] sSig;
                    foreach (string sig in sigs)
                    {
                        sSig = sig.Split('|');
                        sigList.Add(sSig[0], sSig[1]);
                        //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                        //{
                        //    file.WriteLine(string.Format("[{0}][{1}]", sSig[0], sSig[1]));
                        //}
                        Constant.performWebReq(updateUrl + "id=" + sSig[0]+"&status="+Constant.BB_STATUS_AGENT_RECEIVED);
                    }
                    foreach (DictionaryEntry dictionaryEntry in sigList)
                    {
                        trackProcess(dictionaryEntry.Key.ToString(),
                            dictionaryEntry.Value.ToString());
 
                    }

                }
                catch (Exception ex)
                {
                    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                    //{
                    //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                    //}
                }
            }
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
            //{
            //    file.WriteLine(string.Format("{0}", text));
            //}
        }



        public static string trackProcess(string bId, string sha1)
        {
            Process[] processlist = Process.GetProcesses();
            string sha1P = null;
            double fileSize;
            string returnString = string.Format("[{0}]Started Process Tracking\n", Constant.getTime());
            returnString += "--------------------------------" + "\n";

            foreach (Process theprocess in processlist)
            {
                sha1P = null;
                try
                {

                    sha1P = Constant. getSHA1(theprocess.Modules[0].FileName).ToLower();
                    
                    if (sha1.ToLower().Trim() == sha1P.ToLower().Trim())
                    {
                        returnString += "[Begin Process Info]";
                        returnString += "-------------------" + "\n";
                        returnString += "HASH -> " + sha1 + "\n";
                        returnString += "-------------------" + "\n";
                        returnString += "Process ID : " + theprocess.Id.ToString() + "\n";
                        returnString += "-------------------" + "\n";
                        returnString += "Owner : " + Constant.getOwnerName(theprocess.Id) + "\n"; ;
                        returnString += "-------------------" + "\n";
                        returnString += "Child Processes :" + "\n";
                        returnString += "-------------------" + "\n";
                        returnString += Constant.getChildProcess(theprocess.Id) + "\n";
                        returnString += "Path :" + "\n";
                        returnString += "-------------------" + "\n";
                        returnString += theprocess.Modules[0].FileName + "\n";
                        returnString += "[End Process Info]";

                        returnString += "[Begin Netstat info]";
                        returnString += "NetStat Info\n";
                        returnString += "-------------------" + "\n";
                        returnString += Constant.extractNetStatData(theprocess.Modules[0].FileName);
                        returnString += "[End Netstat Info]";

                        returnString += "[Begin File info]";

                        fileSize = Constant.getFileSizeMB(theprocess.Modules[0].FileName);
                        returnString += string.Format("File size : {0} MB", fileSize);
                        if (fileSize > 3)
                        {
                            returnString += "File is too large to send.";
                        }
                        else
                        {
                            returnString += "[Begin File Data]";
                            returnString += Constant.readFile(theprocess.Modules[0].FileName);
                            returnString += "[End File Data]";
                        }

                        returnString += "[End File info]";
                    }

                }
                catch (Exception ex)
                {
                    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                    //{
                    //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                    //}
                }
            }
            returnString += "--------------------------------" + "\n";
            returnString += string.Format("\n[{0}]Ended Process Tracking\n",Constant.getTime());
            try
            {                
                PostSubmitter post = new PostSubmitter();
                post.Url =  url + "/postBlacBoardData.php";
                post.PostItems.Add("post", DateTime.Now.ToShortTimeString());
                post.PostItems.Add("b_id", bId);
                post.PostItems.Add("data", returnString);
                post.Type = PostSubmitter.PostTypeEnum.Post;
                string result = post.Post();
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", result));
                //}

            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids_tracker.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}

            }
            return null;
        }

        private static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }
}
