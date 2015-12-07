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
        public static MvcHtmlString bsSelect(this HtmlHelper helper, Action<bsSelect> settings)
        {
            var tag = new bsSelect(helper.ViewData.Model as BaseModel);
            settings(tag);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(tag.GetHtml());
        }
    }

    public class bsSelectDisabledBinder : BaseBinder
    {
        public string ModelPropertyName;
        public BinderGetMethod<bool> ModelGetMethod;

        public bsSelectDisabledBinder()
        {
            ValueType = typeof(bool);
        }

        public override string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(bsSelectDisabledBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            bool disabled = false;
            if (ModelGetMethod != null)
                disabled = ModelGetMethod();
            else
            if (ModelPropertyName != null)
            {
                var disabled_obj = Control.Model.GetPropertyValue(ModelPropertyName);
                if (disabled_obj == null)
                    disabled = false;
                else
                    disabled = (bool)disabled_obj;

            }

            if (disabled)
                return "$('#" + Control.UniqueId + "')[0].selectize.disable();";
            else
                return "$('#" + Control.UniqueId + "')[0].selectize.enable();";

        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

        }


    }

    //public enum bsSelectSize { Default, Large, Small, ExtraSmall }

    public class bsSelect : bsControl
    {
        public bsSelect(BaseModel model) : base(model) { }

        public int? MaxWidth;
        public bool IsRequired;

        public Type ValueType = typeof(String);

        public void Bind_Disabled(string modelPropertyName)
        {
            AddBinder(new bsSelectDisabledBinder()
            {
                ModelPropertyName = modelPropertyName
            });
        }

        public void Bind_Disabled(BinderGetMethod<bool> getValueMethod)
        {
            AddBinder(new bsSelectDisabledBinder()
            {
                ModelGetMethod = getValueMethod
            });
        }

        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod)
        {
            Bind_Value<T>(getValueMethod, null);
        }

        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod, BinderSetMethod<T> setValueMethod)
        {
            if (optionsBinderToObjectsList == null)
                throw new Exception(nameof(bsSelect) + ": сначала надо привязать '" + nameof(Bind_Options_To_ObjectsList) + "', а уже затем '" + nameof(Bind_Value) + "'");
            var binder = new TwoWayBinder<T>();

            binder.ModelGetMethod = getValueMethod;
            binder.jsSetMethodName = "first()[0].selectize.setValue";

            binder.Is2WayBinding = true;
            binder.jsOnChangeEventName = "change";
            binder.jsGetMethodName = "first()[0].selectize.getValue";
            binder.ModelSetMethod = setValueMethod;

            AddBinder(binder);
        }

        public void Bind_Value<T>(string modelPropertyName)
        {
            if (optionsBinderToObjectsList == null)
                throw new Exception(nameof(bsSelect) + ": сначала надо привязать '"+ nameof(Bind_Options_To_ObjectsList) + "', а уже затем '" + nameof(Bind_Value) + "' к '"+ modelPropertyName + "'");

            AddBinder(new TwoWayBinder<T>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "first()[0].selectize.setValue",
                Is2WayBinding = true,
                jsOnChangeEventName = "change",
                jsGetMethodName = "first()[0].selectize.getValue"
            });
        }

        bsSelectOptionsToObjectsListBinder optionsBinderToObjectsList;
        public void Bind_Options_To_ObjectsList(string datasourceModelPropertyName, string keyFieldName, string titleFieldName = null, string sortFieldName = null)
        {
            optionsBinderToObjectsList = new bsSelectOptionsToObjectsListBinder()
            {
                Select = this,
                DatasourceModelPropertyName = datasourceModelPropertyName,
                KeyFieldName = keyFieldName,
                TitleFieldName = titleFieldName,
                SortFieldName = sortFieldName

            };
            AddBinder(optionsBinderToObjectsList);
        }



        public string Label;
        public string PlaceHolder;

        public string Image;

        //        public bsSelectSize Size = bsSelectSize.Default;

        string GetDisplayText(object value)
        {
            return value.ToString();
        }


        public override string GetHtml()
        {

            //if (Size == bsSelectSize.Large)
            //    AddClass("input-lg");
            //else
            //if (Size == bsSelectSize.Small)
            //    AddClass("input-sm");
            //else
            //if (Size == bsSelectSize.ExtraSmall)
            //    AddClass("input-xs");

            if (PlaceHolder != null)
                AddAttr("placeholder", PlaceHolder);

            Html.Append("<div class='form-group'>"); // begin form-group

            AddClass("form-control");

            if (Label != null)
            {
                Html.Append("<label class='col-sm-3 control-label' >" + Label.AsHtml());
                if (IsRequired)
                    Html.Append("<span style='color:#EF6F6C'>&nbsp;*</span>");
                Html.Append("</label>");

                Html.Append("<div class='col-sm-9'>");  // begin col-sm-9
            }

            Html.Append("<select id='" + UniqueId + "' " + GetAttrs() + "></select>");

            if (Label != null)
            {
                Html.Append("</div>");  // end col-sm-9
            }

            Html.Append("</div>"); // end form-group


            JsObject init = new JsObject();
            init.AddProperty("dropdownParent", "body");

            if (optionsBinderToObjectsList != null)
            {
                init.AddProperty("valueField", "id");
                if (optionsBinderToObjectsList.TitleFieldName != null)
                    init.AddProperty("labelField", "title");
                else
                    init.AddProperty("labelField", "id");
                if (optionsBinderToObjectsList.SortFieldName != null)
                    init.AddProperty("sortField", "sort");
            }

            Script.AppendLine("$('#" + UniqueId + "').selectize(" + init.ToJson() + ");");
            if (MaxWidth != null)
                Script.AppendLine("$('#" + UniqueId + "').parent().first().find('.selectize-control').css('max-width','" + MaxWidth + "px');");


            return base.GetHtml();
        }
    }



}
