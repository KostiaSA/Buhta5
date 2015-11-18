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
        public bsInputType Type = bsInputType.Text;

        public bool? Disabled;
        public string Disabled_Bind;

        public string Text = "";
        public string Text_Bind;

        public string Label;
        public string PlaceHolder;

        public string Image;


        public bsInputSize Size = bsInputSize.Default;
    }

    public class bsInput : bsControl<bsInputSettings>
    {

        public bsInput(object model, bsInputSettings settings) : base(model, settings) { }
        public bsInput(object model, Action<bsInputSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            EmitBeginScript(Script);


            //EmitProperty(Script, "disabled", Settings.Disabled);
            //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");


            //EmitProperty_M(Script, "val", Settings.Text);



            if (Settings.Size == bsInputSize.Large)
                AddClass("input-lg");
            else
            if (Settings.Size == bsInputSize.Small)
                AddClass("input-sm");
            else
            if (Settings.Size == bsInputSize.ExtraSmall)
                AddClass("input-xs");

            if (Settings.PlaceHolder != null)
                AddAttr("placeholder", Settings.PlaceHolder);


            Html.Append("<div class='form-group'>"); // begin form-group

            if (Settings.Type == bsInputType.Text)
            {

                EmitProperty_Bind2Way_M(Script, Settings.Text_Bind, "val", "change");
                AddClass("form-control");

                if (Settings.Label != null)
                {
                    //Html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    Html.Append("<label class='col-sm-3 control-label' >" + Settings.Label + "</label>");
                    //Html.Append("</div>");  // end col-sm-3
                    Html.Append("<div class='col-sm-9'>");  // begin col-sm-9
                }

                Html.Append("<input id='" + UniqueId + "' type='" + Settings.Type.ToString().ToLower() + "' " + GetAttrs() + ">" + Settings.Text + "</input>");

                if (Settings.Label != null)
                {
                    Html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Settings.Type == bsInputType.Checkbox)
            {
                EmitProperty_Bind2Way_Checked(Script, Settings.Text_Bind, "change");

                Html.Append("<div class='col-sm-offset-3 col-sm-9'>"); // beg col-sm-offset-3 col-sm-9
                Html.Append("<div class='checkbox'>");
                Html.Append("<label>");

                Html.Append("<input id='" + UniqueId + "' type='" + Settings.Type.ToString().ToLower() + "' " + GetAttrs() + "></input>");
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
