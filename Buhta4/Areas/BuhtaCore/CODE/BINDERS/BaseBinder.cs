using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class BaseBinder
    {
        public static Dictionary<Type, BaseBinder> DefaultBinders = new Dictionary<Type, BaseBinder>();

        public string PropertyName;
        public string LastSendedText;
        public BaseBinder(string propertyName)
        {
            PropertyName = propertyName;
        }

        public virtual string GetDisplayText(object value)
        {
            return "?";
        }

        public virtual object ParseDisplayText(string text)
        {
            return "?";
        }


    }
}