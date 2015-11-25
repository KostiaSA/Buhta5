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
        public bsButton(BaseModel model) : base(model)
        {
            New_Text_Binder = new NewBaseBinder<string>()
            {
                Control = this,
                Model = Model,
                jQueryMethodName = "text"

            };

        }

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;

        //        public get_string Text_Get;

        public NewBaseBinder<string> New_Text_Binder;

        //public NewBinderGetMethod<string> New_Text_BindGet;
        //public string New_Text_BindToProperty;

        public string Image;

        public string OnClick_Bind;

        public BaseAction ClickAction;

        public bsButtonStyle ButtonStyle = bsButtonStyle.Default;
        public bsButtonSize Size = bsButtonSize.Default;

        public override string GetHtml()
        {

            //if (New_Text_Binder != null || New_Text_BindToProperty != null || New_Text_BindGet != null)
            //{
            //    if (New_Text_Binder == null)
            //    {
            //        New_Text_Binder = new NewBaseBinder<string>()
            //        {
            //            PropertyName = New_Text_BindToProperty,
            //            GetMethod = New_Text_BindGet

            //        };
            //    }
            //    New_Text_Binder.Model = Model;
            //    New_Text_Binder.jQueryMethodName = "text";
            //}

            New_Text_Binder.EmitBindingScript_M(Script);

            EmitProperty_Bind_M(Script, Text_Bind, "val");

            //EmitProperty_StringBinder(Script, new StringBinder(Text_Get), "text");

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
