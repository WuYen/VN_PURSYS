using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace ERP_V2
{
    public class PermissionsAttribute : FilterAttribute, IAuthorizationFilter
    {        
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //目前route
            string area = null;
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                area = filterContext.RouteData.DataTokens["area"].ToString().Trim().ToLower();
            }
            var controller = filterContext.RouteData.Values["controller"].ToString().Trim().ToLower();
            var action = filterContext.RouteData.Values["action"].ToString().Trim().ToLower();

            
            if (UserInfo.Id == null)
            {
                ValidateFail(filterContext);
                return;
            }

            var path = "~/" + controller + "/" + action;

            ////如果有設在 SYS_Permissions 表示就有權限問題 其餘皆不驗證
            //if (UserInfo.AllPermissionsList.Where(x => x.REF_ID != null && x.REF_ID.ToLower().Contains(path)).Count() > 0)
            //{
            //    if (UserInfo.PermissionsList.Where(x => x.REF_ID != null && x.REF_ID.ToLower().Contains(path)).Count() == 0)
            //    {
            //        ValidatePermFail(filterContext);
            //        return;
            //    }
            //}            

            return;
        }

        private void ValidateFail(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.End();
            }

            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "area", ""},
                    { "controller", "Login" },
                    { "action", "Index" }
                });
        }

        /// <summary>驗證權限</summary>
        /// <param name="filterContext"></param>
        private void ValidatePermFail(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.End();
            }

            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "area", ""},
                    { "controller", "Home" },
                    { "action", "Index" }
                });
        }
    }
}