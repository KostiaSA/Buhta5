using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class CloseWindowAction:BaseAction
    {
        public override void EmitJsCode(StringBuilder script)
        {
            script.AppendLine("if (window.opener) window.open('', window.opener.name).focus(); window.close();");
        }

    }
}