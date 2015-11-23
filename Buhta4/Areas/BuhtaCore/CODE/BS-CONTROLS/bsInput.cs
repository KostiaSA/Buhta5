﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        //public static MvcHtmlString bsInput(this HtmlHelper helper, bsInputSettings settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// ""<new bsInput(helper.ViewData.Model, settings).GetHtml());
        //}

        //public static MvcHtmlString bsInput(this HtmlHelper helper, Action<bsInputSettings> settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsInput(helper.ViewData.Model, settings).GetHtml());
        //}

        public static MvcHtmlString bsInput(this HtmlHelper helper, Action<bsInput> settings)
        {
            var Settings = new bsInput(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }
    }

    public enum bsInputSize { Default, Large, Small, ExtraSmall }

    public enum bsInputType { Text, Checkbox, Radio }

    public class bsInput : bsControlSettings
    {
        public bsInput(BaseModel model) : base(model) { }

        public Type ValueType = typeof(String);

        public bsInputType Type = bsInputType.Text;

        public bool? Disabled;
        public string Disabled_Bind;

        public string Value = "";
        public string Value_Bind;
        public BaseBinder Value_Binder;

        public string Label;
        public string PlaceHolder;

        public string Image;

        public BaseBinder Lookup;


        public bsInputSize Size = bsInputSize.Default;

        string GetDisplayText(object value)
        {
            if (Lookup != null)
                return Lookup.GetDisplayText(value);
            else
                return value.ToString();
        }

        object ParseDisplayText(string text)
        {
            if (Lookup != null)
                return Lookup.ParseDisplayText(text);
            else
            {
                if (ValueType == typeof(String))
                    return text;
                else
                if (ValueType == typeof(int))
                    return int.Parse(text);
                else
                if (ValueType == typeof(Decimal))
                    return Decimal.Parse(text);
                else
                    return nameof(ParseDisplayText) + ": неизвестный тип '" + ValueType.FullName + "'";
            }
        }

        public override string GetHtml()
        {


            //EmitProperty(Script, "disabled", Disabled);
            //EmitProperty_Bind(Script, Disabled_Bind, "disabled");


            //EmitProperty_M(Script, "val", Text);


            if (Size == bsInputSize.Large)
                AddClass("input-lg");
            else
            if (Size == bsInputSize.Small)
                AddClass("input-sm");
            else
            if (Size == bsInputSize.ExtraSmall)
                AddClass("input-xs");

            if (PlaceHolder != null)
                AddAttr("placeholder", PlaceHolder);


            Html.Append("<div class='form-group'>"); // begin form-group

            if (Type == bsInputType.Text)
            {

                EmitProperty_Bind2Way_M(Script, Value_Bind, "val", "change");
                AddClass("form-control");

                if (Label != null)
                {
                    //Html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    Html.Append("<label class='col-sm-3 control-label' >" + Label + "</label>");
                    //Html.Append("</div>");  // end col-sm-3
                    Html.Append("<div class='col-sm-9'>");  // begin col-sm-9
                }

                Html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + ">" + GetDisplayText(Value) + "</input>");

                if (Label != null)
                {
                    Html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Type == bsInputType.Checkbox)
            {
                EmitProperty_Bind2Way_Checked(Script, Value_Binder, "change");

                Html.Append("<div class='col-sm-offset-3 col-sm-9'>"); // beg col-sm-offset-3 col-sm-9
                Html.Append("<div class='checkbox'>");
                Html.Append("<label>");

                Html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + "></input>");
                Html.Append(Label != null ? Label.ToString() : "");

                Html.Append("</label>");
                Html.Append("</div>");
                Html.Append("</div>");  // end col-sm-offset-3 col-sm-9
            }


            Html.Append("</div>"); // end form-group

            return base.GetHtml();
        }
    }

}
