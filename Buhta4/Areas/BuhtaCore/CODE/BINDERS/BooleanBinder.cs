using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class BooleanBinder : BaseBinder
    {
        public BooleanBinder(string propertyName) : base(propertyName) { }

        //static BooleanBinder()
        //{
        //    BaseBinder.DefaultBinders.Add(typeof(Boolean), new BooleanBinder());
        //}

        //public override string GetDisplayText()
        //{
        //    return "value.ToString().ToLower()";
        //}

        //public override object ParseDisplayText(string text)
        //{
        //    return Boolean.Parse(text);
        //}
    }
}