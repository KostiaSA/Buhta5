using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class EventBinder : BaseBinder
    {

        public string jsEventName;
        public string ModelEventMethodName;
        public BinderEventMethod ModelEventMethod { get; set; }

        public override string GetPropertyNameForErrorMessage()
        {
            return ModelEventMethodName;
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);

            if (ModelEventMethod == null && ModelEventMethodName == null)
                throw new Exception(nameof(EventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки событий нужно указать имя обработчика или event-метод");

            if (jsEventName == null)
                throw new Exception(nameof(EventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' для привязки события нужно указать jsEventName");

            script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsEventName + "',function(event){");
            script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
            if (ModelEventMethodName != null)
                script.AppendLine(" bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + ModelEventMethodName + "', args );");
            else
                script.AppendLine(" bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + Control.Model.BindingId + "','" + UniqueId + "', args );");
            script.AppendLine("});");
            return;

        }

    }
}