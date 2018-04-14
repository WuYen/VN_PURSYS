using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Models;
using System.Data;
using System.Data.SqlClient;
using DevExpress.XtraPrinting;
using DevExpress.Web;

namespace ERP_V2.Controllers
{
    public class RP01V2Controller : BaseController
    {
        // GET: RP01
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridView(string BA02A_ID, string DateBeg, string DateEnd)
        {
            if (string.IsNullOrWhiteSpace(DateBeg) || string.IsNullOrWhiteSpace(DateEnd))
            {
                return PartialView("_GridView", null);
            }
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            return PartialView("_GridView", GetData(BA02A_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd")));
        }

        public ActionResult GridViewRefreshData(string BA02A_ID, string DateBeg, string DateEnd)
        {
            if (string.IsNullOrWhiteSpace(DateBeg) || string.IsNullOrWhiteSpace(DateEnd))
            {
                return PartialView("_GridView", null);
            }
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            return PartialView("_GridView", GetData(BA02A_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd")));
        }

        public ActionResult ExportToExcel1(string BA02A_ID, string DateBeg, string DateEnd)
        {
            DateTime.TryParse(DateBeg, out DateTime dateBeg);
            DateTime.TryParse(DateEnd, out DateTime dateEnd);
            var dt = GetData(BA02A_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));
            var settings = new GridViewSettings();
            settings.Name = "GridView";
            settings.CallbackRouteValues = new { Controller = "RP01V2", Action = "GridView" };
            settings.CustomActionRouteValues = new { Controller = "RP01V2", Action = "GridViewRefreshData" };
            settings.ClientSideEvents.BeginCallback = "GridViewBegCallback";
            settings.Styles.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9e9e9");
            settings.Styles.Header.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0076c2");
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "PUR_NO";
                    column.Caption = Resources.Resource.TR01A_PUR_NO;
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "PUR_DT";
                    column.Caption = Resources.Resource.TR01A_PUR_DT;
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "ITM_NM";
                    column.Caption = Resources.Resource.BA02A_ITM_NM;
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "INC_NM";
                    column.Caption = Resources.Resource.BA01A_INC_NM;
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "PUR_PR";
                    column.Caption = Resources.Resource.EachPrice;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DecimalPlaces = 2;
                        s.DisplayFormatString = "G29";
                    });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "Each_PR";
                    column.Caption = Resources.Resource.ReceiveMY;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DecimalPlaces = 2;
                        s.DisplayFormatString = "G29";
                    });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "PUR_QT";
                    column.Caption = Resources.Resource.RP01_PUR_QT;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DecimalPlaces = 2;
                        s.DisplayFormatString = "G29";
                    });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "TOT_QT";
                    column.Caption = Resources.Resource.TR01M_ARR_QT;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DecimalPlaces = 2;
                        s.DisplayFormatString = "G29";
                    });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "DIF_QT";
                    column.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
                    //column.UnboundExpression = "[PUR_QT]-[TOT_QT]";
                    column.Caption = Resources.Resource.NotEnoughQT;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        //s.DisplayFormatString = "#,#.#";
                        s.DisplayFormatString = "G29";
                    });
                });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName == "DIF_QT")
                {
                    decimal PUR_QT = (decimal)e.GetListSourceFieldValue("PUR_QT");
                    decimal? TOT_QT = e.GetListSourceFieldValue("TOT_QT") as decimal?;
                    if (!TOT_QT.HasValue)
                    {
                        TOT_QT = 0;
                    }
                    e.Value = PUR_QT - TOT_QT;
                }
            };
            settings.HtmlDataCellPrepared = (s, e) =>
            {
                if (e.DataColumn.FieldName == "TOT_QT")
                {
                    var ss = s as MVCxGridView;
                    var cellValue = ss.GetRowValues(e.VisibleIndex, "PUR_QT") as decimal?;
                    var currentCell = e.CellValue as decimal?;
                    if (cellValue.HasValue && currentCell.HasValue)
                    {
                        if (Decimal.Compare(cellValue.Value, currentCell.Value) != 0)
                        {
                            e.Cell.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                else if (e.DataColumn.FieldName == "DIF_QT")
                {
                    var currentCell = e.CellValue as decimal?;
                    if (currentCell.HasValue && currentCell.Value > 0)
                    {
                        e.Cell.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else if (e.DataColumn.FieldName == "Each_PR")
                {
                    var ss = s as MVCxGridView;
                    var cellValue = ss.GetRowValues(e.VisibleIndex, "PUR_PR") as decimal?;
                    var currentCell = e.CellValue as decimal?;
                    if (currentCell.HasValue)
                    {
                        if (Decimal.Compare(cellValue.Value, currentCell.Value) == 0)
                        {
                            e.Cell.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            e.Cell.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            };

            return GridViewExtension.ExportToXlsx(settings, dt, "商品採購紀錄", new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG });
        }

        public ActionResult GridView2(string TYP_ID, string Year, string Month)
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
            if (string.IsNullOrWhiteSpace(Year) || string.IsNullOrWhiteSpace(Month))
            {
                return PartialView("_GridView2", null);
            }

            DateTime.TryParse(Year + "/" + Month + "/01", out DateTime dateBeg);

            var dateEnd = dateBeg.AddMonths(1).AddDays(-1);


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

        public ActionResult ExportToExcel2(string TYP_ID, string Year, string Month)
        {
            DateTime.TryParse(Year + "/" + Month + "/01", out DateTime dateBeg);
            var dateEnd = dateBeg.AddMonths(1).AddDays(-1);
            var dt = GetGridView2SessionData(TYP_ID, dateBeg.ToString("yyyyMMdd"), dateEnd.ToString("yyyyMMdd"));

            var settings = new GridViewSettings();
            settings.Name = "GridView2";
            settings.Width = 300;
            settings.CallbackRouteValues = new { Controller = "RP01V2", Action = "GridView2" };
            settings.CustomActionRouteValues = new { Controller = "RP01V2", Action = "GridViewRefreshData2" };
            settings.ClientSideEvents.BeginCallback = "GridView2BegCallback";
            settings.ClientSideEvents.EndCallback = "GridView2EndCallback";
            settings.Styles.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9e9e9");
            settings.Styles.Header.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0076c2");
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "BA02A_ID";
                    column.Caption = Resources.Resource.BA02A_ITM_NM;
                    column.EditorProperties().ComboBox(
                        p =>
                        {
                            p.TextField = "ITM_NM";
                            p.ValueField = "BA02A_ID";
                            p.DataSource = ERP_V2.CacheCommonDataModule.GetBA02A();
                        });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "TOT_PR";
                    column.Caption = Resources.Resource.TOT_PR;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DisplayFormatString = "#,0";
                    });
                });
            settings.Settings.ShowFooter = false;
            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            return GridViewExtension.ExportToXlsx(settings, dt, "類別採購項目", new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG });
        }

        public ActionResult GridViewRefreshData2(string TYP_ID, string Year, string Month)
        {
            if (string.IsNullOrWhiteSpace(Year) || string.IsNullOrWhiteSpace(Month) || string.IsNullOrWhiteSpace(TYP_ID))
            {
                return PartialView("_GridView2", null);
            }

            DateTime.TryParse(Year + "/" + Month + "/01", out DateTime dateBeg);
            var dateEnd = dateBeg.AddMonths(1).AddDays(-1);

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
                    + " select A.TR01A_ID, A.PUR_NO, D.ITM_NM, A.PUR_DT, A.BA01A_ID, B.BA02A_ID, B.PUR_PR, B.PUR_QT, C.INC_NM"
                    + " from TR01A as A"
                    + " left join TR01B as B on A.TR01A_ID = B.TR01A_ID"
                    + " left join BA01A as C on A.BA01A_ID = C.BA01A_ID"
                    + " left join BA02A as D on D.BA02A_ID = B.BA02A_ID"
                    + $" where A.PUR_DT >= '{DateBeg}' and A.PUR_DT <= '{DateEnd}'";
            if (!string.IsNullOrWhiteSpace(BA02A_ID))
            {
                command += $" and B.BA02A_ID = {BA02A_ID}";
            }
            command += " ) AA"
               + " LEFT JOIN"
               + " ("
               + " SELECT A.TR01A_ID, A.BA02A_ID, Sum(B.ARR_QT) TOT_QT, Sum(B.INV_MY) TOT_MY, Sum(B.INV_MY) / Sum(B.ARR_QT) as Each_PR"
               + " FROM TR01B as A"
               + " left join TR01C as B on A.TR01B_ID = B.TR01B_ID"
               + " left join TR01A as C on C.TR01A_ID = A.TR01A_ID "
               + $" where C.PUR_DT >= '{DateBeg}' and C.PUR_DT <= '{DateEnd}'";
            if (!string.IsNullOrWhiteSpace(BA02A_ID))
            {
                command += $" and B.BA02A_ID = {BA02A_ID}";
            }
            command += " group by A.TR01A_ID, A.BA02A_ID"
           + " ) BB"
           + " ON AA.TR01A_ID = BB.TR01A_ID and AA.BA02A_ID = BB.BA02A_ID";
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

        //[ValidateInput(false)]
        //public ActionResult PivotGridPartial()
        //{
        //    var dataTable = GetData3();
        //    dataTable.Columns.Add("TYP_NM", typeof(string));
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        var typeID = row["TYP_ID"].ToString();
        //        if (int.TryParse(typeID, out int id))
        //        {
        //            row["TYP_NM"] = CacheCommonDataModule.GetTypeDic().First(x => x.Value == id).Key;
        //        }
        //    }
        //    return PartialView("_PivotGrid", dataTable);
        //}

        public ActionResult GridView3(string Year)
        {
            if (string.IsNullOrWhiteSpace(Year))
            {
                Year = DateTime.Now.Year.ToString();
                //return PartialView("_GridView3", new DataTable());
            }
            var dataTable = GetGridViewSessionData3(Year);
            TempData["Data3"] = dataTable;
            return PartialView("_GridView3", dataTable);
        }

        public ActionResult GridViewRefreshData3(string Year)
        {
            if (string.IsNullOrWhiteSpace(Year))
            {
                return PartialView("_GridView3", new DataTable());
            }
            var date = GetGridViewSessionData3(Year, true);
            TempData["Data3"] = date;
            return PartialView("_GridView3", date);
        }

        public ActionResult ChartPartial3()
        {
            //var temp = TempData["DT"] as DataTable;
            var model = TempData["Data3"] as DataTable;// GetData2();
            return PartialView("_ChartPartial3", model);
        }

        private DataTable GetData3(string Year)
        {
            var ds = new DataSet();
            var command = "SELECT TYP_ID,"
                + $" [{Year}01] as '{Year}01', [{Year}02] as '{Year}02', [{Year}03] as '{Year}03', [{Year}04] as '{Year}04',"
                + $" [{Year}05] as '{Year}05', [{Year}06] as '{Year}06', [{Year}07] as '{Year}07', [{Year}08] as '{Year}08',"
                + $" [{Year}09] as '{Year}09', [{Year}10] as '{Year}10', [{Year}11] as '{Year}11', [{Year}12] as '{Year}12' "
                + $" FROM("
                + $" select C.TYP_ID, left(D.ARR_DT, 6) as DT, SUM(D.INV_MY * D.CUR_RT) as MonthTotal"
                + $" from TR01B as A"
                + $" left join BA02A as C on A.BA02A_ID = C.BA02A_ID"
                + $" left join TR01C as D on A.TR01B_ID = D.TR01B_ID"
                + $" where C.TYP_ID > 0 and D.INV_MY > 0 and left(D.ARR_DT, 4) = '{Year}'"
                + $" group by C.TYP_ID, left(D.ARR_DT, 6)"
                + $" ) as GroupTable"
                + $" PIVOT"
                + $" ("
                + $" Sum(MonthTotal)"
                + $" FOR DT IN([{Year}01], [{Year}02], [{Year}03], [{Year}04], [{Year}05], [{Year}06], [{Year}07], [{Year}08], [{Year}09], [{Year}10], [{Year}11], [{Year}12])"
                + $" ) AS PivotTable";
            SQLCommandReader(command, ds);
            return ds.Tables[0];
        }

        private DataTable GetGridViewSessionData3(string Year, bool Reload = false)
        {
            var data = Session["GridViewData3"] as DataTable;
            if (data == null || Reload)
            {
                data = GetData3(Year);
                data.Columns.Add("TYP_NM", typeof(string));
                foreach (DataRow row in data.Rows)
                {
                    var typeID = row["TYP_ID"].ToString();
                    if (int.TryParse(typeID, out int id))
                    {
                        row["TYP_NM"] = CacheCommonDataModule.GetBA04AText(UserInfo.LanguageType, int.Parse(typeID));
                    }
                }
                Session["GridViewData3"] = data;
            }
            return data;
        }

        public ActionResult ExportToExcel3(string Year)
        {
            var dt = GetGridViewSessionData3(Year);

            var settings = new GridViewSettings();
            settings.Name = "GridView3";
            //settings.Width = Unit.Percentage(100);
            settings.CallbackRouteValues = new { Controller = "RP01V2", Action = "GridView3" };
            settings.CustomActionRouteValues = new { Controller = "RP01V2", Action = "GridViewRefreshData3" };
            settings.ClientSideEvents.BeginCallback = "GridViewBegCallback3";
            settings.ClientSideEvents.EndCallback = "GridViewEndCallback3";
            settings.Styles.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9e9e9");
            settings.Styles.Header.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0076c2");
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "TYP_NM";
                    column.Caption = Resources.Resource.TYP_ID;
                });
            foreach (System.Data.DataColumn item in dt.Columns)
            {
                var month = item.ColumnName.Substring(4);
                if (System.Text.RegularExpressions.Regex.IsMatch(item.ColumnName, "^[0-9]*$") && int.Parse(month) <= DateTime.Now.Month)
                {
                    settings.Columns.Add(
                        column =>
                        {
                            column.FieldName = item.ColumnName;
                            column.Caption = item.ColumnName.Insert(4, "-");
                            column.EditorProperties().SpinEdit(s =>
                            {
                                s.DisplayFormatString = "#,0";
                            });
                        });
                }
            }

            settings.Settings.ShowFooter = false;
            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            return GridViewExtension.ExportToXlsx(settings, dt, Year + "採購彙總", new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG });
        }

        //private DataTable GetData3()
        //{
        //    var ds = new DataSet();
        //    var command = "select C.TYP_ID, left(D.ARR_DT, 6) as DT, SUM(D.INV_MY * D.CUR_RT) as MonthTotal"
        //            + " from TR01B as A"
        //            + " left join BA02A as C on A.BA02A_ID = C.BA02A_ID"
        //            + " left join TR01C as D on A.TR01B_ID = D.TR01B_ID"
        //            + " where C.TYP_ID > 0 and D.INV_MY > 0"
        //            + " group by C.TYP_ID,left(D.ARR_DT, 6)";
        //    SQLCommandReader(command, ds);
        //    return ds.Tables[0];
        //}
    }

}


