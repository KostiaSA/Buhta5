using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class OneWayBinder<T> : BaseBinder
    {
        public string jsSetMethodName;
        public string jsSetPropertyName;
        public bool jsSetIsValueAsObject;
        public string ModelPropertyName;
        public BinderGetMethod<T> ModelGetMethod;

        public OneWayBinder()
        {
            ValueType = typeof(T);
        }

        public override string GetPropertyNameForErrorMessage()
        {
            return ModelPropertyName;
        }


        public override string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(OneWayBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            bool isHtmlMode = false;

            string value = "";
            if (ModelGetMethod != null)
            {
                var _value_obj = ModelGetMethod();

                if ((jsSetMethodName == "text" || jsSetMethodName.EndsWith(".text")) && _value_obj.ToString().StartsWith("@") && !_value_obj.ToString().StartsWith("@@"))
                {
                    value = "'" + _value_obj.ToString().AsHtmlEx().Replace("'", "\'") + "'";
                    isHtmlMode = true;
                }
                else
                    value = _value_obj.AsJavaScript();
            }
            else
            if (ModelPropertyName != null)
            {
                var value_obj = Control.Model.GetPropertyValue(ModelPropertyName);
                if (value_obj == null)
                    value = "null";
                else
                {
                    if ((jsSetMethodName == "text" || jsSetMethodName.EndsWith(".text")) && value_obj.ToString().StartsWith("@") && !value_obj.ToString().StartsWith("@@"))
                    {
                        value = "'" + value_obj.ToString().AsHtmlEx().Replace("'", "\'") + "'";
                        isHtmlMode = true;
                    }
                    else
                        value = value_obj.AsJavaScript();
                }

            }

            if (jsSetMethodName == null)
                throw new Exception(nameof(OneWayBinder<T>) + "." + nameof(GetJsForSettingProperty) + ": не заполнен '" + nameof(jsSetMethodName) + "'");

            //Debug.Print("$('#" + TagUniqueId + "')." + jQueryMethodName + "(" + value + ");");

            if (jsSetPropertyName != null)
            {
                if (jsSetIsValueAsObject)
                    return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "({'" + jsSetPropertyName + "':" + value + "});";
                else
                    return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "('" + jsSetPropertyName + "'," + value + ");";
            }
            else
            {
                if (isHtmlMode)
                {
                    if (jsSetMethodName == "text")
                        return "$('#" + Control.UniqueId + "').html(" + value + ");";
                    else // .text
                        return "$('#" + Control.UniqueId + "')." + jsSetMethodName.Replace(".text", ".html") + "(" + value + ");";

                }
                else
                    return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "(" + value + ",true);";
            }

        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
            Debug.Print("EmitBindingScript: " + LastSendedText);

        }


    }
}