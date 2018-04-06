using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Models;
using ERP_V2.Services.TR01;
using System.Globalization;
using System.Data.Entity.Core.EntityClient;

namespace ERP_V2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductSelectTest()
        {
            return View();
        }

        public ActionResult CreateReport(int key)
        {
            TR01Service _Service = new TR01Service();
            var temp = _Service.GetMD(key);
            var model = new ERP_V2.ViewModels.TR01.Edit();
            model.Master = new ERP_V2.ViewModels.TR01.TR01AViewModel(temp.Master);

            var entity = new PURSysEntities();
            var BA01B = entity.BA01B.FirstOrDefault(x => x.BA01B_ID == model.Master.BA01B_ID);
            if (BA01B == null)
            {
                BA01B = new BA01B();
            }

            var BA01A = entity.BA01A.FirstOrDefault(x => x.BA01A_ID == model.Master.BA01A_ID);

            var dataCollection = new List<ERP_V2.ViewModels.TR01.TR01BViewModel>();
            foreach (var item in temp.DetaiList)
            {
                dataCollection.Add(new ERP_V2.ViewModels.TR01.TR01BViewModel(item));
            }
            model.DetailList = dataCollection;
            var companyName = model.Master.CPN_NM;
            decimal taxRate = 1;
            if (model.Master.TAX_RT.HasValue)
            {
                taxRate = decimal.Parse(model.Master.TAX_RT.Value.ToString());
            }
            var reportModel = new Reports.TR01.TR01ReportModel() { Company = model.Master.CPN_NM, Provider = BA01A.INC_NM, Contact = BA01B.CNT_NM, TelNo = BA01B.TEL_NO, Address = BA01A.ADD_DR, OrderNO = model.Master.PUR_NO, Date = model.Master.DtPUR_DT.Value.ToString("yyyy/MM/dd"), FAX = BA01A.FAX_NO, TaxRate = taxRate };
            reportModel.Unit = entity.BA03A.FirstOrDefault(x => x.BA03A_ID == model.Master.BA03A_ID).CUR_NM;
            reportModel.DetaiList = new List<Reports.TR01.Detail>();
            int i = 1;
            decimal total = 0;
            foreach (var item in dataCollection)
            {
                var BA02A = entity.BA02A.FirstOrDefault(x => x.BA02A_ID == item.BA02A_ID);
                var detailItem = new Reports.TR01.Detail()
                {
                    ItemNO = item.ITM_NM,
                    Spec = BA02A.ITM_SP,
                    Date = model.Master.DtEXP_DT.Value.ToString("yyyy/MM/dd"),
                    Qty = item.PUR_QT,
                    Unit = BA02A.UNT_NM,
                    Price = item.PUR_PR.Value,
                    TotalPrice = item.PUR_QT * item.PUR_PR.Value,
                    RelativeNO = item.REL_NO,
                    SeqNO = i,
                    REM = item.REM_MM
                };
                total += detailItem.TotalPrice;
                reportModel.DetaiList.Add(detailItem);
                i++;
            }
            var result = new List<ERP_V2.Reports.TR01.TR01ReportModel>();
            reportModel.Price = total;//未稅
            reportModel.TotalSummaryPrice = total * taxRate;
            if (UserInfo.LanguageType == Utility.Language.Type.VN)
            {
            }
            reportModel.TotalSummaryPrice = Math.Round(reportModel.TotalSummaryPrice, 0);
            var str = reportModel.TotalSummaryPrice.ToString("G29");

            if (str == "0")
            {
                reportModel.MoneyInChinese = "";
            }
            else
            {
                reportModel.MoneyInChinese = " " + ConvertVN(str); //EastAsiaNumericFormatter.FormatWithCulture("L", total, null, new CultureInfo("zh-TW"));
            }
            // var tempNum = EastAsiaNumericFormatter.FormatWithCulture("L", 50500, null, new CultureInfo("zh-TW"));
            result.Add(reportModel);
            System.Web.HttpContext.Current.Session["tempData"] = result;
            return View();
        }

        public ActionResult DocumentViewerPartial()
        {
            var data = System.Web.HttpContext.Current.Session["tempData"] as List<ERP_V2.Reports.TR01.TR01ReportModel>;
            ERP_V2.Reports.TR01.TR01Report report = new ERP_V2.Reports.TR01.TR01Report(data.First());
            report.DataSource = data;
            return PartialView("_DocumentViewerPartial", report);
        }

        public ActionResult DocumentViewerPartialExport()
        {
            var data = System.Web.HttpContext.Current.Session["tempData"] as List<ERP_V2.Reports.TR01.TR01ReportModel>;
            ERP_V2.Reports.TR01.TR01Report report = new ERP_V2.Reports.TR01.TR01Report(data.First());
            report.DataSource = data;
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        public static List<ERP_V2.Reports.TR01.TR01ReportModel> GetData()
        {
            var reportModel = new Reports.TR01.TR01ReportModel() { Provider = "落泰企業有限公司", Contact = "陳小姐", TelNo = "+886-3-3790713 分機 305", Address = "桃園市蘆竹區中福里興街225號", OrderNO = "T-0001", Date = "2017/01/01", FAX = "123-345-223-3" };
            reportModel.DetaiList = new List<Reports.TR01.Detail>()
            {
                new Reports.TR01.Detail() { ItemNO ="3M-2353", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-4444", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-5555", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-6666", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-7777", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-8888", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  },
                new Reports.TR01.Detail() { ItemNO ="3M-9999", Spec="1KG/罐", Date ="2017/02/03", Qty =5, Unit ="KG", Price = 150, TotalPrice = 750, RelativeNO = "1234-5", SeqNO=1,  REM ="USD"  }
            };
            var result = new List<ERP_V2.Reports.TR01.TR01ReportModel>();
            result.Add(reportModel);
            return result;
        }

        private static string ConvertVN(string value)
        {
            //string[] unit = { "đồng chẵn.", "mười ", "trăm ", "ngàn ", "mươi ngàn ", "trăm ngàn ", "triệu ", "mươi triệu " };
            string[] digitVN = { "lẻ ", "một ", "hai ", "ba ", "bốn ", "năm ", "sáu ", "bẩy ", "tám ", "chín ", "mười " };
            string[] unit = { "đồng chẵn.", "mười ", "trăm ", "ngàn ", "mươi ", "trăm ", "triệu ", "mươi ", "trăm ", "tỷ ", "mươi ", "trăm " };
            //string[] unit = { "圓",         "拾",   "佰",    "仟",    "萬",      "十萬",  "百萬"  "千萬" ,  "億"      "十億"   百億    "千億"};
            List<string> result = new List<string>();
            var valStr = value;
            var digitCount = valStr.Length;

            for (int i = 0; i < digitCount; i++)
            {
                int num = (int)Char.GetNumericValue(valStr[i]);

                var tempDigit = digitVN[num];
                var tempUnit = unit[digitCount - 1 - i];
                if (digitCount - i == 1)//個位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";//最後一位是零的時候不顯示
                    }
                    else if (num == 1 && digitCount > 1)
                    {
                        if ((int)Char.GetNumericValue(valStr[digitCount - 2]) >= 2)
                        {
                            tempDigit = "mốt ";
                        }
                    }
                    else if (num == 5 && digitCount > 1 && valStr[digitCount - 2] != '0')//10位數以上且不為零 個位數字是5
                    {
                        tempDigit = "lăm ";
                    }
                    tempUnit = unit[0];
                }
                else if (digitCount - i == 2)//十位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                    }
                    if (num == 1)//10的時候 只要 mười
                    {
                        tempDigit = "";
                    }

                    if (digitCount >= 3)//有超過百位數
                    {
                        if (num == 0)//0的時候不顯示
                        {
                            tempDigit = "";
                            tempUnit = "";
                            if (valStr[digitCount - 3] != '0' && valStr[digitCount - 1] != '0')//百位數 and 個位數 不為零
                            {
                                tempDigit = digitVN[0];
                            }
                        }
                    }
                }
                else if (digitCount - i == 3)//百位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                    }
                    if (num == 1)
                    {
                        tempDigit = "mốt ";
                    }
                }
                else if (digitCount - i == 4)//千位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        if (digitCount > 4)
                        {
                            if ((int)Char.GetNumericValue(valStr[digitCount - 5]) == 0 && (int)Char.GetNumericValue(valStr[digitCount - 6]) == 0)
                            {
                                tempUnit = "";
                            }
                        }
                    }
                    if (num == 1 && digitCount > 4)//1 的時候 萬位值 != 1
                    {
                        if ((int)Char.GetNumericValue(valStr[digitCount - 5]) >= 2)
                        {
                            tempDigit = "mốt ";
                        }
                    }
                    else if (num == 5 && digitCount > 4)//5 的時候 萬位有值lăm
                    {
                        tempDigit = "lăm ";
                    }
                }
                else if (digitCount - i == 5)//萬位數
                {
                    if (num == 1)//只要顯示Unit
                    {
                        tempDigit = "";
                    }
                    else if (num == 0 && digitCount > 5)  //前後位數!=0 且萬位數 == 0
                    {
                        tempDigit = "";
                        tempUnit = "";
                        if (valStr[digitCount - 6] != '0' && valStr[digitCount - 4] != '0')//百位數 and 個位數 不為零
                        {
                            tempDigit = digitVN[0];
                        }
                    }

                }
                else if (digitCount - i == 6)//十萬位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                    }
                }
                else if (digitCount - i == 7)//百萬位數
                {
                    if (num == 0)
                    {
                        tempDigit = "";

                        if (digitCount > 7)
                        {
                            if ((int)Char.GetNumericValue(valStr[digitCount - 8]) == 0 && (int)Char.GetNumericValue(valStr[digitCount - 9]) == 0)
                            {
                                tempUnit = "";
                            }
                        }
                    }
                    else if (num == 1 && digitCount > 7)//1 的時候 千萬位值 != 1
                    {
                        if ((int)Char.GetNumericValue(valStr[digitCount - 8]) >= 2)
                        {
                            tempDigit = "mốt ";
                        }
                    }
                }
                else if (digitCount - i == 8)//千萬位數
                {
                    if (num == 1)
                    {
                        tempDigit = "";
                    }
                    else if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                        if (digitCount > 8)
                        {
                            if (valStr[digitCount - 9] != '0' && valStr[digitCount - 7] != '0')//百位數 and 個位數 不為零
                            {
                                tempDigit = "linh ";
                            }
                        }
                    }
                }
                else if (digitCount - i == 9)//億
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                    }
                }
                else if (digitCount - i == 10)//十億
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                    }
                    if (digitCount > 10)
                    {
                        if (num == 1 && (int)Char.GetNumericValue(valStr[digitCount - 11]) > 1)//1 的時候
                        {
                            tempDigit = "mốt ";
                        }
                        else if (num == 5)
                        {
                            tempDigit = "trăm ";
                        }
                    }

                }
                else if (digitCount - i == 11)//百億
                {
                    if (num == 1)
                    {
                        tempDigit = "";
                    }
                    else if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                        if (digitCount > 11)
                        {
                            if (valStr[digitCount - 12] != '0' && valStr[digitCount - 10] != '0')//百億位數 and 億位數 不為零
                            {
                                tempDigit = "lÎ ";
                            }
                        }
                    }
                }
                else if (digitCount - i == 12)//千億
                {
                    if (num == 0)
                    {
                        tempDigit = "";
                        tempUnit = "";
                    }
                }
                result.Add(tempDigit + tempUnit);
            }
            string strResult = "";
            foreach (var item in result)
            {
                strResult += item;
            }
            return FirstCharToUpper(strResult);
        }

        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}

public enum HeaderViewRenderMode { Full, Title }