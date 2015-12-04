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
            binder.jsSetMethodName = "val";

            binder.Is2WayBinding = true;
            binder.jsOnChangeEventName = "change";
            binder.jsGetMethodName = "val";
            binder.ModelSetMethod = setValueMethod;

            AddBinder(binder);
        }

        public void Bind_Value_To_String(string modelPropertyName)
        {
            AddBinder(new TwoWayBinder()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "val",
                Is2WayBinding = true,
                jsOnChangeEventName = "change",
                jsGetMethodName = "val"
            });
        }

        //bsGridDataSourceToObjectsListBinder dataSourceBinderToObjectsList;
        //public void Bind_DataSource_To_ObjectsList(string datasourceModelPropertyName, string keyFieldName = null, string iconFieldName = null, string selectedRowsModelPropertyName = null)
        //{
        //    KeyFieldName = keyFieldName;
        //    dataSourceBinderToObjectsList = new bsGridDataSourceToObjectsListBinder()
        //    {
        //        Tree = this,
        //        DatasourceModelPropertyName = datasourceModelPropertyName,
        //        KeyFieldName = keyFieldName,
        //        IconFieldName = iconFieldName,
        //        SelectedRowsModelPropertyName = selectedRowsModelPropertyName

        //    };
        //    AddBinder(dataSourceBinderToObjectsList);
        //}



        public string Label;
        public string PlaceHolder;

        public string Image;

        public bsSelectSize Size = bsSelectSize.Default;

        string GetDisplayText(object value)
        {
            //if (Lookup != null)
            //    return Lookup.GetDisplayText(value);
            //else
            return value.ToString();
        }

        //object ParseDisplayText(string text)
        //{
        //    if (Lookup != null)
        //        return Lookup.ParseDisplayText(text);
        //    else
        //    {
        //        if (ValueType == typeof(String))
        //            return text;
        //        else
        //        if (ValueType == typeof(int))
        //            return int.Parse(text);
        //        else
        //        if (ValueType == typeof(Decimal))
        //            return Decimal.Parse(text);
        //        else
        //            return nameof(ParseDisplayText) + ": неизвестный тип '" + ValueType.FullName + "'";
        //    }
        //}

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

            // EmitProperty_Bind2Way_M(Script, Value_Bind, "val", "change");
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
            init.AddProperty("valueField", "id");
            init.AddProperty("labelField", "title");

            var options = new jsArray();

            var item1 = new JsObject();
            item1.AddProperty("id",1);
            item1.AddProperty("title", "Один");
            options.AddObject(item1);

            var item2 = new JsObject();
            item2.AddProperty("id", 2);
            item2.AddProperty("title", "Два");
            options.AddObject(item2);

            init.AddProperty("options", options);

            Script.AppendLine("$('#" + UniqueId + "').selectize(" + init.ToJson() + ");");

            return base.GetHtml();
        }
    }



}
