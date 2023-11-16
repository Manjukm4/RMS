using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//using context = System.Web.HttpContext;

namespace DBLibrary
{
    public static class  ExceptionLogging
    {
        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd,controller;

        public static void SendErrorToText(Exception ex,string ControllerName)
        {
            var line = Environment.NewLine + Environment.NewLine;
            //str.IndexOf("How")
          //  ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.IndexOf("line"), 8);
           // controller = ex.StackTrace.Substring(ex.StackTrace.IndexOf("Controllers\\"), ex.StackTrace.IndexOf("Controller.")); 
            controller = ControllerName;
            if (String.IsNullOrEmpty(ControllerName))
            {
                controller = ex.StackTrace.Substring(ex.StackTrace.IndexOf("Controllers\\"), ex.StackTrace.IndexOf("Controller."));
            }
             
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            //exurl = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName;
           // exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();

            try
            {
                ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.IndexOf("line"), 8);
                // string filepath = context.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path
                string filepath = AppDomain.CurrentDomain.BaseDirectory + @"\" + "Error";  //Text File Path

                //if (!Directory.Exists(filepath))
                //{
                //    Directory.CreateDirectory(filepath);

                //}
                //filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                filepath = filepath   + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {


                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Controller:" + " " + controller + line;// + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

    }
}
