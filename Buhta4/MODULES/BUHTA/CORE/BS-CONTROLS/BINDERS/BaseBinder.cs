using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public delegate T BinderGetMethod<T>();
    public delegate void BinderSetMethod<T>(T value);
    public delegate void BinderEventMethod(dynamic args);
    public delegate void BinderValidateMethod(ValidateErrorList error);

    public abstract class BaseBinder
    {
        public bool IsActive = true;
        public Type ValueType;
        public abstract void EmitBindingScript(StringBuilder script);

        public virtual string GetJsForSettingProperty() { return ""; }
        public string LastSendedText = "";
        public bool IsNotAutoUpdate;

        public int UpdatePriority = 1000;

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

        public virtual string GetPropertyNameForErrorMessage()
        {
            return null;
        }
           

    }

}