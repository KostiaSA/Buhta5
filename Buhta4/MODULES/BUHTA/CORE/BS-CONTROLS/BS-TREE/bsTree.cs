using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString bsTree(this HtmlHelper helper, Action<bsTree> settings)
        {
            var Settings = new bsTree(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            //            var script = new StringBuilder();
            //            var html = new StringBuilder();
            return new MvcHtmlString(Settings.GetHtml());

            //          return new MvcHtmlString(Settings.EmitScriptAndHtml(script, html));
        }

    }

    public enum bsTreeSize { Default, Large, Small, ExtraSmall }

    public class bsTreeSessionState : BaseSessionStateObject
    {
        [JsonIgnore]
        public bsTree Tree;

        public HashSet<string> ExpandedNodes = new HashSet<string>();

        public override void Save()
        {

            base.Save();
        }

    }

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
        public bool IsExpandAllNodes;

        public string SessionStateId;
        public bool IsPersistNodeExpanded; // todo - пока не работает
        public bool IsPersistNodeSelected; // todo - пока не работает

        public BaseAction ClickAction;

        //public string OnRowClick_Bind;
        //public string OnRowDoubleClick_Bind;
        //public string OnRowActivate_Bind;

        //public string SelectedRows_Bind;

        public bsTreeSize Size = bsTreeSize.Default;

        //public BaseBsTreeDataSourceBinder DataSourceBinder;

        List<bsTreeColumn> columns = new List<bsTreeColumn>();
        List<bsControl> leftToolbar = new List<bsControl>();
        List<bsControl> rightToolbar = new List<bsControl>();
        public List<bsTreeColumn> Columns { get { return columns; } }


        public bsTreeSessionState sessionStateObject;
        public void SaveSessionState()
        {
            if (SessionStateId != null)
            {
                var stateObject = new bsTreeSessionState();
                stateObject.Id = SessionStateId;
                stateObject.Tree = this;
                stateObject.Save();
            }
        }

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

        bsTreeDataSourceToObjectListBinder dataSourceBinderToObjectList;
        public void Bind_DataSource_To_ObjectList(string datasourceModelPropertyName, string displayFieldName, string keyFieldName, string parentFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName = null)
        {
            dataSourceBinderToObjectList = new bsTreeDataSourceToObjectListBinder()
            {
                Tree = this,
                DatasourceModelPropertyName = datasourceModelPropertyName,
                DisplayFieldName = displayFieldName,
                KeyFieldName = keyFieldName,
                ParentFieldName = parentFieldName,
                IconFieldName = iconFieldName,
                SelectedRowsModelPropertyName = selectedRowsModelPropertyName

            };
            AddBinder(dataSourceBinderToObjectList);
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


        public void AddColumn(Action<bsTreeColumn> settings)
        {
            var col = new bsTreeColumn(Model);
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


        void EmitFilterScript(StringBuilder script)
        {
            script.Append(@"
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
 $('#" + UniqueId + @"').fancytree('getTree').filter_match=match;
 $('#" + UniqueId + @"').fancytree('getTree').filterNodes(match, opts);
}).focus();");
        }


        // при загрузке сохраняются и восстанавливаются состояния раскрытых и выбранных нод
        public string GetLoadDataScript(JsArray data, bool isFlatData)
        {
            string flatToTreeConvertFunctionName = "buhta.FancyTree.convertFlatDataToTree";
            if (!isFlatData)
                flatToTreeConvertFunctionName = "";

            return @"
(function(){

var expanded_state={};
var selected_state={};
var active_state={};

var rootNode=$('#" + UniqueId + @"').fancytree('getRootNode');

if (rootNode)
  rootNode.visit(function(node){
     expanded_state[node.key]=node.isExpanded();
     selected_state[node.key]=node.isSelected();
     active_state[node.key]=node.isActive();
  });

$('#" + UniqueId + @"').fancytree('getTree').clearFilter();

$('#" + UniqueId + @"').fancytree('option','source'," + flatToTreeConvertFunctionName + "(" + data.ToJson() + @"));

rootNode=$('#" + UniqueId + @"').fancytree('getRootNode');

rootNode.visit(function(node){
     if (expanded_state[node.key]!=undefined)
        node.setExpanded(expanded_state[node.key]);
     if (selected_state[node.key]!=undefined)
        node.setSelected(selected_state[node.key]);
     if (active_state[node.key]!=undefined)
        node.setActive(active_state[node.key]);
});

var match=$('#" + UniqueId + @"').fancytree('getTree').filter_match;
if (match) {
  var opts = {
   autoExpand: true,
   leavesOnly: true
  };
  $('#" + UniqueId + @"').fancytree('getTree').filterNodes(match, opts);
}

})();
";

        }

        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {
            if (SessionStateId != null)
                sessionStateObject = AppServer.GetStateObject<bsTreeSessionState>(SessionStateId);

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

            init.AddProperty("activeVisible", true);
            init.AddProperty("debugLevel", 0);
            init.AddProperty("keyboard", true);

            //ObservableCollection<string> selectedRows = null;
            //if (SelectedRows_Bind != null)
            //{
            //    selectedRows = Model.GetPropertyValue<ObservableCollection<string>>(SelectedRows_Bind);
            //    init.AddProperty("checkbox", true);

            //    script.AppendLine("var " + SelectedRows_Bind + "=function(event, data) {");
            //    script.AppendLine("  bindingHub.server.sendSelectedRowsChanged('" + Model.BindingId + "', '" + SelectedRows_Bind + "', data.node.key, data.node.isSelected()); ");
            //    script.AppendLine("}");

            //    init.AddRawProperty("select", SelectedRows_Bind);

            //}

            script.AppendLine("var renderColumns=function(event, data) {");
            script.AppendLine("  var node = data.node;");
            script.AppendLine("  var row = node.data.row || {};");
            script.AppendLine("  row.node = node;");
            script.AppendLine("  var td = $(node.tr).find('>td');");
            int colIndex = -1;
            foreach (var col in Columns.Where(c => c.Hidden != true))
            {
                colIndex++;

                if (colIndex != 0)
                    script.AppendLine("  var td_tag=td.eq(" + colIndex + ");");
                else
                    script.AppendLine("  var td_tag=td.eq(" + colIndex + ").find('.fancytree-title');");


                if (col.CellTemplate != null)
                {
                    script.AppendLine(@"  var f" + colIndex + "=function(row){");
                    if (col.CellTemplateJS != null)
                        script.AppendLine(col.CellTemplateJS);
                    script.AppendLine(@"    return Mustache.render(""" + col.CellTemplate.Replace("\n", "").Replace(@"""", @"\""") + @""", row);");
                    script.AppendLine(@"  };");
                    if (colIndex != 0)
                        script.AppendLine("  td_tag.html(f" + colIndex + "(row));");
                    else
                        // node.saveHtml ипользуется в jquery.fancytree.filter.js
                        script.AppendLine("  td_tag.html(f" + colIndex + "(row)); node.saveHtml=f" + colIndex + "(row);");
                }
                else
                if (col.EditableType != bsEditableType.None)
                {
                    var edt = new bsTreeEditable(Model);
                    edt.Tag = "small";
                    edt.Bind_Value<string>(col.Field_Bind);
                    var edt_html = new StringBuilder();
                    var edt_script = new StringBuilder();
                    edt.EmitScriptAndHtml(edt_script, edt_html);

                    script.AppendLine("  td_tag.html(" + edt_html.AsJavaScript() + ".replace('4D57BEAC0040F92312A4',row['" + col.Field_Bind + "']).replace('"+ edt.UniqueId + "',node.key.replace('.','-')));");

                    script.AppendLine("  setTimeout(function(){ " + edt_script.ToString() + " }, 100);");


                }
                else
                {
                    script.AppendLine("  td_tag.text(row['" + col.Field_Bind + "']);");
                }

                //if (col.TextColorClass != null)
                //    script.AppendLine("  td_tag.addClass('" + col.TextColorClass + "');");
                //if (col.BackColorClass != null)
                //    script.AppendLine("  td_tag.addClass('" + col.BackColorClass + "');");

                foreach (string cls in col.Classes)
                    script.AppendLine("  td_tag.addClass('" + cls + "');");

                foreach (var style in col.Styles)
                    script.AppendLine("  td_tag.css('" + style.Key + "','" + style.Value + "');");

                if (col.Align == bsTreeColumnAlign.center)
                    script.AppendLine("  td_tag.css('text-align','center');");
                if (col.Align == bsTreeColumnAlign.right)
                    script.AppendLine("  td_tag.css('text-align','right');");


            }
            script.AppendLine("}");
            init.AddRawProperty("renderColumns", "renderColumns");

            script.AppendLine("$('#" + UniqueId + "').fancytree(" + init.ToJson() + ");");

            if (ClickAction != null)
            {
                script.AppendLine("$('#" + UniqueId + "').on('click',function(event){");
                ClickAction.EmitJsCode(script);
                script.AppendLine("});");

            }

            // toolbar
            if (IsShowTextFilter)
                EmitFilterScript(script);

            if (rightToolbar.Count > 0 || leftToolbar.Count > 0 || IsShowTextFilter)
            {
                html.Append("<div class='row'>");  // begin row

                html.Append(@"<form class='form-inline'>");

                if (leftToolbar.Count > 0 || IsShowTextFilter)
                {
                    html.Append(@"<div class='form-group col-xs-12 col-md-6' style='margin-bottom:10px; padding-left:0px'>");

                    if (IsShowTextFilter)
                        html.Append(@"<input id='" + UniqueId + @"-filter-input' type='text' class='form-control input-sm' placeholder='строка для поиска' style='max-width:150px; margin-left1:-15px; display:inline-block'>");

                    foreach (var control in leftToolbar)
                        html.Append(control.GetHtml());

                    html.Append(@"</div>");
                }

                if (rightToolbar.Count > 0)
                {
                    html.Append("<div class='form-group col-xs-12 col-md-6' style='margin-bottom:10px; padding-right:0px'>");
                    html.Append("<div class='pull-right'>");
                    foreach (var control in rightToolbar)
                        html.Append(control.GetHtml());
                    html.Append("</div>");
                    html.Append("</div>");
                }
                html.Append("</div>");  // end row
                html.Append("</form>");
            }

            html.Append("<div class='row'>");  // begin row
            html.Append("<table id='" + UniqueId + "' " + GetAttrs() + ">");
            //html.Append("<colgroup>");
            //foreach (var col in Columns)
            //    col.EmitColgroupCol(Html, Script);
            //html.Append("</colgroup>");

            html.Append("<thead>");
            html.Append("<tr>");
            foreach (var col in Columns.Where(c => c.Hidden != true))
                html.Append("<th>" + col.Caption + "</th>");
            html.Append("</tr>");
            html.Append("</thead>");

            html.Append("<tbody>");
            html.Append("<tr>");
            foreach (var col in Columns.Where(c => c.Hidden != true))
                html.Append("<td></td>");
            html.Append("</tr>");
            html.Append("</tbody>");

            html.Append("</table>");
            html.Append("</div>");  // end row

            base.EmitScriptAndHtml(script, html);
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
