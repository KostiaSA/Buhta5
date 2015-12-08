using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsInputToSchemaObjectBinder : OneWayBinder<Guid>
    {
        public Type SchemaObjectType;
        public void SelectButtonClick(dynamic args)
        {

            var oldValue = Control.Model.GetPropertyValue(ModelPropertyName);
            if (oldValue==null || oldValue is Guid?)
            {
                var schemaObjectDialogmodel = new SelectSchemaObjectDialogModel(Control.Model.Controller, Control.Model, oldValue as Guid?, SchemaObjectType);
                schemaObjectDialogmodel.OkEventMethod = CallOnChangeBinder;

                var modal = Control.Model.CreateModal(@"~/Areas/BuhtaCore/Views/SelectSchemaObjectDialog.cshtml", schemaObjectDialogmodel);
                modal.Show();
            }
            else
                throw new Exception(nameof(bsInputToSchemaObjectBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'Guid'");
        }

        public void CallOnChangeBinder(dynamic args)
        {
            foreach (var binder in Control.Binders)
            {
                if (binder is EventBinder)
                {
                    var b = (binder as EventBinder);
                    if (b.jsEventName == "change")
                    {
                        if (b.ModelEventMethod != null)
                            b.ModelEventMethod(args);
                        else
                        if (b.ModelEventMethodName != null)
                            Control.Model.InvokeMethod(b.ModelEventMethodName, args);
                        Control.Model.Update();
                    }
                }
            }

        }

        public override string GetJsForSettingProperty()
        {
            var value = Control.Model.GetPropertyValue(ModelPropertyName);
            if (value==null || value is Guid?)
            {
                string lookupValue="<пусто>";
                if (value != null)
                    lookupValue = App.Schema.GetObjectName(value as Guid?);
                return "$('#" + Control.UniqueId + "').val(" + lookupValue.AsJavaScript() + ");";
            }
            else
                throw new Exception(nameof(bsInputToSchemaObjectBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'Guid'");
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
        }

    }

}