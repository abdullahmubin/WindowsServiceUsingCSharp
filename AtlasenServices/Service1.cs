using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AtlasenServices
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            #if DEBUG
                        System.Diagnostics.Debugger.Launch();
            #endif

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 60000 * 15; // 15 minutes
            aTimer.Enabled = true;

        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            WriteToFile("Service is recall at : " + DateTime.Now);
            try
            {
                string urlTogetData = "";
                string urlToSave = "";

                using (WebClient client = new WebClient())
                {
                    System.Collections.Specialized.NameValueCollection reqAtParm = new System.Collections.Specialized.NameValueCollection();
                    System.Collections.Specialized.NameValueCollection reqNoSqlParam = new System.Collections.Specialized.NameValueCollection();

                    reqAtParm.Add("", "");

                    string url = urlTogetData;
                    byte[] responsebytes = client.UploadValues(url, "POST", reqAtParm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    JObject o = JObject.Parse(responsebody);

                    reqNoSqlParam.Add("param", o["paramInfo"].ToString());
                    reqNoSqlParam.Add("param2", o["param2Info"].ToString());
                    reqNoSqlParam.Add("param3", o["param3Info"].ToString());


                    string NoSQlUrl = urlToSave;
                    byte[] NoSQlResponsebytes = client.UploadValues(NoSQlUrl, "POST", reqNoSqlParam);
                    string NoSQlResponsebody = Encoding.UTF8.GetString(NoSQlResponsebytes);
                    JObject obj = JObject.Parse(responsebody);
                    string returnObj = o["Data"].ToString();

                    WriteToFile("Found at : " + DateTime.Now);

                }

            }
            catch (Exception r)
            {
                WriteToFile("found error : " + DateTime.Now);
                WriteToFile("error Name: " + r.Message);
            }
        }

       
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
