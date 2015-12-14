using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {

        public static MvcHtmlString bsAppNavBarButton(this HtmlHelper helper, Action<bsAppNavBarButton> settings)
        {
            var Settings = new bsAppNavBarButton(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }


    public class bsAppNavBarButton : bsControl
    {
        public bsAppNavBarButton(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";

        public bsAppNavBar NavBar;

        public void Bind_Text(string modelPropertyName)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "text"
            });
        }

        public void Bind_Text(BinderGetMethod<string> getValueMethod)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "text"
            });
        }

        public void Bind_Disabled(string modelPropertyName)
        {
            AddBinder(new OneWayBinder<bool>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "prop",
                jsSetPropertyName = "disabled"
            });
        }

        public void Bind_Disabled(BinderGetMethod<bool> getValueMethod)
        {
            AddBinder(new OneWayBinder<bool>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "prop",
                jsSetPropertyName = "disabled"
            });
        }

        public void Bind_OnClick(string modelEventMethodName)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "click"
            });
        }

        public void Bind_OnClick(BinderEventMethod eventMethod)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "click"
            });
        }


        public string Image;

        //public string OnClick_Bind;

        public BaseAction ClickAction;

        public override string GetHtml()
        {
            if (ClickAction != null)
            {
                Script.AppendLine("tag.on('click',function(event){");
                ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

            }

            var imageHtml = "";
            if (Image != null)
                imageHtml = "<img src=" + new UrlHelper(HttpContext.Current.Request.RequestContext).Content(Image).AsJavaScript() + " width='30' height='30'/>";

            Html.Append("<a id='" + UniqueId + "' " + GetAttrs() + " href=javascript:void(0)'>" + imageHtml + "<p>" + Text.AsHtmlEx() + "</p></a>");

            return base.GetHtml();
        }
    }


}
