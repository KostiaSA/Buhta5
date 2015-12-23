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

        public static MvcHtmlString bsTagEnd(this HtmlHelper helper, string tag="div")
        {
            return new MvcHtmlString("</"+ tag + ">");
        }

    }

}
