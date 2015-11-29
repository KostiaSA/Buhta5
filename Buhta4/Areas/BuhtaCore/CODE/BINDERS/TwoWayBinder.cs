using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class TwoWayBinder : OneWayBinder
    {

        public bool Is2WayBinding;
        public string jsOnChangeEventName;
        public string jsGetPropertyName;
        public string jsGetMethodName;

        public virtual BinderSetMethod ModelSetMethod { get; set; }


        //public override string GetJsForSettingProperty()
        //{
        //    if (ModelGetMethod == null && ModelPropertyName == null)
        //        throw new Exception(nameof(CommonBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

        //    string value = "";
        //    if (ModelGetMethod != null)
        //        value = ModelGetMethod().AsJavaScript();
        //    else
        //    if (ModelPropertyName != null)
        //    {
        //        var value_obj = Control.Model.GetPropertyValue(ModelPropertyName);
        //        if (value_obj == null)
        //            value = "null";
        //        else
        //            value = value_obj.AsJavaScript();

        //    }

        //    if (jsSetMethodName == null)
        //        throw new Exception(nameof(CommonBinder<T>) + "." + nameof(GetJsForSettingProperty) + ": не заполнен '" + nameof(jsSetMethodName) + "'");

        //    //Debug.Print("$('#" + TagUniqueId + "')." + jQueryMethodName + "(" + value + ");");

        //    if (jsSetPropertyName != null)
        //    {
        //        if (jsSetIsValueAsObject)
        //            return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "({'" + jsSetPropertyName + "':" + value + "});";
        //        else
        //            return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "('" + jsSetPropertyName + "'," + value + ");";
        //    }
        //    else
        //        return "$('#" + Control.UniqueId + "')." + jsSetMethodName + "(" + value + ");";

        //}

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);

            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

            // Is2WayBinding)

            script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsOnChangeEventName + "', function () {");
            var propName = ModelPropertyName;
            if (ModelSetMethod != null)
                propName = UniqueId;
            if (propName == null)
                throw new Exception(nameof(TwoWayBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для двухсторонней привязки нужно указать или имя свойства или set-метод");

            if (jsGetPropertyName != null)
            {
                script.AppendLine("  bindingHub.server.sendBindedValueChanged('" + Control.Model.BindingId + "', '" + propName + "',$('#" + Control.UniqueId + "')." + jsGetMethodName + "('" + jsGetPropertyName + "'));");
            }
            else
                script.AppendLine("  bindingHub.server.sendBindedValueChanged('" + Control.Model.BindingId + "', '" + propName + "',$('#" + Control.UniqueId + "')." + jsGetMethodName + "());");
            script.AppendLine("}); ");

        }


    }
}