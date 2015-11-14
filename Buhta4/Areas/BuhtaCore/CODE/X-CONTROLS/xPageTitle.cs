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
        public static MvcHtmlString xPageTitle(this HtmlHelper helper, xPageTitleSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xPageTitle(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xPageTitle(this HtmlHelper helper, Action<xPageTitleSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xPageTitle(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public class xPageTitleSettings : xControlSettings
    {
        public string Text;
        public string Text_Bind;
    }

    public class xPageTitle : xControl<xPageTitleSettings>
    {

        public xPageTitle(object model, xPageTitleSettings settings) : base(model, settings) { }
        public xPageTitle(object model, Action<xPageTitleSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            EmitBeginScript(Script, true);

            EmitProperty_M(Script, "text", Settings.Text);
            EmitProperty_Bind_M(Script, Settings.Text_Bind, "text");

            Html.Append("<div class='x-page-title' id='" + UniqueId + "'/>");

            return base.GetHtml();
        }

    }
}
