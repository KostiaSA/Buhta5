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

        public static MvcHtmlString bsSpan(this HtmlHelper helper, Action<bsSpan> settings)
        {
            var tag = new bsSpan(helper.ViewData.Model as BaseModel);
            settings(tag);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            //var script = new StringBuilder();
            //var html = new StringBuilder();

            return new MvcHtmlString(tag.GetHtml());
        }

        public static MvcHtmlString bsSpan(this HtmlHelper helper, BinderGetMethod<string> getTextMethod)
        {
            var tag = new bsSpan(helper.ViewData.Model as BaseModel);
            tag.Bind_Text(getTextMethod);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            var script = new StringBuilder();
            var html = new StringBuilder();

            return new MvcHtmlString(tag.GetHtml());
        }

    }

    public class bsSpan : bsTag
    {
        public bsSpan(BaseModel model) : base(model) { Tag = "span"; }
    }


}
