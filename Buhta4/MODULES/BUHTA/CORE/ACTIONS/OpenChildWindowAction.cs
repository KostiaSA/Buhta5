using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class OpenChildWindowAction : BaseAction
    {
        public string Url="";
        public override void EmitJsCode(StringBuilder script)
        {
            script.AppendLine("window.open("+Url.AsJavaScript()+").focus();");
        }

    }
}