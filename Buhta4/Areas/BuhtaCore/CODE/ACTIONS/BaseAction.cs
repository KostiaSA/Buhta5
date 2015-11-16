using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class BaseAction
    {
        public virtual void EmitJsCode(StringBuilder script)
        {
        }

        public virtual string GetJsCode()
        {
            StringBuilder script = new StringBuilder();
            EmitJsCode(script);
            return script.ToString();
        }
    }
}