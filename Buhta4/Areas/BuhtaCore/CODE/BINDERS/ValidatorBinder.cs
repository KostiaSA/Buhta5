using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class ValidatorBinder : BaseBinder
    {
        public string ModelValidateMethodName;
        public BinderValidateMethod ModelValidateMethod;

        public ValidatorBinder()
        {
            IsActive = false;
            UpdatePriority = 2000;
        }

        public override string GetJsForSettingProperty()
        {
            string retStr;
            var error = new ValidateErrorList();
            foreach (var _b in Control.Binders.Where((b) => b is ValidatorBinder))
            {
                var b = _b as ValidatorBinder;
                if (b.ModelValidateMethod == null && b.ModelValidateMethodName == null)
                    throw new Exception(nameof(ValidatorBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя validate-метода или сам validate-метод");

                if (b.ModelValidateMethod != null)
                {
                    b.ModelValidateMethod(error);
                }
                else
                if (b.ModelValidateMethodName != null)
                {
                    Control.Model.InvokeMethod(b.ModelValidateMethodName, error);
                }
            }

            if (!error.IsEmpty)
                retStr = "$('#" + Control.UniqueId + "-error-text').removeClass('hidden').html(" + error.ToHtmlStringOnlyMessages().AsJavaScript() + "); $('#" + Control.UniqueId + "').parents('.form-group').first().addClass('has-error');";
            else
                retStr = "$('#" + Control.UniqueId + "-error-text').addClass('hidden');$('#" + Control.UniqueId + "').parents('.form-group').first().removeClass('has-error');";

            foreach (var _b in Control.Binders.Where((b) => b is ValidatorBinder))
            {
                if (_b != this)
                    _b.LastSendedText = retStr;
            }

            return retStr;
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //            Control.Model.RegisterBinder(this);
        }


    }
}