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

    public enum bsSelectSize { Default, Large, Small, ExtraSmall }

    public class bsSelect : bsControl
    {
        public bsSelect(BaseModel model) : base(model) { }

        public Type ValueType = typeof(String);

        public bool? Disabled;
        public string Disabled_Bind;

        public string Value = "";

        public void Bind_Value(BinderGetMethod getValueMethod)
        {
            Bind_Value_To_String(getValueMethod, null);
        }

        public void Bind_Value_To_String(BinderGetMethod getValueMethod, BinderSetMethod setValueMethod)
        {
            var binder = new TwoWayBinder();

            binder.ModelGetMethod = getValueMethod;
            binder.jsSetMethodName = "first()[0].selectize.setValue";

            binder.Is2WayBinding = true;
            binder.jsOnChangeEventName = "change";
            binder.jsGetMethodName = "first()[0].selectize.getValue";
            binder.ModelSetMethod = setValueMethod;

            AddBinder(binder);
        }

        public void Bind_Value_To_String(string modelPropertyName)
        {
            AddBinder(new TwoWayBinder()
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
            //KeyFieldName = keyFieldName;
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

        public bsSelectSize Size = bsSelectSize.Default;

        string GetDisplayText(object value)
        {
            return value.ToString();
        }


        public override string GetHtml()
        {

            if (Size == bsSelectSize.Large)
                AddClass("select-lg");
            else
            if (Size == bsSelectSize.Small)
                AddClass("select-sm");
            else
            if (Size == bsSelectSize.ExtraSmall)
                AddClass("select-xs");

            if (PlaceHolder != null)
                AddAttr("placeholder", PlaceHolder);

            Html.Append("<div class='form-group'>"); // begin form-group

            AddClass("form-control");

            if (Label != null)
            {
                Html.Append("<label class='col-sm-3 control-label' >" + Label + "</label>");
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

            return base.GetHtml();
        }
    }



}
