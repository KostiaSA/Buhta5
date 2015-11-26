using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class StringBinder : OldBaseBinder
    {
        public get_string GetMethod;
        public StringBinder(string propertyName) : base(propertyName) { }
        public StringBinder(get_string getMethod) : base("") { GetMethod = getMethod; }
        //static StringBinder()
        //{
        //    BaseBinder.DefaultBinders.Add(typeof(String), new StringBinder());
        //}

        public override string GetDisplayText()
        {
            if (GetMethod != null)
                return GetMethod();
            else
            if (PropertyName != null)
                return Model.GetPropertyValue(PropertyName).ToString();
            else
                throw new Exception(nameof(StringBinder) + "." + nameof(GetDisplayText) + ": internal error");
        }

        public override object ParseDisplayText(string text)
        {
            return text;
        }
    }
}