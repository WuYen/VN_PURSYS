using ERP_V2.Models;
using ERP_V2.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using ERP_V2.Services.SysUser;
using System.Linq;
using ERP_V2.ViewModel.SysUser;
using System.Data.Entity;

namespace ERP_V2.Controllers
{
    public class SysUserController : Controller
    {
        private SysUserService _Service = new SysUserService();
        private SysUserBusiness _Business = new SysUserBusiness();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Grid()
        {
            var list = _Service.GetQuerable().ToList();
            return PartialView("_Grid", list);
        }
        public ActionResult EditForm(SysUserModel model)
        {
            if (model.ID == 0)//新增的時候初始化
            {
                //初始化
                ModelState.Clear();
                model.Password = "00000";
            }
            return PartialView("_EditForm", model);
        }

        public ActionResult AddNew(SysUserModel model)
        {
            var item = new SystemUser();
            var errMsg = Save(model, item, EntityState.Added);

            return ResultHandler(errMsg, model);
        }

        public ActionResult Update(SysUserModel model)
        {
            var item = _Service.GetByKey(model.ID);
            var errMsg = Save(model, item, EntityState.Modified);

            return ResultHandler(errMsg, model);
        }

        public ActionResult Delete(int ID)
        {
            var item = _Service.GetByKey(ID);
            var errMsg = Save(null, item, EntityState.Deleted);

            return ResultHandler(errMsg, null);
        }

        public string Save(SysUserModel model, SystemUser entity, EntityState state)
        {
            var errMsg = _Business.BeforeSave(model, ref entity, state);
            if (errMsg == "")
            {
                errMsg += _Service.SaveChanges(ref entity, state);
            }
            return errMsg;
        }

        private PartialViewResult ResultHandler(string errMsg, SysUserModel model)
        {
            if (errMsg == "")
            {
                ViewData["Success"] = true;
            }
            else
            {
                ViewData["ErrMsg"] = errMsg;
                ViewData["ErrorData"] = model;
            }

            return PartialView("_Grid", _Service.GetQuerable().ToList()); ;
        }

    }
}