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
        public static MvcHtmlString bsEditFormCancelButton(this HtmlHelper helper, Action<bsButton> settings)
        {
            if (!(helper.ViewData.Model is BaseEditFormModel))
                throw new Exception(nameof(bsEditFormSaveButton)+" может использоваться только с моделью типа '"+nameof(BaseEditFormModel)+"'");

            var model = helper.ViewData.Model as BaseEditFormModel;
            var button = new bsButton(model);

            button.AddClass("modal-cancel-button");
            button.ButtonStyle = bsButtonStyle.Default;
            button.Bind_OnClick(model.CancelButtonClick);
            button.Bind_Text(()=>model.GetCancelButtonText());

            settings(button);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(button.GetHtml());
        }

    }


}
