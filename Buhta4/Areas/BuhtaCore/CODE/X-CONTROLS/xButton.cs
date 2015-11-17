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
        public static MvcHtmlString xButton(this HtmlHelper helper, xButtonSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xButton(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xButton(this HtmlHelper helper, Action<xButtonSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xButton(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public class xButtonSettings : xControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public int? Width;
        public string Width_Bind;

        public int? Height;
        public string Height_Bind;

        public string Text;
        public string Text_Bind;

        public string OnClick_Bind;

        //public event xControlOnClickEventHandler<xButton> OnClick;

        //public string FireOnOnClick(xButton sender)
        //{
        //    if (OnClick != null)
        //        return OnClick(sender);
        //    else
        //        return null;
        //}

    }

    public class xButton : xControl<xButtonSettings>
    {
        public override string GetJqxName()
        {
            return "jqxButton";
        }

        public xButton(object model, xButtonSettings settings) : base(model, settings) { }
        public xButton(object model, Action<xButtonSettings> settings) : base(model, settings) { }



        //public xButton(xButtonSettings settings)
        //{
        //    Settings = settings;
        //}

        //public xButton(Action<xButtonSettings> settings)
        //{
        //    Settings = new xButtonSettings();
        //    settings(Settings);
        //}

        public override string GetHtml()
        {
            EmitBeginScript(Script);

            EmitProperty(Script, "width", Settings.Width);
            EmitProperty_Bind(Script, Settings.Width_Bind, "width");

            EmitProperty(Script, "height", Settings.Height);
            EmitProperty_Bind(Script, Settings.Height_Bind, "height");

            EmitProperty(Script, "disabled", Settings.Disabled);
            EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


            EmitProperty_M(Script, "val", Settings.Text);
            EmitProperty_Bind_M(Script, Settings.Text_Bind, "val");

            EmitEvent_Bind(Script, Settings.OnClick_Bind, "click");

            Html.Append("<input type='button'  id='" + UniqueId + "' " + Settings.GetClassAttr() + Settings.GetStyleAttr() + "/>");

            return base.GetHtml();
        }

    }
}
