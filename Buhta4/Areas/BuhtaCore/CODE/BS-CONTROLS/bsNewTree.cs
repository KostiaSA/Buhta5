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
        public static MvcHtmlString bsNewTree(this HtmlHelper helper, Action<bsNewTree> settings)
        {
            var Settings = new bsNewTree(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public enum bsNewTreeSize { Default, Large, Small, ExtraSmall }

    public class bsNewTree : bsControl
    {
        public bsNewTree(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;


        public BaseAction ClickAction;

        public string OnRowClick_Bind;
        public string OnRowDoubleClick_Bind;
        public string OnRowActivate_Bind;

        public string SelectedRows_Bind;

        public bsNewTreeSize Size = bsNewTreeSize.Default;

        //public BaseBsTreeDataSourceBinder DataSourceBinder;

        List<bsTreeColumnSettings> columns = new List<bsTreeColumnSettings>();
        public List<bsTreeColumnSettings> Columns { get { return columns; } }


        bsTreeDataSourceToSqlDataViewBinder dataSourceBinderToSqlDataView;
        public void Bind_DataSource_To_SqlDataView(string datasourceModelPropertyName, string displayFieldName, string keyFieldName, string parentFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName=null)
        {
            dataSourceBinderToSqlDataView = new bsTreeDataSourceToSqlDataViewBinder()
            {
                Tree = this,
                DatasourceModelPropertyName = datasourceModelPropertyName,
                DisplayFieldName = displayFieldName,
                KeyFieldName = keyFieldName,
                ParentFieldName = parentFieldName,
                IconFieldName = iconFieldName,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToSqlDataView);
        }

        bsTreeDataSourceToSchemaRolesBinder dataSourceBinderToSchemaRoles;
        public void Bind_DataSource_To_ToSchemaRoles(string selectedRowsModelPropertyName = null)
        {
            dataSourceBinderToSchemaRoles = new bsTreeDataSourceToSchemaRolesBinder()
            {
                Tree = this,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToSchemaRoles);
        }


        public void AddColumn(Action<bsTreeColumnSettings> settings)
        {
            var col = new bsTreeColumnSettings(Model);
            settings(col);
            columns.Add(col);
        }

        public void EmitRowEvent_BindFunction(string modelMethodName, bool isIgnoreForFolder = false)
        {
            Script.AppendLine("var " + modelMethodName + "=function(event, data) {");
            if (isIgnoreForFolder)
                Script.AppendLine("  if (!data.node.children) {");
            Script.AppendLine("    var _args={rowId:data.node.key, tagId:event.target.id};");
            Script.AppendLine("    bindingHub.server.sendEvent('" + Model.BindingId + "','" + modelMethodName + "', _args );");
            if (isIgnoreForFolder)
                Script.AppendLine("  }");
            Script.AppendLine("}");
        }

        public override string GetHtml()
        {

            AddClass("table");

            JsObject init = new JsObject();
            //DataSourceBinder.Tree = this;

            jsArray extensions = new jsArray();
            extensions.AddObject("table");
            init.AddProperty("extensions", extensions);

            JsObject table = new JsObject();
            table.AddProperty("indentation", 20);
            table.AddProperty("nodeColumnIdx", 0);
            init.AddProperty("table", table);


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

            if (OnRowDoubleClick_Bind != null)
            {
                EmitRowEvent_BindFunction(OnRowDoubleClick_Bind, true);
                init.AddRawProperty("dblclick", OnRowDoubleClick_Bind);
            }


            if (OnRowActivate_Bind != null)
            {
                EmitRowEvent_BindFunction(OnRowActivate_Bind);
                init.AddRawProperty("activate", OnRowActivate_Bind);
            }


            Script.AppendLine("var renderColumns=function(event, data) {");
            Script.AppendLine("  var node = data.node;");
            Script.AppendLine("  var row = node.data.row;");
            Script.AppendLine("  row.node = node;");
            Script.AppendLine("  var td = $(node.tr).find('>td');");
            int i = -1;
            foreach (var col in Columns.Where(c => c.Hidden != true))
            {
                i++;
                if (col.CellTemplate != null)
                {
                    Script.AppendLine(@"  var f" + i + "=function(row){");
                    if (col.CellTemplateJS != null)
                        Script.AppendLine(col.CellTemplateJS);
                    Script.AppendLine(@"    return Mustache.render(""" + col.CellTemplate + @""", row);");
                    Script.AppendLine(@"  };");
                    if (i != 0)
                        Script.AppendLine("  td.eq(" + i + ").html(f" + i + "(row));");
                    else
                        Script.AppendLine("  td.eq(" + i + ").find('.fancytree-title').html(f" + i + "(row));");
                }
                else
                {
                    if (i != 0)
                        Script.AppendLine("  td.eq(" + i + ").text(row['" + col.Field_Bind + "']);");
                    else
                        Script.AppendLine("  td.eq(" + i + ").find('.fancytree-title').text(row['" + col.Field_Bind + "']);");
                }

            }
            Script.AppendLine("}");
            init.AddRawProperty("renderColumns", "renderColumns");

            //init.AddRawProperty("source", DataSourceBinder.GetJsonDataTreeSource(Model, selectedRows));

            Script.AppendLine("$('#" + UniqueId + "').fancytree(" + init.ToJson() + ");");



            //EmitProperty_Bind_M(Script, Text_Bind, "val");


            if (ClickAction != null)
            {
                Script.AppendLine("$('#" + UniqueId + "').on('click',function(event){");
                ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

            }

            Html.Append("<table id='" + UniqueId + "' " + GetAttrs() + ">");
            //Html.Append("<colgroup>");
            //foreach (var col in Columns)
            //    col.EmitColgroupCol(Html, Script);
            //Html.Append("</colgroup>");

            Html.Append("<thead>");
            Html.Append("<tr>");
            foreach (var col in Columns.Where(c => c.Hidden != true))
                Html.Append("<th>" + col.Caption + "</th>");
            Html.Append("</tr>");
            Html.Append("</thead>");

            Html.Append("<tbody>");
            Html.Append("<tr>");
            foreach (var col in Columns.Where(c => c.Hidden != true))
                Html.Append("<td></td>");
            Html.Append("</tr>");
            Html.Append("</tbody>");

            Html.Append("</table>");

            return base.GetHtml();
        }

    }

}
