using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public delegate object BinderGetMethod();
    public delegate void BinderSetMethod(string value);
    public delegate void BinderEventMethod(dynamic args);

    public abstract class BaseBinder
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

        //public abstract BinderEventMethod ModelEventMethod { get; set; }
        //public abstract BinderSetMethod ModelSetMethod { get; set; }

        //public bool IsEventBinding;
        //public abstract string GetJsForSettingProperty();

        //public string LastSendedText;

    }

}