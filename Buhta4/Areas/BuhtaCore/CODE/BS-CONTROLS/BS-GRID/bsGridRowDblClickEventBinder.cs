using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class bsGridRowDblClickEventBinder : EventBinder
    {
        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);

            if (ModelEventMethod == null && ModelEventMethodName == null)
                throw new Exception(nameof(bsGridRowDblClickEventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки событий нужно указать имя обработчика или event-метод");

            script.AppendLine("$('#" + Control.UniqueId + " tbody').on('dblclick','tr',");
            script.AppendLine("function(event) {");
            //script.AppendLine("  alert($(event.target).parent().first().children().eq(" + (Control as bsGrid).GetKeyFieldIndex().ToString() + ").text());");
            //script.AppendLine("  console.log(event); window.xevent=event;");
            script.AppendLine("  var _args={rowId:$(event.target).parent().first().children().eq(" + (Control as bsGrid).GetKeyFieldIndex().ToString() + ").text()};");

            if (ModelEventMethodName != null)
                script.AppendLine("    bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + ModelEventMethodName + "', _args );");
            else
                script.AppendLine("    bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + UniqueId + "', _args );");

            script.AppendLine("}");
            script.AppendLine(");");

            return;

        }

    }
}