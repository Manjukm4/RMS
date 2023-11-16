using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Configuration;

namespace RIC.Utility
{
	public  class Email
	{
       static string EmailFrom = ConfigurationManager.AppSettings["EmailFrom"];
       static string EmailTo = ConfigurationManager.AppSettings["EmailTo"];
       static string EmailSubject = ConfigurationManager.AppSettings["EmailSubject"];
       static string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
       static string SMTPUser = ConfigurationManager.AppSettings["SMTPUser"];
       static string SMTPPW = ConfigurationManager.AppSettings["SMTPPW"];






		public static void SendMail(string[] To, string Subject, string Body, out string sError)
		{
			using (SmtpClient SmtpClient = new SmtpClient())
			{
                SmtpClient.Host = SMTPHost;// outlook.com
                SmtpClient.Port = 25;
                // SmtpClient.Credentials = new NetworkCredential(SMTPUser, SMTPPW); //rmssupport@sunrisebiztechsys.com 
                SmtpClient.UseDefaultCredentials = false;
				using (MailMessage Message = new MailMessage())
				{
                    Message.From = new MailAddress(EmailFrom);
					foreach (var Element in To)
					{
						Message.To.Add(Element);
					}
	  
					Message.ReplyToList.Add("FakeId@sunrisebiztechsys.com");
					Message.Subject = Subject;
					Message.IsBodyHtml = true;
					Message.Body = Body;

					try
					{
						SmtpClient.Send(Message);
					}
					catch (Exception ex)
					{
						sError = ex.Message;
						return;
					}
				}
			}
			sError = null;
		}



        public static void SendMail1(string[] To,string Subject, string Body, out string sError)
        {


            using (SmtpClient SmtpClient = new SmtpClient())
            {
                SmtpClient.Host = SMTPHost;
                SmtpClient.Credentials = new NetworkCredential(SMTPUser, SMTPPW);
                SmtpClient.Port = 25;

                using (MailMessage Message = new MailMessage())
                {
                    Message.From = new MailAddress(EmailFrom);


                    foreach (var Element in To)
                    {
                        Message.To.Add(Element);
                    }
                    

                    Message.Subject = Subject;
                    Message.IsBodyHtml = true;
                    Message.Body = Body;

                    try
                    {
                        SmtpClient.Send(Message);
                    }
                    catch (Exception ex)
                    {
                        sError = ex.Message;
                        return;
                    }
                }
            }
            sError = null;
        }

	}
}
