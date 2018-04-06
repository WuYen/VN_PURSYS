using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace ERP_V2.Utility
{
    public class ReadExcel
    {
        public static List<FieldInfo> FieldInfoList = GetFieldInfoList("D:\\WorkSpace\\WEBERP\\ERP_V2\\App_Data\\FieldInfo.xlsx");

        public static string GetFieldLabel(string table, string field)
        {
            var label = "";
            var fieldInfo = FieldInfoList.FirstOrDefault(x => x.TableName == table && x.Field == field);
            if (fieldInfo != null)
            {
                switch (UserInfo.LanguageType)//vi-VN,zh-TW,en-US
                {
                    case Language.Type.TW:
                        label = fieldInfo.Label_TW;
                        break;
                    case Language.Type.VN:
                        label = fieldInfo.Label_VN;
                        break;
                    case Language.Type.EN:
                        label = fieldInfo.Label_EN;
                        break;
                }              
            }
            return label;
        }

        private static List<FieldInfo> GetFieldInfoList(string filePath)
        {
            var list = new List<FieldInfo>();

            XSSFWorkbook workbook;
            //讀取專案內中的sample.xls 的excel 檔案
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))//AppDomain.CurrentDomain.BaseDirectory + "sample.xls"
            {
                workbook = new XSSFWorkbook(file);
            }

            //讀取Sheet1 工作表
            var sheet = workbook.GetSheet("Info");

            for (int rowCount = 1; rowCount <= sheet.LastRowNum; rowCount++)
            {
                if (sheet.GetRow(rowCount) != null) //null is when the row only contains empty cells 
                {
                    var info = new FieldInfo();

                    info.TableName = sheet.GetRow(rowCount).Cells[0].StringCellValue;
                    info.Field = sheet.GetRow(rowCount).Cells[1].StringCellValue;
                    info.Type = sheet.GetRow(rowCount).Cells[2].StringCellValue;
                    info.Size = sheet.GetRow(rowCount).Cells[3].NumericCellValue;
                    info.Require = sheet.GetRow(rowCount).Cells[4].StringCellValue;
                    info.Label_CN = sheet.GetRow(rowCount).Cells[5].StringCellValue;
                    info.Label_TW = sheet.GetRow(rowCount).Cells[6].StringCellValue;
                    info.Label_EN = sheet.GetRow(rowCount).Cells[7].StringCellValue;
                    info.Label_VN = sheet.GetRow(rowCount).Cells[8].StringCellValue;
                    list.Add(info);

                }
            }
            return list;
        }
    }

    public class FieldInfo
    {
        public string TableName { get; set; }
        public string Field { get; set; }
        public string Type { get; set; }
        public double Size { get; set; }
        public string Require { get; set; }
        public string Label_CN { get; set; }
        public string Label_TW { get; set; }
        public string Label_EN { get; set; }
        public string Label_VN { get; set; }

    }
}