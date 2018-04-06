using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace ERP_V2.Utility
{
    public static class GridViewSetting
    {
        public enum GridCustomButtonType { View, Edit, Delete };
        public enum GridEventType { Add, Update, Delete };
        public enum GridType { Master, Detail };
        public enum ColumnType { None, Date };

        /// <summary> 設定master 初始設定 </summary>
        /// <param name="setting"></param>
        /// <param name="name"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="keyFieldName"></param>
        /// <returns></returns>
        public static GridViewSettings GetMaster(GridViewSettings setting, string controller, string action, string keyFieldName)
        {
            GetDefault(setting, "MasterGrid", controller, action, keyFieldName);

            //setting.ClientSideEvents.EndCallback = "function(s, e) {DXFooterRow(); }";
            setting.Styles.Header.Wrap = DefaultBoolean.True;//header 字數過常會折行
            setting.SettingsBehavior.AllowEllipsisInText = true;//字數過長 用 ... 取代
            setting.CommandColumn.CellStyle.Paddings.Padding = 0;
            setting.CommandColumn.Width = 110;
            return setting;
        }

        /// <summary> 設定datail 初始設定 </summary>
        /// <param name="setting"></param>
        /// <param name="name"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="keyFieldName"></param>
        /// <returns></returns>
        public static GridViewSettings GetDatail(GridViewSettings setting, string controller, string action, string keyFieldName)
        {
            GetDefault(setting, "DetailGrid", controller, action, keyFieldName);
            //setting.SettingsBehavior.AllowEllipsisInText = true;//字數過長 用 ... 取代
            setting.Styles.Header.Wrap = DefaultBoolean.True;
            return setting;
        }

        /// <summary> 設定GridViewSettings  初始設定 </summary>
        /// <param name="setting"></param>
        /// <param name="name"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static GridViewSettings GetDefault(GridViewSettings setting, string name, string controller, string action, string keyFieldName)
        {
            if (setting == null)
            {
                setting = new GridViewSettings();
            }

            setting.Name = name;
            setting.Width = Unit.Percentage(100);
            setting.CallbackRouteValues = new { Controller = controller, Action = action };

            setting.SettingsPager.Visible = true;
            setting.Settings.ShowGroupPanel = false;
            setting.Settings.ShowFilterRow = false;
            //setting.SettingsText.EmptyDataRow = Resources.Resource1.GridNoData;
            //setting.SettingsText.ConfirmDelete = Resources.Resource1.ConfirmDelete;

            setting.KeyFieldName = keyFieldName;

            setting.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.Off;
            setting.SettingsAdaptivity.AdaptiveColumnPosition = GridViewAdaptiveColumnPosition.Right;
            setting.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
            setting.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = false;
            setting.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 0;

            setting.SettingsBehavior.AllowSelectByRowClick = true;





            return setting;
        }

        #region [ CommandColumn ]
        /// <summary> 設定CommandColumn 初始設定 </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static GridViewSettings GetCommandColumnDefault(GridViewSettings setting)
        {
            setting.CommandColumn.Visible = true;
            setting.CommandColumn.Caption = " ";

            setting.CommandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            setting.SettingsCommandButton.RenderMode = GridCommandButtonRenderMode.Button;

            return setting;
        }

        /// <summary> 新增btn 檢視/修改/刪除 </summary>
        /// <param name="setting"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GridViewSettings GetCustomButton(GridViewSettings setting, GridType gridType, GridCustomButtonType customButtonType)
        {
            var name = gridType == GridType.Master ? "Master" : "Detail";
            switch (customButtonType)
            {
                case GridCustomButtonType.View:
                    GetButton(setting, $"Btn{name}CmdView", "~/Content/Icon/ic_view_eye_black_24dp_1x.png");
                    break;
                case GridCustomButtonType.Edit:
                    GetButton(setting, $"Btn{name}CmdEdit", "~/Content/Icon/ic_mode_edit_black_24dp_1x.png");
                    break;
                case GridCustomButtonType.Delete:
                    GetButton(setting, $"Btn{name}CmdDelete", "~/Content/Icon/ic_delete_black_24dp_1x.png");
                    break;
            }
            return setting;
        }

        /// <summary> 新增CommandColumn 按鈕 </summary>
        /// <param name="setting"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static GridViewSettings GetButton(GridViewSettings setting, string name, string url)
        {
            GridViewCommandColumnCustomButton btn = new GridViewCommandColumnCustomButton();
            btn.ID = name;
            btn.Text = " ";
            btn.Image.Url = url;
            btn.Image.Width = 20;
            setting.CommandColumn.CustomButtons.Add(btn);
            return setting;
        }

        #endregion  [ CommandColumn ]

        /// <summary> 新增Column </summary>
        /// <param name="setting"></param>
        /// <param name="tb_NM"></param>
        /// <param name="col_NM"></param>
        /// <returns></returns>
        public static GridViewSettings GetColumn(GridViewSettings setting, string tb_NM, string col_NM, ColumnType type = ColumnType.None, int? width = null, bool isVisible = true)
        {
            setting.Columns.Add(column =>
            {
                column.FieldName = col_NM;
                var nm = col_NM;
                switch (type)
                {
                    case ColumnType.Date:
                        nm = col_NM.Replace("dt", "");
                        column.PropertiesEdit.DisplayFormatString = "yyyy/MM/dd";
                        break;
                }
                //column.Caption = DDMHelper.GetColName(tb_NM, nm);

                if (width.HasValue)
                {
                    column.Width = width.Value;
                }

                column.Visible = isVisible;
            });

            //字數太長 變成 ......
            if (width.HasValue)
            {
                setting.SettingsBehavior.AllowEllipsisInText = true;
            }

            return setting;
        }

        public static void GetBasicColumnProperty(ref MVCxGridViewColumn column, string tb_NM, string col_NM, ColumnType type = ColumnType.None, int? width = null, bool isVisible = true)
        {
            column.FieldName = col_NM;
            var nm = col_NM;
            switch (type)
            {
                case ColumnType.Date:
                    nm = col_NM.Replace("dt", "");
                    column.PropertiesEdit.DisplayFormatString = "yyyy/MM/dd";
                    break;
            }
            //column.Caption = DDMHelper.GetColName(tb_NM, nm);

            if (width.HasValue)
            {
                column.Width = width.Value;
            }

            column.Visible = isVisible;
        }


        /// <summary> 設定Button 事項 </summary>
        /// <param name="setting"></param>
        /// <param name="type"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static GridViewSettings GetEditRoute(GridViewSettings setting, GridEventType type, string controller, string action)
        {
            switch (type)
            {
                case GridEventType.Add:
                    setting.SettingsEditing.AddNewRowRouteValues = new { Controller = controller, Action = action };
                    break;
                case GridEventType.Update:
                    setting.SettingsEditing.UpdateRowRouteValues = new { Controller = controller, Action = action };
                    break;
                case GridEventType.Delete:
                    setting.SettingsEditing.DeleteRowRouteValues = new { Controller = controller, Action = action };
                    setting.SettingsBehavior.ConfirmDelete = true;
                    break;
            }

            return setting;
        }

        /// <summary> 設定 GridView SearchPanel </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static GridViewSettings GetSearchPanel(GridViewSettings setting)
        {
            setting.SettingsSearchPanel.Visible = false;
            setting.SettingsSearchPanel.ShowApplyButton = false;
            setting.SettingsSearchPanel.ShowClearButton = false;

            setting.SettingsCommandButton.SearchPanelApplyButton.RenderMode = GridCommandButtonRenderMode.Button;
            setting.SettingsCommandButton.SearchPanelClearButton.RenderMode = GridCommandButtonRenderMode.Button;

            return setting;
        }

        public static GridViewSettings GetFilterRow(GridViewSettings setting)
        {
            setting.Settings.ShowFilterRow = true;
            setting.SettingsBehavior.FilterRowMode = GridViewFilterRowMode.OnClick;
            setting.CommandColumn.ShowClearFilterButton = true;
            //setting.SettingsCommandButton.ClearFilterButton.Text = Resources.Resource1.Clear;
            setting.SettingsCommandButton.ClearFilterButton.Styles.Style.Paddings.PaddingLeft = 5;
            setting.SettingsCommandButton.ClearFilterButton.Styles.Style.Paddings.PaddingRight = 5;
            setting.CommandColumn.ShowApplyFilterButton = true;
            //setting.SettingsCommandButton.ApplyFilterButton.Text = Resources.Resource1.Filter;
            setting.SettingsCommandButton.ApplyFilterButton.Styles.Style.Paddings.PaddingLeft = 5;
            setting.SettingsCommandButton.ApplyFilterButton.Styles.Style.Paddings.PaddingRight = 5;
            setting.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            return setting;
        }

    }
}