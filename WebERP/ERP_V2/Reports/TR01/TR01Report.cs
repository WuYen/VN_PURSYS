using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Web;

namespace ERP_V2.Reports.TR01
{
    public partial class TR01Report : DevExpress.XtraReports.UI.XtraReport
    {
        public TR01Report(TR01ReportModel model)
        {
            InitializeComponent();
            string picPath = "";
            xrTableCellTotalMoney.Text = $"含税额 Tổng tiền có thuế（{model.Unit}):";//含税额 Tổng tiền có thuế(VND)
            xrTableCell59.Text = $"未税额 Tổng tiền chưa thuế（{model.Unit}):";//未税额 Tổng tiền chưa thuế(VND)
            string companyLabel = "";
            string companyLabel2 = "";
            if (model.Company.Contains("劲亨"))
            {
                companyLabel = "劲亨金属表面处理有限公司";
                companyLabel2 = "CÔNG TY TNHH JINGHENG METAL TREATMENT";
                xrLabel1.Text = "北寧省北寧市桂武工業區 H7-1";
                xrLabel3.Text = "Địa chỉ: Lô H7-1, KCN Quế Võ, Vân Dương, TP Bắc Ninh, tỉnh Bắc Ninh";
                picPath = "/Reports/TR01/JH.jpg";
            }
            else
            {
                companyLabel = "亨利五金工業有限公司";
                companyLabel2 = "CÔNG TY TNHH HENRY HARDWARE INDUSTRY";
                xrLabel1.Text = "北寧省北寧市桂武工業區 H7-2";
                xrLabel3.Text = "Địa chỉ: Lô H7-2, KCN Quế Võ, Vân Dương, TP Bắc Ninh, tỉnh Bắc Ninh";
                picPath = "/Reports/TR01/HR.jpg";
            }
      
            if (model.TaxRate < 1.05m)
            {
                xrTableCellTaxRate.Text = "0%";
            }
            else if (model.TaxRate < 1.07m && model.TaxRate > 1.04m)
            {
                xrTableCellTaxRate.Text = "5%";             
            }
            else
            {
                xrTableCellTaxRate.Text = "10%";    
            }

            xrLabelCompany.Text = companyLabel;// + "\n" + companyLabel2;
            CompanyLabelEN.Text = companyLabel2;
            //xrLabelCompany.Lines[0] = companyLabel;
            //xrLabelCompany.Lines[1] = companyLabel2;
            xrTableCellCompany.Text = model.Company;
            xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            //xrTableCell3.Text = Resources.Resource.Test;
            //xrTableCell3.Font
            var path = "http://" + HttpContext.Current.Request.Url.Authority + picPath;


            xrPictureBox1.ImageUrl = path;// "http://localhost:2524/Reports/TR01/JH.jpg";
            //xrPictureBox1.Image = new Bitmap("~/Reports/TR01/HR.jpg");
            //xrPictureBox1.ImageUrl = new Bitmap("C:\\test.bmp"); "~/Reports/TR01/HR.jpg";
            //"~/Reports/TR01/HR.jpg"
            //"~/Reports/TR01/JH.jpg"
        }


    }
}
