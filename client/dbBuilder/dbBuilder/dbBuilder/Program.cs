using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

using System.Xml;
using System.Collections;

namespace dbBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            buildDb();
        }
        #region XML DB
        private static bool buildDb()
        {
            try
            {

                List<string> hiveList = new List<string>();
                hiveList.Add("HKEY_LOCAL_MACHINE");

                List<string> subKeyList = new List<string>();
                subKeyList.Add("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                //subKeyList.Add("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\SharedTaskScheduler");
                //subKeyList.Add("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ShellServiceObjectDelayLoad");
                

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = null;
                foreach (var hive in hiveList)
                {
                    writer = XmlWriter.Create(hive + ".xml", settings);
                    writer.WriteStartDocument();
                    writer.WriteComment("This file is generated for HIVE : " + hive);
                    writer.WriteStartElement("keySet");

                    Console.Write(".");
                    foreach (var subKey in subKeyList)
                    {
                        writer.WriteStartElement("subKey");
                        writer.WriteAttributeString("keyName", subKey.Replace("\\", "\\\\"));
                        Console.Write(".");

                        Hashtable r = ReadRegistryValues(getHive(hive), subKey);

                        foreach (DictionaryEntry dictionaryEntry in r)
                        {
                            writer.WriteStartElement("valueSet");
                            writer.WriteAttributeString("keyName", dictionaryEntry.Key.ToString());
                            writer.WriteAttributeString("Value", dictionaryEntry.Value.ToString());
                            writer.WriteEndElement();
                            Console.Write(".");
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                    writer.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //hasError = true;
                return false;
            }
        }
        public static Hashtable ReadRegistryValues(RegistryKey regHive, string regKey)
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
        public static RegistryKey getHive(string str)
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
        #endregion
    }
}
