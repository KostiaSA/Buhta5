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
            return new MvcHtmlString(Settings.GetHtml());
        }

    }

    public delegate string get_string();

    public enum bsButtonStyle { Default, Primary, Success, Info, Warning, Danger, Link }
    public enum bsButtonSize { Default, Large, Small, ExtraSmall }

    public class bsButton : bsControlSettings
    {
        public bsButton(BaseModel model) : base(model) { }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";

        public void Bind_Text(string modelPropertyName)
        {
            AddBinder(new NewBaseBinder<string>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "text"
            });
        }

        public void Bind_Text(NewBinderGetMethod<string> getValueMethod)
        {
            AddBinder(new NewBaseBinder<string>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "text"
            });
        }


        public string Image;

        public string OnClick_Bind;

        public BaseAction ClickAction;

        public bsButtonStyle ButtonStyle = bsButtonStyle.Default;
        public bsButtonSize Size = bsButtonSize.Default;

        public override string GetHtml()
        {
            EmitBinders(Script);

            EmitEvent_Bind(Script, OnClick_Bind, "click");

            if (ClickAction != null)
            {
                Script.AppendLine("tag.on('click',function(event){");
                ClickAction.EmitJsCode(Script);
                Script.AppendLine("});");

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


            Html.Append("<div id='" + UniqueId + "' " + GetAttrs() + ">" + imageHtml + Text + "</div>");

            return base.GetHtml();
        }
    }


}
