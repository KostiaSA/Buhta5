﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class TwoWayBinder<T> : OneWayBinder<T>
    {

        public bool Is2WayBinding;
        public string jsOnChangeEventName;
        public string jsGetPropertyName;
        public string jsGetMethodName;

        public TwoWayBinder()
        {
            ValueType = typeof(T);
        }

        public BinderSetMethod<T> ModelSetMethod { get; set; }

        public override void EmitBindingScript(StringBuilder script)
        {
            base.EmitBindingScript(script);

            script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsOnChangeEventName + "', function () {");
            var propName = ModelPropertyName;
            if (ModelSetMethod != null)
                propName = UniqueId;
            if (propName == null)
                throw new Exception(nameof(TwoWayBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для двухсторонней привязки нужно указать или имя свойства или set-метод");

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