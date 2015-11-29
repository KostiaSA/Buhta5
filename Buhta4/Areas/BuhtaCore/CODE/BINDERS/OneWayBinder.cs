using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class OneWayBinder:BaseBinder
    {
        public string jsSetMethodName;
        public string jsSetPropertyName;
        public bool jsSetIsValueAsObject;
        public string ModelPropertyName;
        public BinderGetMethod ModelGetMethod;

        public string LastSendedText;

        public bool IsNotAutoUpdate;

        public virtual string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(OneWayBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            string value = "";
            if (ModelGetMethod != null)
                value = ModelGetMethod().AsJavaScript();
            else
            if (ModelPropertyName != null)
            {
                var value_obj = Control.Model.GetPropertyValue(ModelPropertyName);
                if (value_obj == null)
                    value = "null";
                else
                    value = value_obj.AsJavaScript();

            }

            if (jsSetMethodName == null)
                throw new Exception(nameof(OneWayBinder) + "." + nameof(GetJsForSettingProperty) + ": не заполнен '" + nameof(jsSetMethodName) + "'");

            //Debug.Print("$('#" + TagUniqueId + "')." + jQueryMethodName + "(" + value + ");");

            if (jsSetPropertyName != null)
            {
                if (jsSetIsValueAsObject)
                    return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "({'" + jsSetPropertyName + "':" + value + "});";
                else
                    return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "('" + jsSetPropertyName + "'," + value + ");";
            }
            else
                return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "(" + value + ");";

        }

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

        }


    }
}