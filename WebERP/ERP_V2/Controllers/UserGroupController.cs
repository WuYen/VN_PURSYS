using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Services.UserGroup;
using ERP_V2.Models;
using ERP_V2.ViewModel.UserGroup;

namespace ERP_V2.Controllers
{
    public class UserGroupController : Controller
    {
        private UserGroupService _Service = new Services.UserGroup.UserGroupService();
        private UserGroupBusiness _Business = new Services.UserGroup.UserGroupBusiness();

        // GET: UserGroup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MasterGrid()
        {
            return PartialView("_MasterGrid", _Service.GetQuerable().ToList());
        }

        public ActionResult DetailGrid(int? key)
        {
            if (key.HasValue)
            {
                var data = _Service.GetByKey(key.Value);
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", UserGroupBusiness.FromEntity(data));
                return PartialView("_DetailGrid", _Service.GetGroupUser(key.Value));
            }
            else
            {
                //ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", new ViewModel.UserGroup.UserGroupModel());
                return PartialView("_DetailGrid", new List<Models.SystemUser>());
            }
        }
        public ActionResult DetailGridBatchUpdate(MVCxGridViewBatchUpdateValues<SystemUser, int> updateValues, List<int> permissions, UserGroupModel master)
        {
            //TODO
            //master 要轉成 Entity(UserGroup)

            var deleteKeys = updateValues.DeleteKeys;
            var insertList = updateValues.Insert;
            // SaveChanges(ref Models.UserGroup entity, List<int> permissions, List<int> insertUser, List<int> deleteUser, EntityState state)

            var data = _Service.GetByKey(master.ID);
            UserGroupBusiness.ToEntity(ref data, master);

            ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", UserGroupBusiness.FromEntity(data));
            ViewData["TreeList"] = ReadViewHelper.PartialView(this, "_MasterForm", UserGroupBusiness.FromEntity(data));
            return PartialView("_DetailGrid", _Service.GetGroupUser(master.ID));
        }

        public ActionResult TreeList(int? key)
        {
            if (key.HasValue)
            {
                var data = _Service.GetGroupPermissions(key.Value);
                return PartialView("_TreeList", data);
            }
            else
            {
                //ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", new ViewModel.UserGroup.UserGroupModel());
                return PartialView("_TreeList", new List<Models.UserGroupPermission>());
            }
        }
    }
}