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

        public static MvcHtmlString bsTagBegin(this HtmlHelper helper, Action<bsTagBegin> settings)
        {
            var Settings = new bsTagBegin(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public class bsTagBegin : bsControl
    {
        public bsTagBegin(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Tag = "div";

        //public void Bind_Text(string modelPropertyName)
        //{
        //    AddBinder(new OneWayBinder<string>()
        //    {
        //        ModelPropertyName = modelPropertyName,
        //        jsSetMethodName = "text"
        //    });
        //}

        //public void Bind_Text(BinderGetMethod<string> getValueMethod)
        //{
        //    AddBinder(new OneWayBinder<string>()
        //    {
        //        ModelGetMethod = getValueMethod,
        //        jsSetMethodName = "text"
        //    });
        //}

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

        public override string GetHtml()
        {
            Html.Append("<" + Tag + " id='" + UniqueId + "' " + GetAttrs() + ">");
            return base.GetHtml();
        }
    }


}
