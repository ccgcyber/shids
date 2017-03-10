using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

using System.Xml;
using System.Collections;

namespace buidler
{
    class Program
    {
        static bool hasError = false;
        static void Main(string[] args)
        {
            builtBatch();
            getUrl();
            if (hasError)
            {
                Console.ReadKey();
            }
        }

        #region Building Install Batch File

        private static void getUrl()
        {
            string line=null;
            do{
                Console.WriteLine("Please enter the S-HIDS host url (eg: 192.168.1.1, 192.168.1.1:7890):");
                line = Console.ReadLine();
                Console.WriteLine(string.Format("You have entered: {0}\n\nPress \"y\" to confirm..",line ));
            
            }while(Console.ReadLine().ToLower() != "y");

            writeConfig("http://"+line);
        }
        private static void writeConfig(string line)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"s_hids.url", !System.IO.File.Exists(@"s_hids.url")))
                {
                    file.Write(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                hasError = true;
            }
        }
        private static bool builtBatch()
        {
            try
            {
                Console.WriteLine("Building batch File:");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"builder.bat", !System.IO.File.Exists(@"builder.bat")))
                {
                    file.Write(builtBatchContent());
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                hasError = true;
                return false;
            }
        }
        private static string builtBatchContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@echo OFF");
            sb.AppendLine("set SERVICE=S HIDS Client Service");
            sb.AppendLine("set SERVICE_TRACKER=S HIDS Client Tracker Service");

            sb.AppendLine("net stop \"%SERVICE%\"");
            sb.AppendLine("net stop \"%SERVICE_TRACKER%\"");

            sb.AppendLine("sc delete \"%SERVICE%\"");
            sb.AppendLine("sc delete \"%SERVICE_TRACKER%\"");

            sb.AppendLine("set DOTNETFX2=" + GetFrameworkDirectory());
            sb.AppendLine("set PATH=%PATH%;%DOTNETFX2%");

            sb.AppendLine("echo Installing SHIDS Client service...");
            sb.AppendLine("echo ---------------------------------------------------");
            sb.AppendLine("InstallUtil /i shids_client.exe");

            sb.AppendLine("echo Installing SHIDS Client Tracker Service...");
            sb.AppendLine("echo ---------------------------------------------------");
            sb.AppendLine("InstallUtil /i TrackerService.exe");

            sb.AppendLine("echo ---------------------------------------------------");
            sb.AppendLine("echo Done");
            sb.AppendLine("net start \"%SERVICE%\"");
            sb.AppendLine("net start \"%SERVICE_TRACKER%\"");

            sb.AppendLine("@ECHO OFF");

            return sb.ToString();
        }
        private static string GetFrameworkDirectory()
        {
            // This is the location of the .Net Framework Registry Key
            string framworkRegPath = @"Software\Microsoft\.NetFramework";

            // Get a non-writable key from the registry
            RegistryKey netFramework = Registry.LocalMachine.OpenSubKey(framworkRegPath, false);

            // Retrieve the install root path for the framework
            string installRoot = netFramework.GetValue("InstallRoot").ToString();

            // Retrieve the version of the framework executing this program
            string version = string.Format(@"v{0}.{1}.{2}\",
              Environment.Version.Major,
              Environment.Version.Minor,
              Environment.Version.Build);

            // Return the path of the framework
            return System.IO.Path.Combine(installRoot, version);
        } 
        
        #endregion


    
    }
}
