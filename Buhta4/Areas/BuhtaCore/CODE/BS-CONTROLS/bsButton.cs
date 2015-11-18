using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString bsButton(this HtmlHelper helper, bsButtonSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsButton(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString bsButton(this HtmlHelper helper, Action<bsButtonSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsButton(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public enum bsButtonStyle { Default, Primary, Success, Info, Warning, Danger, Link }
    public enum bsButtonSize { Default, Large, Small, ExtraSmall }

    public class bsButtonSettings : bsControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;

        public string Image;

        public string OnClick_Bind;

        public BaseAction ClickAction;

        public bsButtonStyle ButtonStyle = bsButtonStyle.Default;
        public bsButtonSize Size = bsButtonSize.Default;
    }

    public class bsButton : bsControl<bsButtonSettings>
    {

        public bsButton(object model, bsButtonSettings settings) : base(model, settings) { }
        public bsButton(object model, Action<bsButtonSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            EmitBeginScript(Script);


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

            AddClass("btn");
            AddClass("btn-" + Settings.ButtonStyle.ToNameString().ToLower());


            if (Settings.Size == bsButtonSize.Large)
                AddClass("btn-lg");
            else
            if (Settings.Size == bsButtonSize.Small)
                AddClass("btn-sm");
            else
            if (Settings.Size == bsButtonSize.ExtraSmall)
                AddClass("btn-xs");

            var imageHtml = "";
            if (Settings.Image != null)
            {
                imageHtml = "<img src=" + Settings.Image.AsJavaScriptString(); // + " width='15' height='14' style='margin-right: 5px; margin-left:-3px;'/>";

                if (Settings.Size == bsButtonSize.Large)
                    imageHtml += " width='20' height='20' style='margin-right: 8px; margin-left:-3px;margin-top:-2px'/>";
                else
                if (Settings.Size == bsButtonSize.Small)
                    imageHtml += " width='13' height='13' style='margin-right: 5px; margin-left:-3px;'/>";
                else
                if (Settings.Size == bsButtonSize.ExtraSmall)
                    imageHtml += " width='12' height='12' style='margin-right: 4px; margin-left:0px;margin-top:-2px'/>";
                else
                    imageHtml += " width='15' height='15' style='margin-right: 6px; margin-left:-3px;margin-top:-1px'/>";
            }


            //if (Settings.ClassAttr != null)
            //    Class.Append(Settings.ClassAttr);

            //if (Settings.StyleAttr != null)
            //    Style.Append(Settings.StyleAttr);


            Html.Append("<div id='" + UniqueId + "' " + GetAttrs() + ">" + imageHtml + Settings.Text + "</div>");

            return base.GetHtml();
        }

    }
}
