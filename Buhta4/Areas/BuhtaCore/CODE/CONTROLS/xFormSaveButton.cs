using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString xFormSaveButton(this HtmlHelper helper, xButtonSettings settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormSaveButton(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormSaveButton(this HtmlHelper helper, Action<xButtonSettings> settings)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormSaveButton(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xFormSaveButton(this HtmlHelper helper)
        {
            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(new xFormSaveButton(helper.ViewData.Model, new xButtonSettings()).GetHtml());
        }
    }

    public class xFormSaveButton : xButton
    {
        public xFormSaveButton(object model, xButtonSettings settings) : base(model, settings) { }
        public xFormSaveButton(object model, Action<xButtonSettings> settings) : base(model, settings) { }

        public override string GetHtml()
        {
            if (Settings.Text == null)
                Settings.Text = "Сохранить";

            if (Settings.OnClick_Bind == null)
                Settings.OnClick_Bind = nameof(Model.SaveButtonClick);

            return base.GetHtml();
        }


    }
}