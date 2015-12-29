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
        public static MvcHtmlString bsEditFormSaveButton(this HtmlHelper helper, Action<bsButton> settings)
        {
            if (!(helper.ViewData.Model is BaseEditFormModel))
                throw new Exception(nameof(bsEditFormSaveButton)+" может использоваться только с моделью типа '"+nameof(BaseEditFormModel)+"'");

            var model = helper.ViewData.Model as BaseEditFormModel;
            var button = new bsButton(model);

            button.ButtonStyle = bsButtonStyle.Success;
            button.Bind_OnClick(model.SaveButtonClick);
            button.Bind_Text(()=>model.GetSaveButtonText());
            button.Bind_Disabled(() => model.GetSaveButtonDisabled());
            button.Bind_OnTrueClass(() => model.IsInsertMode, "btn-success");
            button.Bind_OnFalseClass(() => model.IsInsertMode, "btn-info");
            button.AddStyle("min-width", "80px");

            settings(button);

            (helper.ViewData.Model as BaseModel).Helper = helper;

            //var script = new StringBuilder();
            //var html = new StringBuilder();

            return new MvcHtmlString(button.GetHtml());
        }

    }


}
