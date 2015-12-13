using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class bsGridRowSelectEventBinder : EventBinder
    {
        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);

            if (ModelEventMethod == null && ModelEventMethodName == null)
                throw new Exception(nameof(bsGridRowSelectEventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки событий нужно указать имя обработчика или event-метод");

            script.AppendLine("$('#" + Control.UniqueId + "').DataTable().on('select',");
            // параметры события в js
            // e- jQuery event object
            // dt - DataTables API instance
            // type - Items being selected. This can be 'rows', 'columns' or 'cells'.
            // indexes - The DataTables' indexes of the selected items.
            script.AppendLine("function(e, dt, type, indexes) {");
            script.AppendLine("  var _args={rowId:dt.data()[" + (Control as bsGrid).GetKeyFieldIndex().ToString() + "]};");
            script.AppendLine("  if (type=='row'){");

            if (ModelEventMethodName != null)
                script.AppendLine("    bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + ModelEventMethodName + "', _args );");
            else
                script.AppendLine("    bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + UniqueId + "', _args );");

            script.AppendLine("  }");
            script.AppendLine("}");
            script.AppendLine(");");

            return;

        }

    }
}