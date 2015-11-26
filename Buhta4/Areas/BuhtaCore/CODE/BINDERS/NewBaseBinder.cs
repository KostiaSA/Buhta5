using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public delegate T NewBinderGetMethod<T>();
    public delegate void NewBinderSetMethod(string value);

    public class NewBaseBinder<T>
    {
        //public string TagUniqueId;
        public string jsSetMethodName;
        public string jsSetPropertyName;
        public bool jsSetIsValueAsObject;
        public string ModelPropertyName;
        public string LastSendedText;
        public NewBinderGetMethod<T> ModelGetMethod;


        public bool Is2WayBinding;
        public string jQueryOnChangeEventName;
        public string jQueryGetPropertyName;
        public string jQueryGetMethodName;
        public NewBinderSetMethod ModelSetMethod;

        public bsControlSettings Control;
        public NewBaseBinder()
        {
            //control = _control;
        }

        public NewBaseBinder(string propertyName)
        {
            ModelPropertyName = propertyName;
        }

        public NewBaseBinder(NewBinderGetMethod<T> getMethod)
        {
            ModelGetMethod = getMethod;
        }

        public virtual bool IsBindingExists()
        {
            return ModelPropertyName != null || ModelGetMethod != null;
        }


        public virtual string GetJs()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(NewBaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

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
                throw new Exception(nameof(NewBaseBinder<T>) + "." + nameof(GetJs) + ": не заполнен '" + nameof(jsSetMethodName) + "'");

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

        public void EmitBindingScript_M(StringBuilder script)
        {
            if (IsBindingExists())
            {
                Control.Model.NewRegisterBinder(UniqueId, this);
                LastSendedText = GetJs();
                script.AppendLine(LastSendedText);
                //script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + UniqueId + "',function(newValue){");
                //script.AppendLine("    tag." + jqxMethodName + "(newValue);");
                //script.AppendLine("});");

                if (Is2WayBinding)
                {
                    script.AppendLine("$('#" + Control.UniqueId + "').on('" + jQueryOnChangeEventName + "', function () {");
                    //script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Control.Model.BindingId + "', '" + PropertyName + "',tag.prop('checked')); ");
                    var propName = ModelPropertyName;
                    if (ModelSetMethod != null)
                        propName = UniqueId;
                    if (propName == null)
                        throw new Exception(nameof(NewBaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для двухсторонней привязки нужно указать или имя свойства или set-метод");
                    if (jQueryGetPropertyName != null)
                    {
                        //if (jQuerySetIsValueAsObject)
                        //  return "$('#" + Control.UniqueId + "')." + jQuerySetMethodName + "({'" + jQuerySetPropertyName + "':" + value + "});";
                        //else
                        script.AppendLine("  bindingHub.server.sendBindedValueChanged('" + Control.Model.BindingId + "', '" + propName + "',$('#" + Control.UniqueId + "')." + jQueryGetMethodName + "('" + jQueryGetPropertyName + "'));");
                    }
                    else
                        script.AppendLine("  bindingHub.server.sendBindedValueChanged('" + Control.Model.BindingId + "', '" + propName + "',$('#" + Control.UniqueId + "')." + jQueryGetMethodName + "());");
                    script.AppendLine("}); ");
                }

            }

        }

        string uniqueId;
        public string UniqueId
        {
            get
            {
                if (uniqueId == null)
                {
                    uniqueId = "binder:" + Guid.NewGuid().ToString().Substring(0, 8);
                }
                return uniqueId;
            }
        }

    }
}