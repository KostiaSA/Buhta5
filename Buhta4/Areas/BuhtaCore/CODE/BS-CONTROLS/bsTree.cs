using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        //public static MvcHtmlString bsTree(this HtmlHelper helper, bsTree settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsTree(helper.ViewData.Model, settings).GetHtml());
        //}

        //public static MvcHtmlString bsTree(this HtmlHelper helper, Action<bsTree> settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsTree(helper.ViewData.Model, settings).GetHtml());
        //}

        public static MvcHtmlString bsTree1(this HtmlHelper helper, Action<bsTree> settings)
        {
            var Settings = new bsTree(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public enum bsTreeSize { Default, Large, Small, ExtraSmall }

    public class bsTree : bsControlSettings
    {
        public bsTree(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;

        //public string OnClick_Bind;

        public BaseAction ClickAction;

        public string OnRowClick_Bind;
        public string OnRowDoubleClick_Bind;
        public string OnRowSelect_Bind;

        public bsTreeSize Size = bsTreeSize.Default;

        public BaseBsTreeDataSourceBinder DataSourceBinder;

        List<bsTreeColumnSettings> columns = new List<bsTreeColumnSettings>();
        public List<bsTreeColumnSettings> Columns { get { return columns; } }

        public void AddColumn(Action<bsTreeColumnSettings> settings)
        {
            var col = new bsTreeColumnSettings(Model);
            settings(col);
            columns.Add(col);
        }

        public void EmitRowEvent_BindFunction(string modelMethodName)
        {
            Script.AppendLine("var " + modelMethodName + "=function(event, data) {");
            Script.AppendLine("  if (!data.node.children) {");
            Script.AppendLine("    var _args={rowId:data.node.key, tagId:event.target.id};");
            Script.AppendLine("    bindingHub.server.sendEvent('" + Model.BindingId + "','" + modelMethodName + "', _args );");
            Script.AppendLine("  }");
            Script.AppendLine("}");
        }

        public override string GetHtml()
        {

            AddClass("table");

            JsObject init = new JsObject();
            DataSourceBinder.Tree = this;
            init.AddRawProperty("source", DataSourceBinder.GetJsonDataTreeSource(Model));

            jsArray extensions = new jsArray();
            extensions.AddObject("table");
            init.AddProperty("extensions", extensions);

            JsObject table = new JsObject();
            table.AddProperty("indentation", 20);
            table.AddProperty("nodeColumnIdx", 0);
            init.AddProperty("table", table);

            if (OnRowDoubleClick_Bind != null)
            {
                EmitRowEvent_BindFunction(OnRowDoubleClick_Bind);
                init.AddRawProperty("dblclick", OnRowDoubleClick_Bind);
            }


            //var sb = new StringBuilder(); // renderColumns
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

            Script.AppendLine("tag.fancytree(" + init.ToJson() + ");");

            //EmitProperty(Script, "disabled", Settings.Disabled);
            //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


            //EmitProperty_M(Script, "val", Settings.Text);
            EmitProperty_Bind_M(Script, Text_Bind, "val");

            //EmitEvent_Bind(Script, OnClick_Bind, "click");

            if (ClickAction != null)
            {
                Script.AppendLine("tag.on('click',function(event){");
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
