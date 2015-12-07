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
            if (ModelValidateMethod == null && ModelValidateMethodName == null)
                throw new Exception(nameof(ValidatorBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя validate-метода или сам validate-метод");

            var error = new StringBuilder();
            if (ModelValidateMethod != null)
            {
                ModelValidateMethod(error);
            }
            else
            if (ModelValidateMethodName != null)
            {
                Control.Model.InvokeMethod(ModelValidateMethodName, error);
            }

            if (error.Length > 0)
                return "alter('" + error.ToString() + "');";
            else
                return "alter('нет ошибок');";
            //return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "(" + value + ",true);";

        }

        public override void EmitBindingScript(StringBuilder script)
        {
        }


    }
}