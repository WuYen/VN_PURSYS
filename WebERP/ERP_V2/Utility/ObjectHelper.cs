using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ActWeis.Utility
{
    /// <summary>物件類公用函式</summary>
    public class ObjectHelper
    {
        /// <summary>複製物件內屬性(Property)的值</summary>
        /// <param name="source">來源物件</param>
        /// <param name="target">目標物件</param>
        /// <param name="exclusiveList">不處理的屬性名稱清單</param>
        public void CopyValue(object source, object target, List<string> exclusiveList = null)
        {
            DateTime dtTmp; //TryParse
            TimeSpan tsTmp; //TryParse
            string strTmp; //TryPasre

            if (source != null && target != null)
            {
                foreach (var prop in source.GetType().GetProperties())
                {
                    if (exclusiveList == null || !exclusiveList.Contains(prop.Name))
                    {
                        var property = target.GetType().GetProperty(prop.Name);
                        if (property != null && property.CanWrite)
                        {
                            var objValue = prop.GetValue(source, null); //來源值

                            var tarValue = property.GetValue(target, null); //目標現在值
                            if (objValue != null && tarValue != null && objValue.ToString() == tarValue.ToString()) //來源值與目標值相同則不處理
                            {
                                continue;
                            }

                            if ((property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                && prop.PropertyType == typeof(string)) //Target:DateTime DateTime?, Source:string
                            {
                                strTmp = (objValue ?? "").ToString();
                                if (strTmp.Length == 8)
                                {
                                    strTmp = strTmp.Insert(6, "-").Insert(4, "-");
                                }
                                if (DateTime.TryParse(strTmp, out dtTmp))
                                {
                                    property.SetValue(target, dtTmp, null);
                                }
                            }
                            else if (property.PropertyType == typeof(string)
                                && (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))) //Target:string, Source:DateTime DateTime?
                            {
                                var conValue = objValue as DateTime?;
                                property.SetValue(target, conValue.HasValue ? conValue.Value.ToString("yyyyMMdd") : "", null);
                            }
                            else if ((property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(TimeSpan?))
                                && prop.PropertyType == typeof(string)) //Target:TimeSpan TimeSpan?, Source:string
                            {
                                strTmp = (objValue ?? "").ToString();
                                if (TimeSpan.TryParse(strTmp, out tsTmp))
                                {
                                    property.SetValue(target, tsTmp, null);
                                }
                            }
                            else if (property.PropertyType.IsAssignableFrom(prop.PropertyType))
                            {
                                property.SetValue(target, objValue, null);
                            }
                            else
                            {
                                property.SetValue(target, null, null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>DataRow to Object</summary>
        /// <param name="dr">來源資料行</param>
        /// <param name="target">目標物件</param>
        /// <param name="exclusiveList">不處理的屬性名稱清單</param>
        public void CopyValue(DataRow dr, object target, List<string> exclusiveList = null)
        {
            DateTime dtTmp; //TryParse
            TimeSpan tsTmp; //TryParse
            string strTmp; //TryPasre

            if (dr != null && target != null)
            {
                foreach (DataColumn col in dr.Table.Columns)
                {
                    if (dr[col.ColumnName] != null && !Convert.IsDBNull(dr[col.ColumnName]))
                    {
                        if (exclusiveList == null || !exclusiveList.Contains(col.ColumnName))
                        {
                            var property = target.GetType().GetProperty(col.ColumnName);
                            if (property != null && property.CanWrite)
                            {
                                var tarValue = property.GetValue(target, null); //目標現在值
                                if (dr[col.ColumnName] != null && tarValue != null && dr[col.ColumnName].ToString() == tarValue.ToString()) //來源值與目標值相同則不處理
                                {
                                    continue;
                                }

                                if ((property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                    && col.DataType == typeof(string)) //Target:DateTime DateTime?, Source:string
                                {
                                    strTmp = dr[col.ColumnName].ToString();
                                    if (strTmp.Length == 8)
                                    {
                                        strTmp = strTmp.Insert(6, "-").Insert(4, "-");
                                    }
                                    if (DateTime.TryParse(strTmp, out dtTmp))
                                    {
                                        property.SetValue(target, dtTmp, null);
                                    }
                                }
                                else if (property.PropertyType == typeof(string)
                                    && (col.DataType == typeof(DateTime) || col.DataType == typeof(DateTime?))) //Target:string, Source:DateTime DateTime?
                                {
                                    if (DateTime.TryParse(dr[col.ColumnName].ToString(), out dtTmp))
                                    {
                                        property.SetValue(target, dtTmp.ToString("yyyyMMdd"), null);
                                    }
                                    else
                                    {
                                        property.SetValue(target, null, null);
                                    }
                                }
                                else if ((property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(TimeSpan?))
                                    && col.DataType == typeof(string)) //Target:TimeSpan TimeSpan?, Source:string
                                {
                                    strTmp = dr[col.ColumnName].ToString();
                                    if (TimeSpan.TryParse(strTmp, out tsTmp))
                                    {
                                        property.SetValue(target, tsTmp, null);
                                    }
                                    else
                                    {
                                        property.SetValue(target, null, null);
                                    }
                                }
                                else if (property.PropertyType.IsAssignableFrom(col.DataType))
                                {
                                    property.SetValue(target, dr[col.ColumnName], null);
                                }
                                else
                                {
                                    property.SetValue(target, null, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>清除Static Class的Value</summary>
        /// <param name="target"></param>
        /// <param name="bindingAttr"></param>
        public void Clear(Type target, BindingFlags bindingAttr)
        {
            if (target != null)
            {
                foreach (var prop in target.GetProperties(bindingAttr))
                {
                    if (prop.CanWrite)
                    {
                        prop.SetValue(prop, null, null);
                    }
                }
            }
        }

        /// <summary>decimal小數點以下為零, 則清除小數點以下的零</summary>
        /// <param name="data">物件</param>
        /// <param name="digits">小數點位數</param>
        /// <param name="exclusiveList">不處理的屬性名稱清單</param>
        public void DecimalClean(object data, int digits = 0, List<string> exclusiveList = null)
        {
            decimal dTmp; //TryParse

            decimal pow = (decimal)Math.Pow(10, digits);

            if (data != null)
            {
                if (data is DataRow)
                {
                    var dr = data as DataRow;
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        if (exclusiveList == null || !exclusiveList.Contains(dc.ColumnName))
                        {
                            if ((dc.DataType == typeof(decimal) || dc.DataType == typeof(decimal?))
                                && decimal.TryParse(dr[dc.ColumnName].ToString(), out dTmp) && ((dTmp * pow) % 1) == 0)
                            {
                                dr[dc.ColumnName] = Math.Round(dTmp, digits, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var prop in data.GetType().GetProperties())
                    {
                        if (exclusiveList == null || !exclusiveList.Contains(prop.Name))
                        {
                            if (prop != null && prop.CanWrite)
                            {
                                var objValue = prop.GetValue(data, null);
                                if ((prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                    && decimal.TryParse(objValue.ToString(), out dTmp) && ((dTmp * pow) % 1) == 0)
                                {
                                    prop.SetValue(data, Math.Round(dTmp, digits, MidpointRounding.AwayFromZero), null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
