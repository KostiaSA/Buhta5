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
        //public static MvcHtmlString bsButton(this HtmlHelper helper, bsButtonSettings settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsButton(helper.ViewData.Model, settings).GetHtml());
        //}

        //public static MvcHtmlString bsButton(this HtmlHelper helper, Action<bsButtonSettings> settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsButton(helper.ViewData.Model, settings).GetHtml());
        //}

        public static MvcHtmlString bsButton(this HtmlHelper helper, Action<bsButton> settings)
        {
            var Settings = new bsButton(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            var script = new StringBuilder();
            var html = new StringBuilder();

            return new MvcHtmlString(Settings.GetHtml(script, html));
        }

    }

    public delegate string get_string();

    public enum bsButtonStyle { Default, Primary, Success, Info, Warning, Danger, Link }
    public enum bsButtonSize { Default, Large, Small, ExtraSmall }

    public class bsButton : bsControl
    {
        public bsButton(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";

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
                jsSetPropertyName="disabled"
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

        public bsButtonStyle ButtonStyle = bsButtonStyle.Default;
        public bsButtonSize Size = bsButtonSize.Default;

        public override string GetHtml(StringBuilder script, StringBuilder html)
        {
            //EmitBinders(Script);

            //EmitEvent_Bind(Script, OnClick_Bind, "click");

            if (ClickAction != null)
            {
                script.AppendLine("tag.on('click',function(event){");
                ClickAction.EmitJsCode(script);
                script.AppendLine("});");

            }

            AddClass("btn");
            AddClass("btn-" + ButtonStyle.ToNameString().ToLower());


            if (Size == bsButtonSize.Large)
                AddClass("btn-lg");
            else
            if (Size == bsButtonSize.Small)
                AddClass("btn-sm");
            else
            if (Size == bsButtonSize.ExtraSmall)
                AddClass("btn-xs");

            var imageHtml = "";
            if (Image != null)
            {
                imageHtml = "<img src=" + Image.AsJavaScriptString(); // + " width='15' height='14' style='margin-right: 5px; margin-left:-3px;'/>";

                if (Size == bsButtonSize.Large)
                    imageHtml += " width='20' height='20' style='margin-right: 8px; margin-left:-3px;margin-top:-2px'/>";
                else
                if (Size == bsButtonSize.Small)
                    imageHtml += " width='13' height='13' style='margin-right: 5px; margin-left:-3px;'/>";
                else
                if (Size == bsButtonSize.ExtraSmall)
                    imageHtml += " width='12' height='12' style='margin-right: 4px; margin-left:0px;margin-top:-2px'/>";
                else
                    imageHtml += " width='15' height='15' style='margin-right: 6px; margin-left:-3px;margin-top:-1px'/>";
            }


            html.Append("<button type='button' id='" + UniqueId + "' " + GetAttrs() + ">" + imageHtml + Text.AsHtmlEx() + "</button>");

            return base.GetHtml(script, html);
        }
    }


}
