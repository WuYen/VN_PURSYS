using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Models;
using System.Data;
using System.Data.SqlClient;

namespace ERP_V2.Controllers
{
    public class RP01V2Controller : Controller
    {
        // GET: RP01
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridView(string BA02A_ID, string DateBeg, string DateEnd)
        {
            if (string.IsNullOrWhiteSpace(BA02A_ID))
            {
                return PartialView("_GridView", null);
            }
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            return PartialView("_GridView", GetData(BA02A_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd")));
        }

        public ActionResult GridViewRefreshData(string BA02A_ID, string DateBeg, string DateEnd)
        {
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            return PartialView("_GridView", GetData(BA02A_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd")));
        }

        public ActionResult GridView2(string TYP_ID, string DateBeg, string DateEnd)
        {
            //DateTime.TryParse(DateBeg, out DateTime dateBeg);
            //DateTime.TryParse(DateEnd, out DateTime dateEnd);
            //if (string.IsNullOrWhiteSpace(TYP_ID))
            //{
            //    TYP_ID = "2";
            //}
            //var data = GetData2(TYP_ID, "20170101", "20171231");
            //TempData["DT"] = data;
            //return PartialView("_GridView2", data);
            if (string.IsNullOrWhiteSpace(DateBeg) || string.IsNullOrWhiteSpace(DateEnd))
            {
                return PartialView("_GridView2", null);
            }

            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);

            var data = GetGridView2SessionData(TYP_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));
            TempData["DT"] = data;
            return PartialView("_GridView2", data);
        }
        private DataTable GetGridView2SessionData(string TYP_ID, string DateBeg, string DateEnd)
        {
            var data = Session["GridView2Data"] as DataTable;
            if (data == null)
            {
                DateTime.TryParse(DateBeg, out DateTime dateBeg);
                DateTime.TryParse(DateEnd, out DateTime dateEnd);
                data = GetData2(TYP_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));
                Session["GridView2Data"] = data;
            }
            return data;
        }

        public ActionResult GridViewRefreshData2(string TYP_ID, string DateBeg, string DateEnd)
        {
            //if (string.IsNullOrWhiteSpace(TYP_ID))
            //{
            //    TYP_ID = "2";
            //}
            //DateTime.TryParse(DateBeg, out DateTime dateBeg);
            //DateTime.TryParse(DateEnd, out DateTime dateEnd);
            //var data = GetData2(TYP_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));
            //TempData["DT"] = data;
            //return PartialView("_GridView2", data);
            if (string.IsNullOrWhiteSpace(DateBeg) || string.IsNullOrWhiteSpace(DateEnd))
            {
                return PartialView("_GridView2", null);
            }
            if (string.IsNullOrWhiteSpace(TYP_ID))
            {
                TYP_ID = "2";
            }
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            var data = GetData2(TYP_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));
            TempData["DT"] = data;
            Session["GridView2Data"] = data;
            return PartialView("_GridView2", data);
        }

        private DataTable GetData(string BA02A_ID = "1005", string DateBeg = "20170101", string DateEnd = "20171212")
        {
            var ds = new DataSet();
            var command = "SELECT AA.*,BB.TOT_QT,BB.TOT_MY,BB.Each_PR FROM"
                    + " ("
                    + " select A.TR01A_ID, A.PUR_NO, A.PUR_DT, A.BA01A_ID, B.BA02A_ID, B.PUR_PR, B.PUR_QT, C.INC_NM"
                    + " from TR01A as A"
                    + " left join TR01B as B on A.TR01A_ID = B.TR01A_ID"
                    + " left join BA01A as C on A.BA01A_ID = C.BA01A_ID"
                    + $" where B.BA02A_ID = {BA02A_ID} and A.PUR_DT >= '{DateBeg}' and A.PUR_DT <= '{DateEnd}'"
                    + " ) AA"
                    + " LEFT JOIN"
                    + " ("
                    + " SELECT A.TR01A_ID, A.BA02A_ID, Sum(B.ARR_QT) TOT_QT, Sum(B.INV_MY) TOT_MY, Sum(B.INV_MY) / Sum(B.ARR_QT) as Each_PR"
                    + " FROM TR01B as A"
                    + " left join TR01C as B on A.TR01B_ID = B.TR01B_ID"
                    + $" where A.BA02A_ID = {BA02A_ID}"
                    + " group by A.TR01A_ID, A.BA02A_ID"
                    + " ) BB"
                    + " ON AA.TR01A_ID = BB.TR01A_ID";
            SQLCommandReader(command, ds);
            return ds.Tables[0];
        }

        private void SQLCommandReader(string command, DataSet ds)
        {
            using (var _Entity = new PURSysEntities())
            {
                var conn = _Entity.Database.Connection;
                var connectionState = conn.State;
                if (connectionState != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = command;
                    cmd.CommandTimeout = 60;
                    using (var reader = cmd.ExecuteReader())
                    {
                        do
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            ds.Tables.Add(dt);
                        }
                        while (!reader.IsClosed);
                    }
                }
            }
        }

        private DataTable GetData2(string TYP_ID = "2", string DateBeg = "20170101", string DateEnd = "20171212")
        {
            var ds = new DataSet();
            var command = "select B.BA02A_ID, sum(D.INV_MY * D.CUR_RT) as TOT_PR from TR01A as A"
                        + " left join TR01B as B on A.TR01A_ID = B.TR01A_ID"
                        + " left join TR01C as D on D.TR01B_ID = B.TR01B_ID"
                        + " left join BA02A as C on C.BA02A_ID = B.BA02A_ID"
                        + $" where D.ARR_DT >= '{DateBeg}' and D.ARR_DT <= '{DateEnd}' and C.TYP_ID = {TYP_ID} and D.INV_MY > 0"
                        + " group by B.BA02A_ID";
            SQLCommandReader(command, ds);
            return ds.Tables[0];
        }

        public ActionResult ChartPartial()
        {
            //var temp = TempData["DT"] as DataTable;
            var model = TempData["DT"] as DataTable;// GetData2();
            return PartialView("_ChartPartial", model);
        }

        private DataTable GetData3()
        {
            var ds = new DataSet();
            var command = "select C.TYP_ID, left(D.ARR_DT, 6) as DT, SUM(D.INV_MY * D.CUR_RT) as MonthTotal"
                    + " from TR01B as A"
                    + " left join BA02A as C on A.BA02A_ID = C.BA02A_ID"
                    + " left join TR01C as D on A.TR01B_ID = D.TR01B_ID"
                    + " where C.TYP_ID > 0 and D.INV_MY > 0"
                    + " group by C.TYP_ID,left(D.ARR_DT, 6)";
            SQLCommandReader(command, ds);
            return ds.Tables[0];
        }

        [ValidateInput(false)]
        public ActionResult PivotGridPartial()
        {
            var dataTable = GetData3();
            dataTable.Columns.Add("TYP_NM", typeof(string));
            foreach (DataRow row in dataTable.Rows)
            {
                var typeID = row["TYP_ID"].ToString();
                if (int.TryParse(typeID, out int id))
                {
                    row["TYP_NM"] = CacheCommonDataModule.GetTypeDic().First(x => x.Value == id).Key;
                }
            }
            return PartialView("_PivotGrid", dataTable);
        }
    }

}


