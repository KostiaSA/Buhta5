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
        public static MvcHtmlString xFormButtonsRowEnd(this HtmlHelper helper, xFormButtonsRowEndSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowEnd(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormButtonsRowEnd(this HtmlHelper helper, Action<xFormButtonsRowEndSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowEnd(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormButtonsRowEnd(this HtmlHelper helper)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowEnd(helper.ViewData.Model, new xFormButtonsRowEndSettings()).GetHtml());
        }

    }

    public class xFormButtonsRowEndSettings : xControlSettings
    {

    }

    public class xFormButtonsRowEnd : xControl<xFormButtonsRowEndSettings>
    {

        public xFormButtonsRowEnd(object model, xFormButtonsRowEndSettings settings) : base(model, settings) { }
        public xFormButtonsRowEnd(object model, Action<xFormButtonsRowEndSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            //EmitBeginScript(Script);

            Html.Append("</div></td></tr>");
            return base.GetHtml();
        }

    }
}
