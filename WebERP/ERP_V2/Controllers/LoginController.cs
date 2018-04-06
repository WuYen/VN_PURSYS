using ERP_V2.ViewModels.Login;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ERP_V2.Utility;

namespace ERP_V2.Controllers
{
    public class LoginController : Controller
    {
        //private LoginService _LoginService = new LoginService();

        // GET: Login
        public ActionResult Index()
        {
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
            var userLogin = Account().FirstOrDefault(x => x.Acc == user.USR_ID && x.Psw == user.USR_PW);
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

        private static List<AccountModel> Account()
        {
            List<AccountModel> data = new List<AccountModel>();
            data.Add(new AccountModel() { Acc = "TV101", Psw = "H4981", Permission = "0" });
            data.Add(new AccountModel() { Acc = "TV102", Psw = "T1304", Permission = "0" });
            data.Add(new AccountModel() { Acc = "TV103", Psw = "22890", Permission = "0" });
            data.Add(new AccountModel() { Acc = "TV104", Psw = "99999", Permission = "0" });

            data.Add(new AccountModel() { Acc = "QL102", Psw = "88888", Permission = "1" });
            data.Add(new AccountModel() { Acc = "QL103", Psw = "02618", Permission = "1" });
            data.Add(new AccountModel() { Acc = "admin", Psw = "00000", Permission = "1" });

            return data;
        }

        public class AccountModel
        {
            public string Acc { get; set; }
            public string Psw { get; set; }
            public Language.Type LanguageCode { get; set; }
            public string Permission { get; set; }
        }
    }
}