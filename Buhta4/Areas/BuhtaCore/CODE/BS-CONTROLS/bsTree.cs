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
        public static MvcHtmlString bsTree(this HtmlHelper helper, Action<bsTree> settings)
        {
            var Settings = new bsTree(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public enum bsTreeSize { Default, Large, Small, ExtraSmall }


    public class bsTree : bsControl
    {
        public bsTree(BaseModel model) : base(model) { }

        //public bool? Disabled;
        //public string Disabled_Bind;

        //public string Text = "";
        //public string Text_Bind;

        public bool IsShowCheckboxes;
        public bool IsShowIcons;
        public bool IsShowTextFilter;

        public BaseAction ClickAction;

        //public string OnRowClick_Bind;
        //public string OnRowDoubleClick_Bind;
        //public string OnRowActivate_Bind;

        //public string SelectedRows_Bind;

        public bsTreeSize Size = bsTreeSize.Default;

        //public BaseBsTreeDataSourceBinder DataSourceBinder;

        List<bsTreeColumnSettings> columns = new List<bsTreeColumnSettings>();
        List<bsControl> leftToolbar = new List<bsControl>();
        List<bsControl> rightToolbar = new List<bsControl>();
        public List<bsTreeColumnSettings> Columns { get { return columns; } }


        bsTreeDataSourceToSqlDataViewBinder dataSourceBinderToSqlDataView;
        public void Bind_DataSource_To_SqlDataView(string datasourceModelPropertyName, string displayFieldName, string keyFieldName, string parentFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName = null)
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
        public void Bind_DataSource_To_ToSchemaRoles(string selectedRowsModelPropertyName, SchemaBaseRole rootRole)
        {
            dataSourceBinderToSchemaRoles = new bsTreeDataSourceToSchemaRolesBinder()
            {
                Tree = this,
                RootRole = rootRole,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToSchemaRoles);
            if (selectedRowsModelPropertyName != null)
                IsShowCheckboxes = true;
        }

        bsTreeDataSourceToSchemaObjectsBinder dataSourceBinderToSchemaObjects;
        public void Bind_DataSource_To_SchemaObjects(string selectedObjectModelPropertyName, Type schemaObjectType)
        {
            dataSourceBinderToSchemaObjects = new bsTreeDataSourceToSchemaObjectsBinder()
            {
                Tree = this,
                SchemaObjectType = schemaObjectType,
                selectedObjectModelPropertyName = selectedObjectModelPropertyName

            };
            AddBinder(dataSourceBinderToSchemaObjects);
            //if (selectedRowsModelPropertyName != null)
            IsShowCheckboxes = false;
        }


        public void AddColumn(Action<bsTreeColumnSettings> settings)
        {
            var col = new bsTreeColumnSettings(Model);
            settings(col);
            columns.Add(col);
        }

        public void AddButtonToRightToolbar(Action<bsTreeToolbarButton> settings)
        {
            var button = new bsTreeToolbarButton(Model);
            button.Tree = this;
            settings(button);
            rightToolbar.Add(button);
        }

        public void AddButtonToLeftToolbar(Action<bsTreeToolbarButton> settings)
        {
            var button = new bsTreeToolbarButton(Model);
            button.Tree = this;
            settings(button);
            leftToolbar.Add(button);
        }

        public void Bind_OnRowClick(string modelEventMethodName)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "click"
            });
        }

        public void Bind_OnRowClick(BinderEventMethod eventMethod)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "click"
            });
        }

        public void Bind_OnRowDblClick(string modelEventMethodName, Boolean isIgnoreForFolder = true)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                isIgnoreForFolder = isIgnoreForFolder,
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "dblclick"
            });
        }

        public void Bind_OnRowDblClick(BinderEventMethod eventMethod, Boolean isIgnoreForFolder = true)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                isIgnoreForFolder = isIgnoreForFolder,
                ModelEventMethod = eventMethod,
                jsEventName = "dblclick"
            });
        }

        public void Bind_OnRowSelect(string modelEventMethodName)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "select"
            });
        }

        public void Bind_OnRowSelect(BinderEventMethod eventMethod)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "select"
            });
        }

        public void Bind_OnRowActivate(BinderEventMethod eventMethod)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "activate"
            });
        }

        public void Bind_OnRowDeactivate(BinderEventMethod eventMethod)
        {
            AddBinder(new bsTreeRowEventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "deactivate"
            });
        }


        void EmitFilterScript()
        {
            Script.Append(@"
$('#" + UniqueId + @"-filter-input').keyup(function(e){
 var opts = {
  autoExpand: true,
  leavesOnly: true
 };
 var match = $(this).val();
 if (e && e.which === 27 || $.trim(match) === ''){
    $('#" + UniqueId + @"').fancytree('getTree').clearFilter();
    return;
 }
 $('#" + UniqueId + @"').fancytree('getTree').filterNodes(match, opts);
}).focus();");
        }

        public override string GetHtml()
        {

            AddClass("table");

            JsObject init = new JsObject();

            JsArray extensions = new JsArray();
            extensions.AddObject("table");
            extensions.AddObject("filter");
            init.AddProperty("extensions", extensions);

            JsObject filter = new JsObject();
            filter.AddProperty("mode", "hide");
            filter.AddProperty("counter", "false");
            filter.AddProperty("hideExpandedCounter", "false");
            init.AddProperty("filter", filter);

            JsObject table = new JsObject();
            table.AddProperty("indentation", 20);
            table.AddProperty("nodeColumnIdx", 0);
            init.AddProperty("table", table);

            if (IsShowCheckboxes)
                init.AddProperty("checkbox", true);
            init.AddProperty("icon", IsShowIcons);


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

            Script.AppendLine("var renderColumns=function(event, data) {");
            Script.AppendLine("  var node = data.node;");
            Script.AppendLine("  var row = node.data.row || {};");
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
                        // node.saveHtml ипользуется в jquery.fancytree.filter.js
                        Script.AppendLine("  td.eq(" + i + ").find('.fancytree-title').html(f" + i + "(row)); node.saveHtml=f" + i + "(row);");
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

            Script.AppendLine("$('#" + UniqueId + "').fancytree(" + init.ToJson() + ");");

            if (ClickAction != null)
            {
                Script.AppendLine("$('#" + UniqueId + "').on('click',function(event){");
                ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

            }

            // toolbar
            if (IsShowTextFilter)
                EmitFilterScript();

            if (rightToolbar.Count > 0 || leftToolbar.Count > 0 || IsShowTextFilter)
            {
                Html.Append("<div class='row'>");  // begin row

                Html.Append(@"<form class='form-inline'>");

                if (leftToolbar.Count > 0 || IsShowTextFilter)
                {
                    Html.Append(@"<div class='form-group col-xs-12 col-md-6' style='margin-bottom:10px; padding-left:0px'>");

                    if (IsShowTextFilter)
                        Html.Append(@"<input id='" + UniqueId + @"-filter-input' type='text' class='form-control input-sm' placeholder='строка для поиска' style='max-width:150px; margin-left1:-15px; display:inline-block'>");

                    foreach (var control in leftToolbar)
                        Html.Append(control.GetHtml());

                    Html.Append(@"</div>");
                }

                if (rightToolbar.Count > 0)
                {
                    Html.Append("<div class='form-group col-xs-12 col-md-6' style='margin-bottom:10px; padding-right:0px'>");
                    Html.Append("<div class='pull-right'>");
                    foreach (var control in rightToolbar)
                        Html.Append(control.GetHtml());
                    Html.Append("</div>");
                    Html.Append("</div>");
                }
                Html.Append("</div>");  // end row
                Html.Append("</form>");
            }

            Html.Append("<div class='row'>");  // begin row
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
            Html.Append("</div>");  // end row

            return base.GetHtml();
        }

        public void JavaScript_ExpandAll(dynamic args)
        {
            Model.ExecuteJavaScript("$('#" + UniqueId + "').fancytree('getRootNode').visit(function(node){node.setExpanded(true);});");
        }

        public void JavaScript_CollapseAll(dynamic args)
        {
            Model.ExecuteJavaScript("$('#" + UniqueId + "').fancytree('getRootNode').visit(function(node){node.setExpanded(false);});");
        }
    }

}
