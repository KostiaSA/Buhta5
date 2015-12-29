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

        public static MvcHtmlString bsTag(this HtmlHelper helper, Action<bsTagBegin> settings)
        {
            var Settings = new bsTag(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            //var script = new StringBuilder();
            //var html = new StringBuilder();

            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public class bsTag : bsTagBegin
    {
        public bsTag(BaseModel model) : base(model) { }

        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {
            base.EmitScriptAndHtml(script, html);
            html.Append("</" + Tag + ">");
        }
    }


}
