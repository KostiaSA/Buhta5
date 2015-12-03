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
        public static MvcHtmlString bsMessageDialogSaveButton(this HtmlHelper helper, Action<bsButton> settings)
        {
            if (!(helper.ViewData.Model is MessageDialogModel))
                throw new Exception(nameof(bsEditFormSaveButton)+" может использоваться только с моделью типа '"+nameof(MessageDialogModel) +"'");

            var model = helper.ViewData.Model as MessageDialogModel;
            var button = new bsButton(model);

            button.ButtonStyle = bsButtonStyle.Success;
            button.Bind_OnClick(model.OkButtonClick);
            button.Text = "Ok";

            settings(button);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(button.GetHtml());
        }

    }


}
