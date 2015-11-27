using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace Buhta
{
    public class jsArray : JsBaseObject
    {
        class obj
        {
            public bool isRaw;
            public object value;
        }

        List<obj> props = new List<obj>();

        public void AddObject(object value)
        {
            props.Add(new obj() { value = value, });
        }

        public void AddRawObject(object value)
        {
            props.Add(new obj() { value = value, isRaw = true });
        }

        public override string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var obj in props)
            {
                if (obj.isRaw)
                    sb.Append(obj.value.ToString() + ",");
                else
                if (obj.value is JsBaseObject)
                    sb.Append(((JsBaseObject)obj.value).ToJson() + ",");
                else
                    sb.Append(Json.Encode(obj.value) + ",");
            }

            sb.RemoveLastChar();
            sb.Append("]");
            return sb.ToString();
        }
    }
}