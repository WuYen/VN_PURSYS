using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class RP02Controller : Controller
    {
        // GET: RP02
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GridView(string Year)
        {
            return PartialView("_GridView", GetData(Year).Tables[0]);
        }

        public ActionResult ChartPartial(string Year)
        {
            var model = GetData(Year).Tables[0];
            return PartialView("_ChartPartial", model);
        }

        public ActionResult GridViewRefreshData(string Year)
        {
            return PartialView("_GridView", GetData(Year, true).Tables[0]);
        }

        private DataSet GetData(string Year, bool Reload = false)
        {
            var data = Session["GridViewData"] as DataSet;
            if (data == null || Reload)
            {
                string sqlCmd = "SELECT AA.BA04A_ID,BB.YearTotal FROM"
                                + " ("
                                + " select* from BA04A where BA04A_ID > 0"
                                + " ) AA"
                                + " LEFT JOIN"
                                + " ("
                                + " select C.TYP_ID, SUM(D.INV_MY * D.CUR_RT) as YearTotal"
                                + " from TR01B as A"
                                + " left join BA02A as C on A.BA02A_ID = C.BA02A_ID"
                                + " left join TR01C as D on A.TR01B_ID = D.TR01B_ID"
                                + $" where C.TYP_ID > 0 and D.INV_MY > 0 and left(D.ARR_DT, 4) = '{Year}'"
                                + " group by C.TYP_ID"
                                + " ) BB"
                                + " ON AA.BA04A_ID = BB.TYP_ID";
                data = new DataSet();
                new Utility.MSSQL().SQLCommandReader(sqlCmd, data);
                Session["GridViewData"] = data;
            }
            return data;
        }

        public ActionResult GridView2(string Year, string Month)
        {
            return PartialView("_GridView2", GetData2(Year, Month).Tables[0]);
        }

        public ActionResult GridViewRefreshData2(string Year, string Month)
        {
            var data = GetData2(Year, Month, true);
            return PartialView("_GridView2", data.Tables[0]);
        }

        public ActionResult ChartPartial2(string Year, string Month)
        {
            var model = GetData2(Year, Month).Tables[0];
            return PartialView("_ChartPartial2", model);
        }

        private DataSet GetData2(string Year, string Month, bool Reload = false)
        {
            var date = new DateTime();
            DateTime.TryParse(Year + "/" + Month, out date);

            var data = Session["GridViewData2"] as DataSet;
            if (data == null || Reload)
            {
                string sqlCmd = "SELECT AA.BA04A_ID,BB.MonthTotal FROM"
                                + " ("
                                + " select* from BA04A where BA04A_ID > 0"
                                + " ) AA"
                                + " LEFT JOIN"
                                + " ("
                                + " select C.TYP_ID, SUM(D.INV_MY * D.CUR_RT) as MonthTotal"
                                + " from TR01B as A"
                                + " left join BA02A as C on A.BA02A_ID = C.BA02A_ID"
                                + " left join TR01C as D on A.TR01B_ID = D.TR01B_ID"
                                + $" where C.TYP_ID > 0 and D.INV_MY > 0 and left(D.ARR_DT, 6) = '{date.ToString("yyyyMM")}'"
                                + " group by C.TYP_ID"
                                + " ) BB"
                                + " ON AA.BA04A_ID = BB.TYP_ID";
                data = new DataSet();
                new Utility.MSSQL().SQLCommandReader(sqlCmd, data);
                Session["GridViewData2"] = data;
            }
            return data;
        }
    }
}