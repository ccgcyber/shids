using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Timers;

namespace syslogd
{
    class Program
    {
        static Timer timer = new Timer();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("===========================");
                Console.WriteLine("S-HIDS Syslog Deamon");
                Console.WriteLine("===========================");
                Console.WriteLine(string.Format("[ {0} ] Started Demon", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ")));

                Constant.SYSLOG_SERVER = args[0];
                Constant.DB_SERVER = args[1];
                Constant.DATABASE = args[2];
                Constant.UID = args[3];
                if (args.Length > 4)
                {
                    Constant.PASSWORD = args[4];
                }
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Interval = 30000;// 60;            
                timer.Enabled = true;

                Console.ReadLine();
                string input=string.Empty;
                while ((input = Console.ReadLine()).ToLower() != "q") { }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            getNewSysLogEntries();
        }

        public static string getConnectionString()
        {
            if (Constant.PASSWORD != string.Empty)
            {
                return String.Format("Server={0};Database={1};Uid={2};Pwd={3};",
                    Constant.DB_SERVER, Constant.DATABASE, Constant.UID, Constant.PASSWORD);
            }
            else
            {
                return String.Format("Server={0};Database={1};Uid={2};",
                    Constant.DB_SERVER, Constant.DATABASE, Constant.UID);
            }
        }

        public static long exeNonQuery(MySqlCommand cmd)
        {
            long r;
            MySqlConnection con = new MySqlConnection(getConnectionString());
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
                r = cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con = null;
                }
            }
            return r;
        }

        public static DataSet exeQuery(MySqlCommand cmd)
        {
            MySqlConnection con = new MySqlConnection(getConnectionString());
            try
            {
                con.Open();
                cmd.Connection = con;
                MySqlDataAdapter adpt = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adpt.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con = null;
                }
            }
        }

        public static void getNewSysLogEntries()
        {
            MySqlCommand cmd = null;
            cmd = new MySqlCommand();

            cmd.CommandText = "SELECT  * FROM `incidents` WHERE `incidents`.`syslogd_status`=@iStatus ";
            cmd.Parameters.AddWithValue("@iStatus", Constant.NEW_INCIDENTS);
            DataSet ds = exeQuery(cmd);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine(string.Format("[ {0} ] Started Data Transfer", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ")));
                Console.WriteLine("Found ->" + ds.Tables[0].Rows.Count.ToString());
                string msg = string.Empty;
                string body=string.Empty;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    body=string.Empty;
                    if (row["sha1"].ToString() != string.Empty)
                    {
                        body=string.Format("[{0}][{1}][SHA 1={2}]",
    row["type"].ToString(),
     row["desc"].ToString(),

     row["sha1"].ToString());
                    }
                    else
                    {
                        body=string.Format("[{0}][{1}]",
    row["type"].ToString(),
     row["desc"].ToString());
                    }
                    msg = string.Empty;
                    msg = string.Format("{0} {1} {2} {3}\n",
                        Constant.SEVIARITY,
     DateTime.Parse(
         row["timeStamp"].ToString()
     ).ToString("yyyy-MM-dd HH:mm:ss"),

     row["sourceIp"].ToString(),body);
                    Console.WriteLine(string.Format("[Sending][{0}][{1}]", Constant.SYSLOG_SERVER, msg));

                    Sender.Send(Constant.SYSLOG_SERVER, msg);

                    cmd = new MySqlCommand();

                    cmd.CommandText = "UPDATE `incidents` SET `syslogd_status`=@iStatus  WHERE `id`=@id";

                    cmd.Parameters.AddWithValue("@iStatus", Constant.PROCESSED_INCIDENTS);
                    cmd.Parameters.AddWithValue("@id", row["id"].ToString());

                    exeNonQuery(cmd);
                    cmd = null;
                }
                Console.WriteLine(string.Format("[ {0} ] Ended Data Transfer", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ")));
                Console.WriteLine("-------------------------------------");
            }
            else
            {
                //Console.WriteLine("No entries found");
            }
        }
    }
}
