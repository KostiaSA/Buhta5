using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class StringBinder : BaseBinder
    {
        public StringBinder(string propertyName) : base(propertyName) { }
        //static StringBinder()
        //{
        //    BaseBinder.DefaultBinders.Add(typeof(String), new StringBinder());
        //}

        public override string GetDisplayText(object value)
        {
            return value.ToString();
        }

        public override object ParseDisplayText(string text)
        {
            return text;
        }
    }
}