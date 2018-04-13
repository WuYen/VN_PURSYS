using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
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
            if (string.IsNullOrEmpty(Year))
            {
                return PartialView("_GridView", new DataTable());
            }
            return PartialView("_GridView", GetData(Year));
        }

        public ActionResult ChartPartial(string Year)
        {
            if (string.IsNullOrEmpty(Year))
            {
                return PartialView("_ChartPartial", new DataTable());
            }
            var model = GetData(Year);
            return PartialView("_ChartPartial", model);
        }

        public ActionResult GridViewRefreshData(string Year)
        {
            return PartialView("_GridView", GetData(Year, true));
        }

        private DataTable GetData(string Year, bool Reload = false)
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
            return data.Tables[0];
        }

        public ActionResult ExportToExcel(string Year)
        {
            var dt = GetData(Year);

            var settings = new GridViewSettings();
            settings.Name = "GridView";
            settings.Styles.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9e9e9");
            settings.Styles.Header.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0076c2");
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "BA04A_ID";
                    column.Caption = Resources.Resource.TYP_ID;
                    column.EditorProperties().ComboBox(
                        p =>
                        {
                            p.TextField = "key";
                            p.ValueField = "value";
                            p.DataSource = ERP_V2.CacheCommonDataModule.GetBA04A(ERP_V2.UserInfo.LanguageType, false);
                        });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "YearTotal";
                    column.Caption = Resources.Resource.TOT_PR;// "採購金額";
                    column.EditorProperties().SpinEdit(s =>
                        {
                            s.DisplayFormatString = "#,0";
                        });
                });
            return GridViewExtension.ExportToXlsx(settings, dt, Year + Resources.Resource.YearTypePurchaseRatio, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG });
        }



        public ActionResult GridView2(string Year, string Month)
        {
            if (string.IsNullOrWhiteSpace(Year) || string.IsNullOrWhiteSpace(Month))
            {
                return PartialView("_GridView2", new DataTable());
            }
            return PartialView("_GridView2", GetData2(Year, Month));
        }

        public ActionResult GridViewRefreshData2(string Year, string Month)
        {
            var data = GetData2(Year, Month, true);
            return PartialView("_GridView2", data);
        }

        public ActionResult ChartPartial2(string Year, string Month)
        {
            if (string.IsNullOrWhiteSpace(Year) || string.IsNullOrWhiteSpace(Month))
            {
                return PartialView("_ChartPartial2", new DataTable());
            }
            var model = GetData2(Year, Month);
            return PartialView("_ChartPartial2", model);
        }

        private DataTable GetData2(string Year, string Month, bool Reload = false)
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
            return data.Tables[0];
        }

        public ActionResult ExportToExcel2(string Year, string Month)
        {
            var dt = GetData2(Year, Month);

            var settings = new GridViewSettings();
            settings.Name = "GridView2";
            settings.Styles.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9e9e9");
            settings.Styles.Header.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0076c2");
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "BA04A_ID";
                    column.Caption = Resources.Resource.TYP_ID;
                    column.EditorProperties().ComboBox(
                        p =>
                        {
                            p.TextField = "key";
                            p.ValueField = "value";
                            p.DataSource = ERP_V2.CacheCommonDataModule.GetBA04A(ERP_V2.UserInfo.LanguageType, false);
                        });
                });
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = "MonthTotal";
                    column.Caption = Resources.Resource.TOT_PR;
                    column.EditorProperties().SpinEdit(s =>
                    {
                        s.DisplayFormatString = "#,0";
                    });
                });
            return GridViewExtension.ExportToXlsx(settings, dt, Year + Month + Resources.Resource.MonthTypePurchaseRatio, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG });
        }
    }
}