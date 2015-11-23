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
        public static MvcHtmlString bsTree(this HtmlHelper helper, bsTree settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString("");// new bsTree(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString bsTree(this HtmlHelper helper, Action<bsTree> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString("");// new bsTree(helper.ViewData.Model, settings).GetHtml());
        }

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

        public string OnClick_Bind;

        public BaseAction ClickAction;

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

        public override string GetHtml()
        {

            AddClass("table");
            //var list = new List<string>();
            //list.Add("жопа-1");
            //list.Add("жопа-2");
            //list.Add("жопа-3");

            //Script.AppendLine("var tag =$('#" + UniqueId + "');");

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

            EmitEvent_Bind(Script, OnClick_Bind, "click");

            if (ClickAction != null)
            {
                Script.AppendLine("tag.on('click',function(event){");
                ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

            }

            //AddClass("btn");

            //if (Settings.Size == bsTreeSize.Large)
            //    AddClass("btn-lg");
            //else
            //if (Settings.Size == bsTreeSize.Small)
            //    AddClass("btn-sm");
            //else
            //if (Settings.Size == bsTreeSize.ExtraSmall)
            //    AddClass("btn-xs");


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


    //class treeNode
    //{
    //    public string title;
    //    public bool expanded;
    //    public string key;
    //}

    //public class bsTree : bsControl<bsTreeSettings>
    //{

    //    public bsTree(object model, bsTreeSettings settings) : base(model, settings) { }
    //    public bsTree(object model, Action<bsTreeSettings> settings) : base(model, settings) { }


    //    //private List<treeNode> GetTreeSourceFromListOfStrings(List<string> list)
    //    //{
    //    //    var ret = new List<treeNode>();
    //    //    foreach (var str in list)
    //    //    {
    //    //        var node = new treeNode();
    //    //        node.title = str;
    //    //        ret.Add(node);
    //    //    }
    //    //    return ret;

    //    //}

    //    public override string GetHtml()
    //    {
    //        //var list = new List<string>();
    //        //list.Add("жопа-1");
    //        //list.Add("жопа-2");
    //        //list.Add("жопа-3");

    //        //Script.AppendLine("var tag =$('#" + UniqueId + "');");

    //        JsObject init = new JsObject();
    //        init.AddRawProperty("source", Settings.DataSourceBinder.GetJsonDataTreeSource(Model));

    //        //var xx = Json.Encode(init);
    //        //Debug.Write(xx);

    //        Script.AppendLine("tag.fancytree(" + init.ToJson() + ");");

    //        //EmitProperty(Script, "disabled", Settings.Disabled);
    //        //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


    //        //EmitProperty_M(Script, "val", Settings.Text);
    //        EmitProperty_Bind_M(Script, Settings.Text_Bind, "val");

    //        EmitEvent_Bind(Script, Settings.OnClick_Bind, "click");

    //        if (Settings.ClickAction != null)
    //        {
    //            Script.AppendLine("tag.on('click',function(event){");
    //            Settings.ClickAction.EmitJsCode(Script);
    //            Script.AppendLine("});");

    //        }

    //        //AddClass("btn");

    //        //if (Settings.Size == bsTreeSize.Large)
    //        //    AddClass("btn-lg");
    //        //else
    //        //if (Settings.Size == bsTreeSize.Small)
    //        //    AddClass("btn-sm");
    //        //else
    //        //if (Settings.Size == bsTreeSize.ExtraSmall)
    //        //    AddClass("btn-xs");


    //        Html.Append("<table id='" + Settings.UniqueId + "' " + Settings.GetAttrs() + ">");
    //        Html.Append("<colgroup>");
    //        foreach (var col in Settings.Columns)
    //            col.EmitColgroupCol(Html, Script);
    //        Html.Append("</colgroup>");


    //        Html.Append("</table>");

    //        return base.GetHtml();
    //    }

    //}
}
