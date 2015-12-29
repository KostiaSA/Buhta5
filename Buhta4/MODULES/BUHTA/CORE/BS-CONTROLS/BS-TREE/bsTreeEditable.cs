using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{

    public class bsTreeEditable : bsTagBegin
    {
        public bsEditableType Type = bsEditableType.Text;
        public string Label;

        public bsTreeEditable(BaseModel model) : base(model)
        {
            Tag = "span";
        }

        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod)
        {
            Bind_Value<T>(getValueMethod, null);
        }

        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod, BinderSetMethod<T> setValueMethod)
        {
            if (typeof(T) == typeof(string))
            {
                Type = bsEditableType.Text;

                var binder = new bsTreeEditableValueBinder<T>();

                binder.ModelGetMethod = getValueMethod;

                binder.Is2WayBinding = true;
                binder.ModelSetMethod = setValueMethod;

                AddBinder(binder);
            }
            else
                throw new Exception(nameof(bsEditable) + ": неизвестный тип привязки значения '" + typeof(T).FullName + "'");

        }

        public void Bind_Value<T>(string modelPropertyName)
        {

            if (typeof(T) == typeof(string))
            {
                Type = bsEditableType.Text;
                AddBinder(new bsTreeEditableValueBinder<T>()
                {
                    Is2WayBinding = true,
                    ModelPropertyName = modelPropertyName
                });
            }
            else
                throw new Exception(nameof(bsEditable) + ": неизвестный тип привязки значения '" + typeof(T).FullName + "'");
        }

        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {
          //  UniqueId = "AA2E0C8C4CA5E365";
            AddClass("bs-editable");

            if (Label == null)
                Label = "Введите значение";

            script.AppendLine(@"
var tag=$('#'+node.key.replace('.','-')+'>span');

console.log('key='+node.key.replace('.','-'));
console.log('tag='+tag.toString());

tag.editable({
    type: 'text',
    title: " + Label.AsJavaScript() + @"
});

tag.on('shown', function(e, editable) {
    editable.input.$input.val(tag.text());
    });
");
            base.EmitScriptAndHtml(script, html);
            html.Append("<span>4D57BEAC0040F92312A4</span>&nbsp;<i class='fa fa-pencil-square-o'></i></" + Tag + ">");
        }
    }


}
