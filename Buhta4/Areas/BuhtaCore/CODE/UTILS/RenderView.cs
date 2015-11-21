using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Buhta
{
    public class R
    {
        /// <summary>Renders a view to string.</summary>
        public static string RenderViewToString(Controller controller, string viewName, object viewData)
        {
            //Create memory writer
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            //Create fake http context to render the view
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                controller.ControllerContext.RouteData,
                controller.ControllerContext.Controller);

            var oldContext = HttpContext.Current;
            HttpContext.Current = fakeContext;

            var html = new HtmlHelper(
                new ViewContext(fakeControllerContext, new FakeView(),
                    new ViewDataDictionary(), new TempDataDictionary(), memWriter),
                new ViewPage());
            html.RenderPartial(viewName, viewData);
            //Restore context
            HttpContext.Current = oldContext;

            //Flush memory and return output
            memWriter.Flush();
            return sb.ToString();
        }

    }
}
/// <summary>Fake IView implementation used to instantiate an HtmlHelper.</summary>
public class FakeView : IView
{
    #region IView Members

    public void Render(ViewContext viewContext, System.IO.TextWriter writer)
    {
        throw new NotImplementedException();
    }

    #endregion
}