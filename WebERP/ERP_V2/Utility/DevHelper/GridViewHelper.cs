using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Web.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ERP_V2.Utility
{
    public class GridViewHelper
    {
        /// <summary>Grid Basic setting</summary>
        /// <param name="settings">GridViewSettings</param>
        /// <param name="name">Extension name</param>
        /// <param name="controller">controller name</param>
        /// <param name="action">action name</param>
        public static void GetBasicSetting(GridViewSettings settings, string name, string controller, string action)
        {
            settings.Name = name;
            settings.CallbackRouteValues = new { Controller = controller, Action = action };
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsBehavior.AllowFocusedRow = true;

            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderTop.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);

            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollableHeight = 770;

            settings.SettingsPager.PageSize = 50;
            settings.SettingsPager.NumericButtonCount = 8;//page bar show page number count

            settings.Styles.FocusedRow.BackColor = System.Drawing.Color.Gray;
        }

        public static void SetCommandColumn(GridViewSettings settings, bool visible)
        {
            settings.CommandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            settings.CommandColumn.Visible = visible;
            settings.CommandColumn.Width = 130;
            settings.CommandColumn.CellStyle.Paddings.Padding = 0;
            settings.CommandColumn.ShowNewButtonInHeader = false;
            settings.CommandColumn.ShowEditButton = false;
            settings.CommandColumn.ShowDeleteButton = false;
            settings.CommandColumn.Caption = "";
        }

        public static void SetSearchPanel(GridViewSettings settings, bool visible)
        {
            settings.SettingsSearchPanel.Visible = visible;
            settings.SettingsSearchPanel.ShowApplyButton = true;
            settings.SettingsSearchPanel.AllowTextInputTimer = false;
            settings.SettingsCommandButton.SearchPanelApplyButton.Image.Url = "~/Content/Icon/search_grey_18x18.png";
        }

        public static void SetEditRoute(GridViewSettings settings, string controller)
        {
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = controller, Action = "AddNew" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = controller, Action = "Update" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = controller, Action = "Delete" };
        }

        /// <summary> Add BtnCmdNew in Header、BtnCmdEdit、BtnCmdDelete 
        /// With ClientSideEvent CustomButtonClick() and AddNewClick()
        /// </summary>
        /// <param name="helper">this.Html</param>
        /// <param name="settings">GridViewSettings</param>
        public static void SetCommandButton(HtmlHelper helper, GridViewSettings settings)
        {
            settings.SettingsCommandButton.RenderMode = GridCommandButtonRenderMode.Button;
            settings.CommandColumn.SetHeaderCaptionTemplateContent(c =>
            {
                helper.DevExpress().Button(button =>
                {
                    button.Name = "BtnCmdNew";
                    button.Text = "New";
                    button.Styles.Style.Paddings.PaddingTop = 0;
                    button.Styles.Style.Paddings.PaddingBottom = 0;
                    button.Images.Image.Url = "~/Content/Icon/add_circle_outline_grey_18x18.png";
                    button.ClientSideEvents.Click = "AddNewClick";
                }).GetHtml();
            });

            GridViewCommandColumnCustomButton btnEdit = new GridViewCommandColumnCustomButton();
            btnEdit.ID = "BtnCmdEdit";
            btnEdit.Text = "Edit";
            btnEdit.Image.Url = "~/Content/Icon/edit_grey_18x18.png";
            btnEdit.Styles.Style.Paddings.Padding = 0;
            settings.CommandColumn.CustomButtons.Add(btnEdit);
            settings.CommandColumn.ButtonRenderMode = GridCommandButtonRenderMode.Button;

            GridViewCommandColumnCustomButton btnDelete = new GridViewCommandColumnCustomButton();
            btnDelete.ID = "BtnCmdDelete";
            btnDelete.Text = "Del";
            btnDelete.Image.Url = "~/Content/Icon/delete_grey_18x18.png";
            btnDelete.Styles.Style.Paddings.Padding = 0;
            settings.CommandColumn.CustomButtons.Add(btnDelete);
            settings.CommandColumn.ButtonRenderMode = GridCommandButtonRenderMode.Button;
        }


        public static void SetColumn(GridViewSettings settings, string fieldName, string Caption)
        {
            settings.Columns.Add(
                column =>
                {
                    column.FieldName = fieldName;
                    column.Caption = Caption;
                });
        }
    }
}