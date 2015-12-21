using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsInputToSchemaObjectBinder : OneWayBinder<Guid?>
    {
        public BinderSetMethod<Guid?> ModelSetMethod { get; set; }

        SelectSchemaObjectDialogModel schemaObjectDialogmodel = null;

        public Type SchemaObjectType;
        public void SelectButtonClick(dynamic args)
        {

            var oldValue = Control.Model.GetPropertyValue(ModelPropertyName);
            if (oldValue == null || oldValue is Guid?)
            {
                schemaObjectDialogmodel = new SelectSchemaObjectDialogModel(Control.Model.Controller, Control.Model, oldValue as Guid?, SchemaObjectType);
                schemaObjectDialogmodel.OkEventMethod = CallOnChangeBinder;
                var modal = Control.Model.CreateModal(@"~/MODULES/BUHTA/CORE/DIALOGS/SelectSchemaObjectDialogView.cshtml", schemaObjectDialogmodel);
                modal.Show();
            }
            else
                throw new Exception(nameof(bsInputToSchemaObjectBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'Guid'");
        }

        void CallOnChangeBinder(dynamic args)
        {
            if (ModelSetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(bsInputToSchemaObjectBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или set-метод");

            if (ModelPropertyName != null)
            {
                Control.Model.SetPropertyValue(ModelPropertyName, schemaObjectDialogmodel.Value);
            }
            else
                ModelSetMethod(schemaObjectDialogmodel.Value);

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
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(bsInputToSchemaObjectBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            Guid? value;
            if (ModelGetMethod != null)
            {
                value = ModelGetMethod();
            }
            else
            {
                var _value = Control.Model.GetPropertyValue(ModelPropertyName);
                if (_value == null)
                    value = null;
                else
                {
                    if (_value is Guid?)
                        value = (Guid?)_value;
                    else
                        throw new Exception(nameof(bsInputToSchemaObjectBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'Guid'");
                }
            }
            string lookupValue = "<пусто>";
            if (value != null)
                lookupValue = App.Schema.GetObjectName(value as Guid?);
            return "$('#" + Control.UniqueId + "').val(" + lookupValue.AsJavaScript() + ");";
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
        }

    }

}