using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class IntBinder : BaseBinder
    {
        public IntBinder(string propertyName) : base(propertyName) { }

        //static IntBinder()
        //{
        //    BaseBinder.DefaultBinders.Add(typeof(int), new IntBinder());
        //}

        public override string GetDisplayText()
        {
            return "value.ToString()";
        }

        public override object ParseDisplayText(string text)
        {
            return int.Parse(text);
        }
    }
}