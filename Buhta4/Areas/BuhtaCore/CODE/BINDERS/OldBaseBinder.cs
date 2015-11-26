using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class OldBaseBinder
    {
        public BaseModel Model;

        public static Dictionary<Type, OldBaseBinder> DefaultBinders = new Dictionary<Type, OldBaseBinder>();

        public string PropertyName;
        public string LastSendedText;

        public OldBaseBinder(string propertyName)
        {
            PropertyName = propertyName;
        }

        public virtual string GetDisplayText()
        {
            throw new Exception("метод " + nameof(GetDisplayText) + " не реализован");
        }

        public virtual object ParseDisplayText(string text)
        {
            throw new Exception("метод "+nameof(ParseDisplayText)+" не реализован");
        }


    }
}