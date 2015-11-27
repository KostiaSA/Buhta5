using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace Buhta
{
    public class JsObject : JsBaseObject
    {
        Dictionary<string, object> props = new Dictionary<string, object>();
        Dictionary<string, string> rawProps = new Dictionary<string, string>();  // свойство в уже готовом формате json

        public void AddProperty(string propertyName, object value)
        {
            props.Add(propertyName, value);
        }

        public void AddRawProperty(string propertyName, string value)
        {
            rawProps.Add(propertyName, value);
        }

        public override string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("{");

            foreach (var keyVP in props)
            {
                sb.Append(keyVP.Key.AsJavaScript() + ":");
                if (keyVP.Value is JsBaseObject)
                    sb.Append(((JsBaseObject)keyVP.Value).ToJson() + ",");
                else
                    sb.Append(Json.Encode(keyVP.Value) + ",");
            }

            foreach (var keyVP in rawProps)
            {
                sb.Append(keyVP.Key.AsJavaScript() + ":");
                sb.Append(keyVP.Value + ",");
            }

            if (sb.Length > 1)
                sb.RemoveLastChar();
            sb.Append("}");
            return sb.ToString();
        }
    }
}