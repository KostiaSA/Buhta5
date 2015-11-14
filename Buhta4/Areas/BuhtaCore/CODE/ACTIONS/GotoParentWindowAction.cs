using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class GotoParentWindowAction:BaseAction
    {
        public override void EmitJsCode(StringBuilder script)
        {
            script.AppendLine("if (window.opener) window.open('', window.opener.name).focus();");
        }

    }
}