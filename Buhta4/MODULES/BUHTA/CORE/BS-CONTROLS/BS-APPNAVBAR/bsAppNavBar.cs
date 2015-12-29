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

        public static MvcHtmlString bsAppNavBar(this HtmlHelper helper, Action<bsAppNavBar> settings)
        {
            var tag = new bsAppNavBar(helper.ViewData.Model as BaseModel);
            settings(tag);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            //var script = new StringBuilder();
            //var html = new StringBuilder();
            return new MvcHtmlString(tag.GetHtml());
        }

    }

    public class bsAppNavBar : bsControl
    {
        public bsAppNavBar(BaseModel model) : base(model) { }

        List<bsControl> leftToolbar = new List<bsControl>();
        List<bsControl> rightToolbar = new List<bsControl>();

        public void AddButtonToRightToolbar(Action<bsAppNavBarButton> settings)
        {
            var button = new bsAppNavBarButton(Model);
            button.NavBar = this;
            settings(button);
            rightToolbar.Add(button);
        }

        public void AddButtonToLeftToolbar(Action<bsAppNavBarButton> settings)
        {
            var button = new bsAppNavBarButton(Model);
            button.NavBar = this;
            settings(button);
            leftToolbar.Add(button);
        }

        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {
            html.Append("<nav id='" + UniqueId + "' " + GetAttrs() + " class='navbar navbar-default'>");
            html.Append("<div class='container' style='padding-left:0px;padding-right:0px'>");
            html.Append("<div class='collapse navbar-collapse'>");

            html.Append("<ul class='nav navbar-nav app-navbar'>");

            foreach (var control in leftToolbar)
            {
                html.Append("<li>");
                html.Append(control.GetHtml());
                html.Append("</li>");
            }
            html.Append("</ul>");

            html.Append("<ul class='nav navbar-nav app-navbar pull-right'>");
            foreach (var control in rightToolbar)
            {
                html.Append("<li>");
                html.Append(control.GetHtml());
                html.Append("</li>");
            }
            html.Append("</ul>");

            html.Append("</div>");
            html.Append("</div>");
            html.Append("</nav>");

            if (Model is AppNavBarModel)
                Model.Update();

            base.EmitScriptAndHtml(script, html);
        }
    }


}
