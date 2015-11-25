using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public delegate T NewBinderGetMethod<T>();

    public class NewBaseBinder<T>
    {
        public BaseModel Model;

        //public string TagUniqueId;
        public string jQueryMethodName;
        public string jQueryPropertyName;
        public string PropertyName;
        public string LastSendedText;
        public NewBinderGetMethod<T> GetMethod;

        public bsControlSettings Control;
        public NewBaseBinder()
        {
            //control = _control;
        }

        public NewBaseBinder(string propertyName)
        {
            PropertyName = propertyName;
        }

        public NewBaseBinder(NewBinderGetMethod<T> getMethod)
        {
            GetMethod = getMethod;
        }

        public virtual bool IsBindingExists()
        {
            return PropertyName != null || GetMethod != null;
        }


        //public virtual string GetDisplayText()
        //{
        //    throw new Exception("метод " + nameof(GetDisplayText) + " не реализован");
        //}

        //public virtual object ParseDisplayText(string text)
        //{
        //    throw new Exception("метод "+nameof(ParseDisplayText)+" не реализован");
        //}

        public virtual string GetJs()
        {
            string value = "";
            if (GetMethod != null)
                value = GetMethod().AsJavaScript();
            else
            if (PropertyName != null)
                value = Model.GetPropertyValue(PropertyName).ToString();

            if (jQueryMethodName == null)
                throw new Exception(nameof(NewBaseBinder<T>) + "." + nameof(GetJs) + ": не заполнен '" + nameof(jQueryMethodName) + "'");

            //Debug.Print("$('#" + TagUniqueId + "')." + jQueryMethodName + "(" + value + ");");

            if (jQueryPropertyName != null)
                return "$('#" + Control.UniqueId + "')." + jQueryMethodName + "({'" + jQueryPropertyName + "':" + value + "});";
            else
                return "$('#" + Control.UniqueId + "')." + jQueryMethodName + "(" + value + ");";

        }

        public void EmitBindingScript_M(StringBuilder script)
        {
            if (IsBindingExists())
            {
                Model.NewRegisterBinder(UniqueId, this);
                LastSendedText = GetJs();
                script.AppendLine(LastSendedText);
                //script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + UniqueId + "',function(newValue){");
                //script.AppendLine("    tag." + jqxMethodName + "(newValue);");
                //script.AppendLine("});");

                //script.AppendLine("tag.on('" + jqxEventName + "', function () {");
                //script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',tag.prop('checked')); ");
                //script.AppendLine("}); ");

            }

        }

        string uniqueId;
        public string UniqueId
        {
            get
            {
                if (uniqueId == null)
                {
                    uniqueId = "Binder:" + Guid.NewGuid().ToString().Substring(1, 8);
                }
                return uniqueId;
            }
        }

    }
}