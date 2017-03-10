using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace shids_client
{
    public class Constant
    {
        public const int TYPE_FILE_MODIFICATION = 1;
        public const int TYPE_REG_MODIFICATION = 2;

        public const string SERVICE_NAME = "s_hid_client";
        public const string SERVICE_DESC = "S HIDS Client Service";

        public const string EVENT_LOG_NAME = "S HIDS Client";
        public const string EVENT_LOG_TYPE = "Application";

        private static List<string> fileEx = null;

        private static Hashtable ht = null;
        public static List<string> getFileExtensions()
        {
            if (fileEx == null)
            {
                fileEx = new List<string> { ".exe", ".dll", ".com", ".pif", ".vbs",".bat" }; ;
            }
            return fileEx;
        }

        public static string getTime()
        {
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}",
                    DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"),
                    DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("000"));
        }
        public static string getTime(int year, int month, int day, int h, int m, int s)
        {
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    year, month, day, h, m, s);
        }

        public static List<string> loadDb(string hiveName)
        {
            if (ht == null)
            {
                ht = new Hashtable();
            }
            List<string> element = null;
            if (startProcess("dbBuilder.exe"))
            {
                element = new List<string>();
                XmlDocument document = new XmlDocument();
                document.Load(hiveName + ".xml");

                XmlNodeList xmlNl = document.SelectNodes("/keySet/subKey");
                foreach (XmlNode item in xmlNl)
                {
                    element.Add(item.Attributes["keyName"].Value);

                    if (!ht.ContainsKey(item.Attributes["keyName"].Value))
                    {
                        if (item.HasChildNodes)
                        {
                            Hashtable htC = new Hashtable();
                            foreach (XmlNode child in item.ChildNodes)
                            {
                                htC.Add(child.Attributes["keyName"].Value, child.Attributes["Value"].Value);
                            }
                            ht.Add(item.Attributes["keyName"].Value, htC);
                        }
                        else
                        {
                            ht.Add(item.Attributes["keyName"].Value, null);
                        }
                    }
                }
            }
            return element;
        }

        public static string getModifiedKey(string regHive, string regKey)
        {
            Hashtable htL = ReadRegistryValues(getHive(regHive), regKey);
            regKey = regKey.Replace("\\", "\\\\");
            if (ht.ContainsKey(regKey))
            {
                var tmp = ht[regKey];
                if (tmp != null)
                {
                    Hashtable htL2 = (Hashtable)ht[regKey];
                    foreach (DictionaryEntry dictionaryEntry in htL)
                    {
                        if (!htL2.ContainsKey(dictionaryEntry.Key.ToString()))
                        {
                            htL2.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                            ht[regKey] = htL2;
                            htL2 = null;
                            tmp = null;
                            htL = null;
                            //updateDatabase(regHive, regKey, dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                            return string.Format("\n[Key Name]:{0}\n[Value]:{1}", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                        }
                    }

                }

            }
            return null;

        }
        private static Hashtable ReadRegistryValues(RegistryKey regHive, string regKey)
        {
            Microsoft.Win32.RegistryKey key = regHive.OpenSubKey(regKey);
            string[] valNames = key.GetValueNames();
            Hashtable hashTable = new Hashtable();
            for (int i = 0; i < valNames.Length; i++)
            {
                hashTable.Add(valNames[i], key.GetValue(valNames[i]).ToString());

            }

            return hashTable;
        }
        public static string ReadRegistrySingleValues(string regHive, string regKey, string value)
        {
            Microsoft.Win32.RegistryKey key = getHive(regHive).OpenSubKey(regKey);
            return key.GetValue(value).ToString();

        }
        private static RegistryKey getHive(string str)
        {
            if (str == "HKEY_LOCAL_MACHINE")
            {
                return Registry.LocalMachine;
            }
            else if (str == "HKEY_CURRENT_USER")
            {
                return Registry.CurrentUser;
            }
            else
            {
                return Registry.CurrentConfig;
            }
        }


        private static bool startProcess(string processName)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();

            string system32 = Environment.GetEnvironmentVariable("WINDIR") + @"\system32\";
            startInfo.FileName = system32 + processName;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = system32;
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //private static void updateDatabase(string hiveName,string regKey, string newKEy, string newValue)
        //{            
        //    XmlDocument document = new XmlDocument();
        //    document.Load(hiveName + ".xml");

        //    XmlNodeList xmlNl = document.SelectNodes("/keySet/subKey");
        //    regKey = regKey.Replace("\\", "\\\\");

        //    foreach (XmlNode xmlN in xmlNl)
        //    {
        //        if (xmlN.Attributes["keyName"].Value==regKey)
        //        {
        //            XmlElement newElem = document.CreateElement("valueSet");
        //            newElem.SetAttribute("keyName", newKEy);
        //            newElem.SetAttribute("Value", newValue);
        //            xmlN.AppendChild(newElem);

        //            document.PreserveWhitespace = true;
        //            XmlTextWriter wrtr = new XmlTextWriter(hiveName + ".xml", System.Text.Encoding.Unicode);
        //            document.WriteTo(wrtr);
        //            wrtr.Close();
        //            break;
        //        }
        //    }
        //}
        public static string readUrl()
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(@"s_hids.url", !System.IO.File.Exists(@"s_hids.url")))
            {
                return file.ReadLine() + "/postIncidents.ws.php";
            }
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
            while (IsFileLocked(filePath)){}
            using (FileStream fileStream = new FileStream(filePath,FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

        private static Boolean IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
