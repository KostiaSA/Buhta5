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
            return new MvcHtmlString(tag.GetHtml());
        }

        public static MvcHtmlString bsSpan(this HtmlHelper helper, BinderGetMethod<string> getTextMethod)
        {
            var tag = new bsSpan(helper.ViewData.Model as BaseModel);
            tag.Bind_Text(getTextMethod);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(tag.GetHtml());
        }

    }

    public class bsSpan : bsControl
    {
        public bsSpan(BaseModel model) : base(model) { }

        public string Text = "";
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

        public override string GetHtml()
        {
            Html.Append("<span id='" + UniqueId + "' " + GetAttrs() + ">" + Text + "</span>");
            return base.GetHtml();
        }
    }


}
