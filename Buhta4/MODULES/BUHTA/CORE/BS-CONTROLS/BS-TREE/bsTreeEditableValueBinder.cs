﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class bsTreeEditableValueBinder<T> : BaseBinder, ITwoWayBinder<T>
    {
        public bool Is2WayBinding;
//        public string jsOnChangeEventName;
  //      public string jsGetPropertyName;
    //    public string jsGetMethodName;
        public string ModelPropertyName { get; set; }
        public BinderGetMethod<T> ModelGetMethod;

        public bsTreeEditableValueBinder()
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
            if (ModelPropertyName != null)
            {
                value = "row['" + ModelPropertyName + "']";
            }

            if (isHtmlMode)
            {
                return "$('#" + Control.UniqueId + ">span').html(" + value + ");";
            }
            else
                return "$('#" + Control.UniqueId + ">span').text(" + value + ");";

        }

        public BinderSetMethod<T> ModelSetMethod { get; set; }

        public override void EmitBindingScript(StringBuilder script)
        {

            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

            script.AppendLine("$('#" + Control.UniqueId + ">span').on('save', function(e,params) {");

            var propName = UniqueId;
            if (propName == null && ModelSetMethod != null)
                throw new Exception(nameof(bsEditableValueBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для двухсторонней привязки нужно указать или имя свойства или set-метод");

            script.AppendLine("  bindingHub.server.sendBindedValueChanged(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "', '" + propName + "', params.newValue);");
            script.AppendLine("}); ");

        }



    }
}