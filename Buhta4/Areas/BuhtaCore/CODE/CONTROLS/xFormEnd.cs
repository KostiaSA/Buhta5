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
        public static MvcHtmlString xFormEnd(this HtmlHelper helper, xFormEndSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormEnd(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormEnd(this HtmlHelper helper, Action<xFormEndSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormEnd(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormEnd(this HtmlHelper helper)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormEnd(helper.ViewData.Model, new xFormEndSettings()).GetHtml());
        }
    }

    public class xFormEndSettings : xControlSettings
    {

    }

    public class xFormEnd : xControl<xFormEndSettings>
    {

        public xFormEnd(object model, xFormEndSettings settings) : base(model, settings) { }
        public xFormEnd(object model, Action<xFormEndSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            //EmitBeginScript(Script);

            Html.Append("</table>");

            return base.GetHtml();
        }

    }
}
