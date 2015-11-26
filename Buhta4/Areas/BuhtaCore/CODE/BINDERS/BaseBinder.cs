using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public abstract class CoreBinder
    {
        public abstract void EmitBindingScript(StringBuilder script);

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

        public bsControl Control;

        public abstract BinderEventMethod ModelEventMethod { get; set; }
        public abstract BinderSetMethod ModelSetMethod { get; set; }

        public bool IsEventBinding;
        public abstract string GetJsForSettingProperty();

        public string LastSendedText;

    }

    public delegate T BinderGetMethod<T>();
    public delegate void BinderSetMethod(string value);
    public delegate void BinderEventMethod(dynamic args);

    public class BaseBinder<T>:CoreBinder
    {
        //public string TagUniqueId;
        public string jsSetMethodName;
        public string jsSetPropertyName;
        public bool jsSetIsValueAsObject;
        public string ModelPropertyName;
        public BinderGetMethod<T> ModelGetMethod;


        public bool Is2WayBinding;
        public string jsOnChangeEventName;
        public string jsGetPropertyName;
        public string jsGetMethodName;
        public override BinderSetMethod ModelSetMethod { get; set; }

        public string jsEventName;
        public override BinderEventMethod ModelEventMethod { get; set; }
        public string ModelEventMethodName;

        public BaseBinder()
        {
            //control = _control;
        }

        //public BaseBinder(string propertyName)
        //{
        //    ModelPropertyName = propertyName;
        //}

        //public BaseBinder(BinderGetMethod<T> getMethod)
        //{
        //    ModelGetMethod = getMethod;
        //}

        //public virtual bool IsBindingExists()
        //{
        //    return ModelPropertyName != null || ModelGetMethod != null;
        //}


        public override string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(BaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

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
                throw new Exception(nameof(BaseBinder<T>) + "." + nameof(GetJsForSettingProperty) + ": не заполнен '" + nameof(jsSetMethodName) + "'");

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

            if (IsEventBinding)
            {
                if (ModelEventMethod == null && ModelEventMethodName == null)
                    throw new Exception(nameof(BaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки событий нужно указать имя обработчика или event-метод");

                if (jsEventName == null)
                    throw new Exception(nameof(BaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' для привязки события нужно указать jsEventName");

                script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsEventName + "',function(event){");
                script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
                if (ModelEventMethodName != null)
                    script.AppendLine(" bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + ModelEventMethodName + "', args );");
                else
                    script.AppendLine(" bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + UniqueId + "', args );");
                script.AppendLine("});");
                return;
            }


            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

            if (Is2WayBinding)
            {
                script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsOnChangeEventName + "', function () {");
                var propName = ModelPropertyName;
                if (ModelSetMethod != null)
                    propName = UniqueId;
                if (propName == null)
                    throw new Exception(nameof(BaseBinder<T>) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для двухсторонней привязки нужно указать или имя свойства или set-метод");

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
}