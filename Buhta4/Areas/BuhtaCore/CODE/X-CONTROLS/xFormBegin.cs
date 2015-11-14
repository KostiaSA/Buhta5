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
        public static MvcHtmlString xFormBegin(this HtmlHelper helper, xFormBeginSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormBegin(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormBegin(this HtmlHelper helper, Action<xFormBeginSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormBegin(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormBegin(this HtmlHelper helper)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormBegin(helper.ViewData.Model, new xFormBeginSettings()).GetHtml());
        }
    }

    public class xFormBeginSettings : xControlSettings
    {

    }

    public class xFormBegin : xControl<xFormBeginSettings>
    {

        public xFormBegin(object model, xFormBeginSettings settings) : base(model, settings) { }
        public xFormBegin(object model, Action<xFormBeginSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            //EmitBeginScript(Script);

            Html.Append("<table class='x-form' id='" + UniqueId + "' " + Settings.GetClassAttr() + Settings.GetStyleAttr() + ">");

            return base.GetHtml();
        }

    }
}
