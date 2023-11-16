using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DBLibrary.UnitOfWork;
using DBLibrary;

namespace RIC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //MvcSiteMapProvider.DI.Composer.Compose();
        }
        protected void  Application_Error()
        {
            Exception ex = Server.GetLastError();
            ExceptionLogging.SendErrorToText(ex,"");
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
          
            if (Request.IsAuthenticated)
            {
                if (Session==null|| Session.Count == 0)
                {
                    FormsAuthentication.SignOut();
                }

                UnitOfWork unitOfWork = new UnitOfWork();
                string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                var notification=unitOfWork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false);
                Session["NotificationCount"] = notification.Count();
                Session["InterimReviews"] = notification.Where(s => s.RN_NotificationType == "InterimReviewRequest" 
                                            || s.RN_NotificationType == "InterimReviewUpdate").Count();
                Session["InterimReviewRequest"] = notification.Where(s => s.RN_NotificationType == "InterimReviewRequest").Count();
                Session["InterimReviewUpdate"] = notification.Where(s => s.RN_NotificationType == "InterimReviewUpdate").Count();
            }            
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}