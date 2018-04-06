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
    public class RP01Controller : Controller
    {
        // GET: RP01
        public ActionResult Index()
        {
            //PURSysEntities entities = new PURSysEntities();
            //var data = entities.BA02A.ToList();

            //var result = entities.Database.SqlQuery<DataTable>();
            //var sql = "select * from TR01A";
            //特樂通
            //string connectionString = "Data Source=192.168.1.7; Initial Catalog = WSBI_Demo; User ID = sa; Password = P@ssw0rd";
            //using (var sqlConnection = new SqlConnection(connectionString))// GetSqlConnectionString()WSBI_Demo
            //{
            //    using (var da = new SqlDataAdapter(sql, sqlConnection))
            //    {
            //        DataSet ds = new DataSet();
            //        da.Fill(ds);
            //        return ds;
            //    }
            //}            
            //using (var ctx = new SchoolDBEntities())
            //{
            ////Get student name of string type
            //string studentName = ctx.Database.SqlQuery<string>("Select studentname 
            //from Student where studentid = 1").FirstOrDefault<string>();
            //}
            //ViewBag.Data = GetData2();
            return View(GetData());
        }

        public ActionResult GridView(string BA02A_ID, string DateBeg, string DateEnd)
        {
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

        public ActionResult GridViewRefreshData2(string TYP_ID, string DateBeg, string DateEnd)
        {
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

        private DataTable GetData(string BA02A_ID = "1005", string DateBeg = "20170101", string DateEnd = "20171212")
        {
            var ds = new DataSet();

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
                    //cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"select A.PUR_NO,A.PUR_DT,A.BA01A_ID,B.BA02A_ID,B.PUR_PR,B.PUR_QT,C.INC_NM from TR01A as A left join TR01B as B on A.TR01A_ID = B.TR01A_ID left join BA01A as C on A.BA01A_ID = C.BA01A_ID where B.BA02A_ID={BA02A_ID} and A.PUR_DT >= '{DateBeg}' and A.PUR_DT <= '{DateEnd}'";
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
            return ds.Tables[0];
        }

        private DataTable GetData2(string TYP_ID = "2", string DateBeg = "20170101", string DateEnd = "20171212")
        {
            var ds = new DataSet();

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
                    //cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"select B.BA02A_ID, sum(B.PUR_PR* B.PUR_QT) TOT_PR from TR01A as A "
                        + "left join TR01B as B on A.TR01A_ID = B.TR01A_ID "
                        + "left join BA02A as C on C.BA02A_ID = B.BA02A_ID "
                        + $"where A.PUR_DT >= '{DateBeg}' and A.PUR_DT <= '{DateEnd}' and C.TYP_ID = {TYP_ID}"
                        + "group by B.BA02A_ID order by TOT_PR DESC";
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

                    //select C.TYP_ID,left(B.PUR_DT, 6) as DT,sum(A.PUR_PR * A.PUR_QT) as MonthTotal from TR01B as A
                    //left join TR01A as B on A.TR01A_ID = B.TR01A_ID
                    //left join BA02A as C on A.BA02A_ID = C.BA02A_ID
                    //where C.TYP_ID > 0
                    //group by C.TYP_ID,left(B.PUR_DT, 6)
                    cmd.CommandText = $"select C.TYP_ID,left(B.PUR_DT, 6) as DT,sum(A.PUR_PR * A.PUR_QT * B.CUR_RT) as MonthTotal from TR01B as A "
                        + "left join TR01A as B on A.TR01A_ID = B.TR01A_ID "
                        + "left join BA02A as C on A.BA02A_ID = C.BA02A_ID "
                        + "where C.TYP_ID > 0 "
                        + "group by C.TYP_ID,left(B.PUR_DT, 6)";
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


