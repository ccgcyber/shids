using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using shids_client;
using System.Net;
using System.Net.Sockets;
using RegMonitor;
using System.Collections.Generic;

namespace WindowsService
{
    class WindowsService : ServiceBase
    {


        #region WindowsService/Main
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            this.ServiceName = Constant.SERVICE_NAME;
            this.EventLog.Source = Constant.EVENT_LOG_NAME;
            this.EventLog.Log = Constant.EVENT_LOG_TYPE;

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            if (!EventLog.SourceExists(Constant.EVENT_LOG_NAME))
                EventLog.CreateEventSource(Constant.EVENT_LOG_NAME, Constant.EVENT_LOG_TYPE);
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }
        #endregion

        #region Serivce
        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart: Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
                this.listenFileModification(Environment.GetEnvironmentVariable("WINDIR"));
                this.listenFileModification(Environment.GetEnvironmentVariable("WINDIR") + @"\system32");

                List<string> regList = Constant.loadDb("HKEY_LOCAL_MACHINE");
                checkKeyChange("HKEY_LOCAL_MACHINE", regList);

                List<string> valuesToCheck = new List<string>();
                valuesToCheck.Add("Shell");
                valuesToCheck.Add("Userinit");
                checkValueChange("HKEY_LOCAL_MACHINE", "SOFTWARE\\\\Microsoft\\\\Windows NT\\\\CurrentVersion\\\\Winlogon", valuesToCheck);
                valuesToCheck = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// OnStop: Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// OnContinue: Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();

        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcase Status (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event from a Terminal Server session.
        ///   Useful if you need to determine when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
        #endregion

        #region File Modification
        private void FileSystemOnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Created
                    || e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    FileInfo f = new FileInfo(e.FullPath);

                    if (Constant.getFileExtensions().Contains(f.Extension.ToLower()))
                    {
                        string time = Constant.getTime();
                        string s = string.Format("[{0}]{1}",
                            e.ChangeType.ToString(),
                            e.FullPath
                            );
                        string sha1 = Constant.getSHA1(e.FullPath);
                        log(s, time, Constant.TYPE_FILE_MODIFICATION.ToString(), sha1);
                        s = null;
                        time = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }

        }

        private void FileSystemOnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {
                    FileInfo f = new FileInfo(e.FullPath);

                    if (Constant.getFileExtensions().Contains(f.Extension.ToLower()))
                    {
                        string time = Constant.getTime();
                        string s = string.Format("[{0}][{1}]->[{2}]",
                            e.ChangeType.ToString(),
                            e.OldFullPath,
                            e.FullPath
                            );
                        string sha1 = Constant.getSHA1(e.FullPath);
                        log(s, time, Constant.TYPE_FILE_MODIFICATION.ToString(), sha1);
                        s = null;
                        time = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }

        }

        private void listenFileModification(string path)
        {
            System.IO.FileSystemWatcher m_Watcher = new System.IO.FileSystemWatcher();
            m_Watcher.Filter = "*.*";
            m_Watcher.Path = path;

            m_Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                     | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //m_Watcher.Changed += new FileSystemEventHandler(FileSystemOnChanged);
            m_Watcher.Created += new FileSystemEventHandler(FileSystemOnChanged);
            m_Watcher.Deleted += new FileSystemEventHandler(FileSystemOnChanged);
            m_Watcher.Renamed += new RenamedEventHandler(FileSystemOnRenamed);
            m_Watcher.EnableRaisingEvents = true;
        }
        #endregion

        #region Logging

        private static void log(string line, string time, string type)
        {
            try
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\log.log", true))
                //{
                //    file.Write(string.Format("{0} {1} {2} {3}",time,
                //    LocalIPAddress(),
                //    type,
                //    line));
                //}


                //r.Url = Constant.readUrl();                
                //r.UseDefaultCredentials = true;

                //r.CallReportIncident(time,
                //    LocalIPAddress(),
                //    type,
                //    line);

                //time, LocalIPAddress(), type, line);
                string ip = LocalIPAddress();
                PostSubmitter post = new PostSubmitter();
                post.Url = Constant.readUrl();
                post.PostItems.Add("post", DateTime.Now.ToShortTimeString());
                post.PostItems.Add("timeStamp", time);
                post.PostItems.Add("sourceIp", ip);
                post.PostItems.Add("type", type);
                post.PostItems.Add("desc", line);
                post.Type = PostSubmitter.PostTypeEnum.Post;
                string result = post.Post();


                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                {
                    file.WriteLine(string.Format("[SUCCESS][{0}][{1}][{2}][{3}]", time, ip, type, line));
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }
        }
        private static void log(string line, string time, string type,string sha1)
        {
            try
            {
                string ip = LocalIPAddress();
                PostSubmitter post = new PostSubmitter();
                post.Url = Constant.readUrl();
                post.PostItems.Add("post", DateTime.Now.ToShortTimeString());
                post.PostItems.Add("timeStamp", time);
                post.PostItems.Add("sourceIp", ip);
                post.PostItems.Add("type", type);
                post.PostItems.Add("desc", line);
                post.PostItems.Add("sha1", sha1);

                post.Type = PostSubmitter.PostTypeEnum.Post;
                string result = post.Post();


                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                //{
                //    file.WriteLine(string.Format("[SUCCESS][{0}][{1}][{2}][{3}]", time, ip, type, line));
                //}
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"shids.log", true))
                //{
                //    file.WriteLine(string.Format("[ERROR][{0}]", ex.Message));
                //}
            }
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
        #endregion

        #region Reg Midification
        static RegistryKeyChange keychange;
        static RegistryValueChange valuechange;

        private static void checkValueChange(string hive, string keyPath, List<string> values)
        {
            valuechange = new RegistryValueChange(hive, keyPath, values);
            valuechange.RegistryValueChanged += new EventHandler<RegistryValueChangedEventArgs>(valuechange_RegistryValueChanged);
            valuechange.Start();
        }

        static void valuechange_RegistryValueChanged(object sender, RegistryValueChangedEventArgs e)
        {
            DateTime eventTimeStamp = DateTime.FromFileTime((long)e.RegistryValueChangeData.TIME_CREATED);

            string time = Constant.getTime(eventTimeStamp.Year, eventTimeStamp.Month, eventTimeStamp.Day
                , eventTimeStamp.Hour, eventTimeStamp.Minute, eventTimeStamp.Second);
            string value =Constant. ReadRegistrySingleValues(e.RegistryValueChangeData.Hive, 
                e.RegistryValueChangeData.KeyPath, 
                e.RegistryValueChangeData.ValueName);
            string desc = string.Format("[Value Modified]\n[Hive]: {0}\n[KeyPath]: {1}\n[Value]:{2}\n[String]:{3} ", e.RegistryValueChangeData.Hive,
                e.RegistryValueChangeData.KeyPath, e.RegistryValueChangeData.ValueName, value);

            log(desc, time, Constant.TYPE_REG_MODIFICATION.ToString());
        }
        private static void checkKeyChange(string hive, List<string> keyList)
        {
            keychange = new RegistryKeyChange(hive, keyList);
            keychange.RegistryKeyChanged += new EventHandler<RegistryKeyChangedEventArgs>(keychange_RegistryKeyChanged);
            keychange.Start();
        }
        static void keychange_RegistryKeyChanged(object sender, RegistryKeyChangedEventArgs e)
        {
            DateTime eventTimeStamp = DateTime.FromFileTime((long)e.RegistryKeyChangeData.TIME_CREATED);
            string time = Constant.getTime(eventTimeStamp.Year, eventTimeStamp.Month, eventTimeStamp.Day
                , eventTimeStamp.Hour, eventTimeStamp.Minute, eventTimeStamp.Second);

            string mk = Constant.getModifiedKey(e.RegistryKeyChangeData.Hive, e.RegistryKeyChangeData.KeyPath);
            string desc = string.Format("[Key Changed]\n[Hive]: {0}\n[KeyPath]: {1}{2} ", e.RegistryKeyChangeData.Hive,
                e.RegistryKeyChangeData.KeyPath, mk);
            log(desc, time, Constant.TYPE_REG_MODIFICATION.ToString());
        }
        #endregion

    }
}
