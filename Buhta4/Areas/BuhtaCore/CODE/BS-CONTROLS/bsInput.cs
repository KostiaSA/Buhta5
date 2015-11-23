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
        public static MvcHtmlString bsInput(this HtmlHelper helper, bsInputSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsInput(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString bsInput(this HtmlHelper helper, Action<bsInputSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new bsInput(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public enum bsInputSize { Default, Large, Small, ExtraSmall }

    public enum bsInputType { Text, Checkbox, Radio }

    public class bsInputSettings : bsControlSettings
    {
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
    }

    public class bsInput : bsControl<bsInputSettings>
    {

        public bsInput(object model, bsInputSettings settings) : base(model, settings) { }
        public bsInput(object model, Action<bsInputSettings> settings) : base(model, settings) { }


        string GetDisplayText(object value)
        {
            if (Settings.Lookup != null)
                return Settings.Lookup.GetDisplayText(value);
            else
                return value.ToString();
        }

        object ParseDisplayText(string text)
        {
            if (Settings.Lookup != null)
                return Settings.Lookup.ParseDisplayText(text);
            else
            {
                if (Settings.ValueType == typeof(String))
                    return text;
                else
                if (Settings.ValueType == typeof(int))
                    return int.Parse(text);
                else
                if (Settings.ValueType == typeof(Decimal))
                    return Decimal.Parse(text);
                else
                    return nameof(ParseDisplayText) + ": неизвестный тип '" + Settings.ValueType.FullName + "'";
            }
        }

        public override string GetHtml()
        {
            EmitBeginScript(Script);


            //EmitProperty(Script, "disabled", Settings.Disabled);
            //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


            //EmitProperty_M(Script, "val", Settings.Text);


            if (Settings.Size == bsInputSize.Large)
                Settings.AddClass("input-lg");
            else
            if (Settings.Size == bsInputSize.Small)
                Settings.AddClass("input-sm");
            else
            if (Settings.Size == bsInputSize.ExtraSmall)
                Settings.AddClass("input-xs");

            if (Settings.PlaceHolder != null)
                Settings.AddAttr("placeholder", Settings.PlaceHolder);


            Html.Append("<div class='form-group'>"); // begin form-group

            if (Settings.Type == bsInputType.Text)
            {

                EmitProperty_Bind2Way_M(Script, Settings.Value_Bind, "val", "change");
                Settings.AddClass("form-control");

                if (Settings.Label != null)
                {
                    //Html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    Html.Append("<label class='col-sm-3 control-label' >" + Settings.Label + "</label>");
                    //Html.Append("</div>");  // end col-sm-3
                    Html.Append("<div class='col-sm-9'>");  // begin col-sm-9
                }

                Html.Append("<input id='" + Settings.UniqueId + "' type='" + Settings.Type.ToString().ToLower() + "' " + Settings.GetAttrs() + ">" + GetDisplayText(Settings.Value) + "</input>");

                if (Settings.Label != null)
                {
                    Html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Settings.Type == bsInputType.Checkbox)
            {
                EmitProperty_Bind2Way_Checked(Script, Settings.Value_Binder, "change");

                Html.Append("<div class='col-sm-offset-3 col-sm-9'>"); // beg col-sm-offset-3 col-sm-9
                Html.Append("<div class='checkbox'>");
                Html.Append("<label>");

                Html.Append("<input id='" + Settings.UniqueId + "' type='" + Settings.Type.ToString().ToLower() + "' " + Settings.GetAttrs() + "></input>");
                Html.Append(Settings.Label != null ? Settings.Label.ToString() : "");

                Html.Append("</label>");
                Html.Append("</div>");
                Html.Append("</div>");  // end col-sm-offset-3 col-sm-9
            }


            Html.Append("</div>"); // end form-group

            return base.GetHtml();
        }

    }
}
