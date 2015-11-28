using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{

    public class bsTreeRowEventBinder : BaseBinder
    {
        public string jsEventName;
        public override BinderEventMethod ModelEventMethod { get; set; }
        public string ModelEventMethodName;
        public bool isIgnoreForFolder;

        public override BinderSetMethod ModelSetMethod { get; set; }
        public override string GetJsForSettingProperty() { return ""; }

        public bsTreeRowEventBinder()
        {
            IsEventBinding = true;
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);

            if (ModelEventMethod == null && ModelEventMethodName == null)
                throw new Exception(nameof(bsTreeRowEventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки событий нужно указать имя обработчика или event-метод");

            if (jsEventName == null)
                throw new Exception(nameof(bsTreeRowEventBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' для привязки события нужно указать jsEventName");


            script.AppendLine("$('#" + Control.UniqueId + "').fancytree('option','" + jsEventName + "',");
            script.AppendLine("function(event, data) { alert('бля1');");
            if (isIgnoreForFolder)
                script.AppendLine("  if (!data.node.children) {");
            script.AppendLine("    var _args={rowId:data.node.key, tagId:event.target.id, isSelected:data.node.isSelected()};");

            //script.AppendLine("   alert(data.node.isSelected()); if (data.node.isSelected()) {alert('бля X');}");

            //if (jsEventName == "select")
              //  script.AppendLine("   if (data.node.isSelected()) {");
            //if (jsEventName == "unselect")
              //  script.AppendLine("   if (!data.node.isSelected()) {");

            if (ModelEventMethodName != null)
                script.AppendLine("    bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + ModelEventMethodName + "', _args );");
            else
                script.AppendLine("    bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + UniqueId + "', _args );");

            //if (jsEventName == "select" || jsEventName == "unselect")
              //  script.AppendLine("   };");

            if (isIgnoreForFolder)
                script.AppendLine("  };");
            script.AppendLine("}");
            script.AppendLine(");");

            //script.AppendLine("$('#" + Control.UniqueId + "').on('" + jsEventName + "',function(event){");  data.node.isSelected()
            //script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
            //if (ModelEventMethodName != null)
            //    script.AppendLine(" bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + ModelEventMethodName + "', args );");
            //else
            //    script.AppendLine(" bindingHub.server.sendEvent('" + Control.Model.BindingId + "','" + UniqueId + "', args );");
            //script.AppendLine("});");
            return;

        }

    }
}