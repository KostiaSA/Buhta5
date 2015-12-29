using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {

        public static MvcHtmlString bsEditable(this HtmlHelper helper, Action<bsEditable> settings)
        {
            var Settings = new bsEditable(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            var script = new StringBuilder();
            var html = new StringBuilder();

            return new MvcHtmlString(Settings.GetHtml(script, html));
        }

    }

    public enum bsEditableType { None, Text }

    public class bsEditable : bsTagBegin
    {
        public bsEditableType Type = bsEditableType.Text;
        public string Label;

        public bsEditable(BaseModel model) : base(model)
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

                var binder = new bsEditableValueBinder<T>();

                binder.ModelGetMethod = getValueMethod;

                binder.Is2WayBinding = true;
                binder.jsOnChangeEventName = "change";
                binder.jsGetMethodName = "val";
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
                AddBinder(new bsEditableValueBinder<T>()
                {
                    Is2WayBinding = true,
                    ModelPropertyName = modelPropertyName,
                    jsOnChangeEventName = "change",
                    jsGetMethodName = "val"
                });
            }
            else
                throw new Exception(nameof(bsEditable) + ": неизвестный тип привязки значения '" + typeof(T).FullName + "'");
        }
        public override string GetHtml(StringBuilder script, StringBuilder html)
        {
            AddClass("bs-editable");

            if (Label == null)
                Label = "Введите значение";

            script.AppendLine(@"
var tag=$('#" + UniqueId + @">span');

tag.editable({
    type: 'text',
    title: " + Label.AsJavaScript() + @"
});

tag.on('shown', function(e, editable) {
    editable.input.$input.val(tag.text());
    });
");
            return base.GetHtml(script, html) + "<span>&nbsp;</span>&nbsp;<i class='fa fa-pencil-square-o'></i></" + Tag + ">";
        }
    }


}
