using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Collections.ObjectModel;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString bsGrid(this HtmlHelper helper, Action<bsGrid> settings)
        {
            var Settings = new bsGrid(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public enum bsGridSize { Default, Large, Small, ExtraSmall }

    public class bsGrid : bsControl
    {
        public bsGrid(BaseModel model) : base(model) { }

        //public bool? Disabled;
        //public string Disabled_Bind;

        //public string Text = "";
        //public string Text_Bind;

        public bool IsShowCheckboxes;
        public bool IsShowIcons;

        public BaseAction ClickAction;

        //public string OnRowClick_Bind;
        //public string OnRowDoubleClick_Bind;
        //public string OnRowActivate_Bind;

        //public string SelectedRows_Bind;

        public bsGridSize Size = bsGridSize.Default;

        //public BasebsGridDataSourceBinder DataSourceBinder;

        List<bsGridColumnSettings> columns = new List<bsGridColumnSettings>();
        public List<bsGridColumnSettings> Columns { get { return columns; } }


        bsGridDataSourceToSqlDataViewBinder dataSourceBinderToSqlDataView;
        public void Bind_DataSource_To_SqlDataView(string datasourceModelPropertyName, string keyFieldName = null, string parentFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName = null)
        {
            dataSourceBinderToSqlDataView = new bsGridDataSourceToSqlDataViewBinder()
            {
                Tree = this,
                DatasourceModelPropertyName = datasourceModelPropertyName,
                KeyFieldName = keyFieldName,
                ParentFieldName = parentFieldName,
                IconFieldName = iconFieldName,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToSqlDataView);
        }

        bsGridDataSourceToObjectsListBinder dataSourceBinderToObjectsList;
        public void Bind_DataSource_To_ObjectsList(string datasourceModelPropertyName, string keyFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName = null)
        {
            dataSourceBinderToObjectsList = new bsGridDataSourceToObjectsListBinder()
            {
                Tree = this,
                DatasourceModelPropertyName = datasourceModelPropertyName,
                KeyFieldName = keyFieldName,
                IconFieldName = iconFieldName,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToObjectsList);
        }


        public void AddColumn(Action<bsGridColumnSettings> settings)
        {
            var col = new bsGridColumnSettings(Model);
            settings(col);
            columns.Add(col);
        }

        public void Bind_OnRowClick(string modelEventMethodName)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "click"
            });
        }

        public void Bind_OnRowClick(BinderEventMethod eventMethod)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "click"
            });
        }

        public void Bind_OnRowDblClick(string modelEventMethodName, Boolean isIgnoreForFolder = true)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                isIgnoreForFolder = isIgnoreForFolder,
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "dblclick"
            });
        }

        public void Bind_OnRowDblClick(BinderEventMethod eventMethod, Boolean isIgnoreForFolder = true)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                isIgnoreForFolder = isIgnoreForFolder,
                ModelEventMethod = eventMethod,
                jsEventName = "dblclick"
            });
        }

        public void Bind_OnRowSelect(string modelEventMethodName)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "select"
            });
        }

        public void Bind_OnRowSelect(BinderEventMethod eventMethod)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "select"
            });
        }

        public void Bind_OnRowActivate(BinderEventMethod eventMethod)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "activate"
            });
        }

        public void Bind_OnRowDeactivate(BinderEventMethod eventMethod)
        {
            AddBinder(new bsGridRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "deactivate"
            });
        }

        public override string GetHtml()
        {

            AddClass("table");
            AddClass("table-bordered");

            JsObject init = new JsObject();
            init.AddProperty("paging", false);
            init.AddProperty("select", true);

            //jsArray extensions = new jsArray();
            //extensions.AddObject("table");
            //init.AddProperty("extensions", extensions);

            jsArray columns = new jsArray();
            foreach (var col in Columns)
            {
                var jscol = new JsObject();
                jscol.AddProperty("title", col.Caption);
                columns.AddObject(jscol);
            }
            init.AddProperty("columns", columns);


            //if (IsShowCheckboxes)
            //    init.AddProperty("checkbox", true);
            //init.AddProperty("icon", IsShowIcons);


            //ObservableCollection<string> selectedRows = null;
            //if (SelectedRows_Bind != null)
            //{
            //    selectedRows = Model.GetPropertyValue<ObservableCollection<string>>(SelectedRows_Bind);
            //    init.AddProperty("checkbox", true);

            //    Script.AppendLine("var " + SelectedRows_Bind + "=function(event, data) {");
            //    Script.AppendLine("  bindingHub.server.sendSelectedRowsChanged('" + Model.BindingId + "', '" + SelectedRows_Bind + "', data.node.key, data.node.isSelected()); ");
            //    Script.AppendLine("}");

            //    init.AddRawProperty("select", SelectedRows_Bind);

            //}

            //Script.AppendLine("var renderColumns=function(event, data) {");
            //Script.AppendLine("  var node = data.node;");
            //Script.AppendLine("  var row = node.data.row;");
            //Script.AppendLine("  row.node = node;");
            //Script.AppendLine("  var td = $(node.tr).find('>td');");
            //int i = -1;
            //foreach (var col in Columns.Where(c => c.Hidden != true))
            //{
            //    i++;
            //    if (col.CellTemplate != null)
            //    {
            //        Script.AppendLine(@"  var f" + i + "=function(row){");
            //        if (col.CellTemplateJS != null)
            //            Script.AppendLine(col.CellTemplateJS);
            //        Script.AppendLine(@"    return Mustache.render(""" + col.CellTemplate + @""", row);");
            //        Script.AppendLine(@"  };");
            //        if (i != 0)
            //            Script.AppendLine("  td.eq(" + i + ").html(f" + i + "(row));");
            //        else
            //            Script.AppendLine("  td.eq(" + i + ").find('.fancytree-title').html(f" + i + "(row));");
            //    }
            //    else
            //    {
            //        if (i != 0)
            //            Script.AppendLine("  td.eq(" + i + ").text(row['" + col.Field_Bind + "']);");
            //        else
            //            Script.AppendLine("  td.eq(" + i + ").find('.fancytree-title').text(row['" + col.Field_Bind + "']);");
            //    }

            //}
            //Script.AppendLine("}");
            //init.AddRawProperty("renderColumns", "renderColumns");

            Script.AppendLine("$('#" + UniqueId + "').DataTable(" + init.ToJson() + ");");

            //if (ClickAction != null)
            //{
            //    Script.AppendLine("$('#" + UniqueId + "').on('click',function(event){");
            //    ClickAction.EmitJsCode(Script);
            //    Script.AppendLine("});");

            //}

            Html.Append("<table id='" + UniqueId + "' " + GetAttrs() + ">");
            //Html.Append("<colgroup>");
            //foreach (var col in Columns)
            //    col.EmitColgroupCol(Html, Script);
            //Html.Append("</colgroup>");

            //Html.Append("<thead>");
            //Html.Append("<tr>");
            //foreach (var col in Columns.Where(c => c.Hidden != true))
            //    Html.Append("<th>" + col.Caption + "</th>");
            //Html.Append("</tr>");
            //Html.Append("</thead>");

            //Html.Append("<tbody>");
            //Html.Append("<tr>");
            //foreach (var col in Columns.Where(c => c.Hidden != true))
            //    Html.Append("<td></td>");
            //Html.Append("</tr>");
            //Html.Append("</tbody>");

            Html.Append("</table>");

            return base.GetHtml();
        }

    }

}
