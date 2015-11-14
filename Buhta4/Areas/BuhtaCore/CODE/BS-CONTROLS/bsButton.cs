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

    public class bsButtonSettings : bsControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;

        public string OnClick_Bind;

        public BaseAction ClickAction;

        public bsButtonStyle ButtonStyle=bsButtonStyle.Default;
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

            if (Settings.ClickAction!=null)
            {
                Script.AppendLine("tag.on('click',function(event){");
                Settings.ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

            }

            Class.Append("btn ");
            Class.Append("btn-"+Settings.ButtonStyle.ToNameString().ToLower()+" ");

            if (Settings.ClassAttr != null)
                Class.Append(Settings.ClassAttr);

            if (Settings.StyleAttr != null)
                Style.Append(Settings.StyleAttr);

            Html.Append("<div id='" + UniqueId + "' " + GetClassAttr() + GetStyleAttr() + ">" + Settings.Text + "</div>");

            return base.GetHtml();
        }

    }
}
