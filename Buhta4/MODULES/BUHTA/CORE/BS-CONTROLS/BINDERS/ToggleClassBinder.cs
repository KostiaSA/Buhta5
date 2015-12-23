using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class ToggleClassBinder : BaseBinder
    {
        public bool IsToogleOnTrue = true;
        public string ClassName;
        public string ModelPropertyName;
        public BinderGetMethod<bool> ModelGetMethod;

        public ToggleClassBinder()
        {
            ValueType = typeof(Boolean);
        }

        public override string GetPropertyNameForErrorMessage()
        {
            return ModelPropertyName;
        }


        public override string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(ToggleClassBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            bool value = false;
            if (ModelGetMethod != null)
            {
                value = ModelGetMethod();
            }
            else
            if (ModelPropertyName != null)
            {
                var value_obj = Control.Model.GetPropertyValue(ModelPropertyName);
                if (value_obj is Boolean)
                    value = (Boolean)value_obj;
                else
                    throw new Exception(nameof(ToggleClassBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - привязанное свойство должно быть Boolean");

            }

            if (IsToogleOnTrue && value)
                return "$('#" + Control.UniqueId + "').addClass('" + ClassName + "');";
            else
            if (IsToogleOnTrue && !value)
                return "$('#" + Control.UniqueId + "').removeClass('" + ClassName + "');";
            else
            if (!IsToogleOnTrue && !value)
                return "$('#" + Control.UniqueId + "').addClass('" + ClassName + "');";
            else
            // (!IsToogleOnTrue && value)
                return "$('#" + Control.UniqueId + "').removeClass('" + ClassName + "');";

        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
            //          Debug.Print("EmitBindingScript: " + LastSendedText);

        }


    }
}