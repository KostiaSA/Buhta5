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
        public static MvcHtmlString xFormButtonsRowBegin(this HtmlHelper helper, xFormButtonsRowBeginSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowBegin(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormButtonsRowBegin(this HtmlHelper helper, Action<xFormButtonsRowBeginSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowBegin(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormButtonsRowBegin(this HtmlHelper helper)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormButtonsRowBegin(helper.ViewData.Model, new xFormButtonsRowBeginSettings()).GetHtml());
        }

    }

    public class xFormButtonsRowBeginSettings : xControlSettings
    {

    }

    public class xFormButtonsRowBegin : xControl<xFormButtonsRowBeginSettings>
    {

        public xFormButtonsRowBegin(object model, xFormButtonsRowBeginSettings settings) : base(model, settings) { }
        public xFormButtonsRowBegin(object model, Action<xFormButtonsRowBeginSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            //EmitBeginScript(Script);

            Html.Append("<tr><td colspan='10'><div class='x-form-buttons-row' " + Settings.GetClassAttr() + Settings.GetStyleAttr() + ">");
            return base.GetHtml();
        }

    }
}
