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
        //public static MvcHtmlString bsInput(this HtmlHelper helper, bsInputSettings settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// ""<new bsInput(helper.ViewData.Model, settings).GetHtml());
        //}

        //public static MvcHtmlString bsInput(this HtmlHelper helper, Action<bsInputSettings> settings)
        //{
        //    (helper.ViewData.Model as BaseModel).Helper = helper;
        //    return new MvcHtmlString("");// new bsInput(helper.ViewData.Model, settings).GetHtml());
        //}

        public static MvcHtmlString bsInput(this HtmlHelper helper, Action<bsInput> settings)
        {
            var Settings = new bsInput(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(Settings.GetHtml());
        }
    }

    public enum bsInputSize { Default, Large, Small, ExtraSmall }

    public enum bsInputType { Text, Checkbox, Radio, List }

    public class bsInput : bsControl
    {
        public bsInput(BaseModel model) : base(model) { }

        public Type ValueType = typeof(String);

        public bsInputType Type = bsInputType.Text;

        public bool? Disabled;
        public string Disabled_Bind;

        public int? MaxWidth;

        public string Value = "";

        public void Bind_OnChange(string modelEventMethodName)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "change"
            });
        }

        public void Bind_OnChange(BinderEventMethod eventMethod)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "change"
            });
        }


        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod)
        {
            Bind_Value<T>(getValueMethod, null);
        }

        public void Bind_Value<T>(BinderGetMethod<T> getValueMethod, BinderSetMethod<T> setValueMethod)
        {
            if (typeof(T) == typeof(string))
            {
                Type = bsInputType.Text;

                var binder = new TwoWayBinder<T>();

                binder.ModelGetMethod = getValueMethod;
                binder.jsSetMethodName = "val";

                binder.Is2WayBinding = true;
                binder.jsOnChangeEventName = "change";
                binder.jsGetMethodName = "val";
                binder.ModelSetMethod = setValueMethod;

                AddBinder(binder);
            }
            else
            if (typeof(T) == typeof(bool))
            {
                Type = bsInputType.Checkbox;

                var binder = new TwoWayBinder<T>();

                binder.ModelGetMethod = getValueMethod;
                binder.jsSetMethodName = "prop";
                binder.jsSetPropertyName = "checked";

                binder.Is2WayBinding = true;
                binder.jsOnChangeEventName = "change";
                binder.jsGetMethodName = "prop";
                binder.jsGetPropertyName = "checked";
                binder.ModelSetMethod = setValueMethod;

                AddBinder(binder);
            }
            else
                throw new Exception(nameof(bsInput) + ": неизвестный тип привязки значения '" + typeof(T).FullName + "'");

        }

        public void Bind_Value<T>(string modelPropertyName)
        {

            if (typeof(T) == typeof(string))
            {
                Type = bsInputType.Text;
                AddBinder(new TwoWayBinder<T>()
                {
                    ModelPropertyName = modelPropertyName,
                    jsSetMethodName = "val",
                    Is2WayBinding = true,
                    jsOnChangeEventName = "change",
                    jsGetMethodName = "val"
                });
            }
            else
            if (typeof(T) == typeof(bool))
            {
                Type = bsInputType.Checkbox;

                AddBinder(new TwoWayBinder<bool>()
                {
                    ModelPropertyName = modelPropertyName,
                    jsSetMethodName = "prop",
                    jsSetPropertyName = "checked",
                    Is2WayBinding = true,
                    jsOnChangeEventName = "change",
                    jsGetMethodName = "prop",
                    jsGetPropertyName = "checked"
                });
            }
            else
                throw new Exception(nameof(bsInput) + ": неизвестный тип привязки значения '" + typeof(T).FullName + "'");
        }


        bsInputToRolesListBinder listBinder;
        public void Bind_Value_To_RolesList(string modelPropertyName, SchemaBaseRole rootRole)
        {
            Type = bsInputType.List;
            listBinder = new bsInputToRolesListBinder()
            {
                ModelPropertyName = modelPropertyName,
                RootRole = rootRole
            };
            AddBinder(listBinder);
        }


        public string Label;
        public string PlaceHolder;

        public string Image;

        public bsInputSize Size = bsInputSize.Default;

        string GetDisplayText(object value)
        {
            //if (Lookup != null)
            //    return Lookup.GetDisplayText(value);
            //else
            return value.ToString();
        }

        public override string GetHtml()
        {
            if (MaxWidth != null)
                AddAttr("max-width", MaxWidth + "px");

            if (Size == bsInputSize.Large)
                AddClass("input-lg");
            else
            if (Size == bsInputSize.Small)
                AddClass("input-sm");
            else
            if (Size == bsInputSize.ExtraSmall)
                AddClass("input-xs");

            if (PlaceHolder != null)
                AddAttr("placeholder", PlaceHolder);


            Html.Append("<div class='form-group'>"); // begin form-group

            if (Type == bsInputType.Text)
            {

                AddClass("form-control");

                if (Label != null)
                {
                    Html.Append("<label class='col-sm-3 control-label' >" + Label + "</label>");
                    Html.Append("<div class='col-sm-9'>");  // begin col-sm-9
                }

                Html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + ">" + /*GetDisplayText(Value) +*/ "</input>");

                if (Label != null)
                {
                    Html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Type == bsInputType.Checkbox)
            {

                Html.Append("<div class='col-sm-offset-3 col-sm-9'>"); // beg col-sm-offset-3 col-sm-9
                Html.Append("<div class='checkbox'>");
                Html.Append("<label>");

                Html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + "></input>");
                Html.Append(Label != null ? Label.ToString() : "");

                Html.Append("</label>");
                Html.Append("</div>");
                Html.Append("</div>");  // end col-sm-offset-3 col-sm-9
            }
            else
            if (Type == bsInputType.List)
            {

                AddClass("form-control");

                if (Label != null)
                {
                    //Html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    Html.Append("<label class='col-sm-3 control-label' >" + Label + "</label>");
                    //Html.Append("</div>");  // end col-sm-3
                    Html.Append("<div class='col-sm-8'>");  // begin col-sm-9
                }


                Html.Append("<div class='input-group bs-input'>");
                Html.Append("<input id='" + UniqueId + "' type='text' " + GetAttrs() + ">" + GetDisplayText(Value) + "</input>");
                Html.Append("<span class='input-group-btn'>");
                //Html.Append("<div id='" + UniqueId + "-select-btn' class='btn btn-default' type='button'>Выбрать</div>");

                var selectButton = new bsButton(Model);
                selectButton.Text = "Выбрать";
                selectButton.Bind_OnClick(listBinder.SelectButtonClick);
                Html.Append(selectButton.GetHtml());


                Html.Append("</span>");
                Html.Append("</div>");

                if (Label != null)
                {
                    Html.Append("</div>");  // end col-sm-9
                }
            }

            Html.Append("</div>"); // end form-group

            return base.GetHtml();
        }
    }



}
