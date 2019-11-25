using ERP_V2.ViewModels.Login;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ERP_V2.Utility;
using Newtonsoft.Json;

namespace ERP_V2.Controllers
{
    public class LoginController : Controller
    {
        //private LoginService _LoginService = new LoginService();

        // GET: Login
        public ActionResult Index()
        {
            //var result = JsonConvert.SerializeObject(Account());
            if (AccountList == null)
            {
                var file = Server.MapPath("~/App_Data/Account.txt");
                string jsonText = System.IO.File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<List<AccountModel>>(jsonText);
                AccountList = data;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel user)
        {
            //#if DEBUG
            //            user.USR_ID = "admin";
            //            user.USR_PW = "00000";
            //#endif
            string errMsg = "";
            var userLogin = AccountList.FirstOrDefault(x => x.Acc == user.USR_ID && x.Psw == user.USR_PW);
            //var model = _LoginService.GetGroup(user.USR_ID, user.USR_PW);
            //if (!(user.USR_ID == "admin" && user.USR_PW == "00000"))
            //{
            //    errMsg = "帳號或密碼錯誤";
            //}
            if (userLogin == null)
            {
                errMsg = "帳號或密碼錯誤";
            }
            if (!string.IsNullOrWhiteSpace(errMsg))//錯誤訊息
            {
                TempData["LoginMsg"] = new List<string>() { errMsg };
                return View();
            }

            //設立登入者
            var info = new SettingUserInfo()
            {
                Info = new SettingUserInfo.DataStruct()
                {
                    Id = userLogin.Acc,
                    Name = userLogin.Psw,
                    Permission = userLogin.Permission,
                    LanguageType = user.Lang_TP
                    //PermissionsList = model.Data.Permissions,
                    //AllPermissionsList = model.Data.AllPermissions
                }
            };

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        private static List<AccountModel> AccountList { get; set; }

        public class AccountModel
        {
            public string Acc { get; set; }
            public string Psw { get; set; }
            public Language.Type LanguageCode { get; set; }
            public string Permission { get; set; }
        }
    }
}