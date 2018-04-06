using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ERP_V2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

        }
        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            var cultureInfoName = Utility.Language.CN;
            try
            {
                switch (UserInfo.LanguageType)
                {
                    case Language.Type.EN:
                        cultureInfoName = Language.US;
                        break;
                    case Language.Type.TW:
                        cultureInfoName = Language.TW;
                        break;
                    case Language.Type.CN:
                        cultureInfoName = Language.CN;
                        break;
                    case Language.Type.VN:
                        cultureInfoName = Language.VN;
                        break;
                }
                //if (!string.IsNullOrWhiteSpace(UserInfo.LanguageType))
                //{
                //    cultureInfoName = UserInfo.LanguageCode;
                //}
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
            }

            // set culture
            System.Threading.Thread.CurrentThread.CurrentCulture =
            new System.Globalization.CultureInfo(Language.US);
            System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(cultureInfoName);
        }
    }
}