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
        public static MvcHtmlString bsTree(this HtmlHelper helper, bsTreeSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsTree(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString bsTree(this HtmlHelper helper, Action<bsTreeSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsTree(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public enum bsTreeSize { Default, Large, Small, ExtraSmall }

    public class bsTreeSettings : bsControlSettings
    {
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
            var col = new bsTreeColumnSettings();
            settings(col);
            columns.Add(col);
        }

    }


    class treeNode
    {
        public string title;
        public bool expanded;
        public string key;
    }

    public class bsTree : bsControl<bsTreeSettings>
    {

        public bsTree(object model, bsTreeSettings settings) : base(model, settings) { }
        public bsTree(object model, Action<bsTreeSettings> settings) : base(model, settings) { }


        private List<treeNode> GetTreeSourceFromListOfStrings(List<string> list)
        {
            var ret = new List<treeNode>();
            foreach (var str in list)
            {
                var node = new treeNode();
                node.title = str;
                ret.Add(node);
            }
            return ret;

        }

        public override string GetHtml()
        {
            //var list = new List<string>();
            //list.Add("жопа-1");
            //list.Add("жопа-2");
            //list.Add("жопа-3");

            //Script.AppendLine("var tag =$('#" + UniqueId + "');");

            JsObject init = new JsObject();
            init.AddRawProperty("source", Settings.DataSourceBinder.GetJsonDataTreeSource(Model));

            //var xx = Json.Encode(init);
            //Debug.Write(xx);

            Script.AppendLine("tag.fancytree(" + init.ToJson() + ");");

            //EmitProperty(Script, "disabled", Settings.Disabled);
            //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


            //EmitProperty_M(Script, "val", Settings.Text);
            EmitProperty_Bind_M(Script, Settings.Text_Bind, "val");

            EmitEvent_Bind(Script, Settings.OnClick_Bind, "click");

            if (Settings.ClickAction != null)
            {
                Script.AppendLine("tag.on('click',function(event){");
                Settings.ClickAction.EmitJsCode(Script);
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


            Html.Append("<div id='" + UniqueId + "' " + GetAttrs() + "></div>");

            return base.GetHtml();
        }

    }
}
