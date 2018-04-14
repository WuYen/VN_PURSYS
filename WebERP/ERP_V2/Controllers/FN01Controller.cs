using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class FN01Controller : BaseController
    {
        // GET: FN01
        public ActionResult Index()
        {
            return View();
        }

        Models.PURSysEntities db = new Models.PURSysEntities();

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.FN01;
            return PartialView("_GridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew(ERP_V2.Models.FN01 item)
        {
            var model = db.FN01;
            item.CREATE_DATE = DateTime.Now;
            item.CREATE_USER = "Admin";
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate(ERP_V2.Models.FN01 item)
        {
            var model = db.FN01;
            item.CREATE_DATE = DateTime.Now;
            item.CREATE_USER = "Admin";
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.FN01_ID == item.FN01_ID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(System.Int32 FN01_ID)
        {
            var model = db.FN01;
            if (FN01_ID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.FN01_ID == FN01_ID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_GridViewPartial", model.ToList());
        }
    }
}