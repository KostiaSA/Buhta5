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
            var script = new StringBuilder();
            var html = new StringBuilder();

            return new MvcHtmlString(Settings.GetHtml(script, html));
        }
    }

    public class bsInputVisibleBinder : BaseBinder
    {
        public string ModelPropertyName;
        public BinderGetMethod<bool> ModelGetMethod;

        public bsInputVisibleBinder()
        {
            ValueType = typeof(bool);
        }

        public override string GetJsForSettingProperty()
        {
            if (ModelGetMethod == null && ModelPropertyName == null)
                throw new Exception(nameof(bsInputVisibleBinder) + ": модель '" + Control.Model.GetType().FullName + "', control '" + Control.GetType().FullName + "' - для привязки нужно указать или имя свойства или get-метод");

            bool visible = false;
            if (ModelGetMethod != null)
                visible = ModelGetMethod();
            else
            if (ModelPropertyName != null)
            {
                var disabled_obj = Control.Model.GetPropertyValue(ModelPropertyName);
                if (disabled_obj == null)
                    visible = false;
                else
                    visible = (bool)disabled_obj;

            }

            if (visible)
                return "$('#" + Control.UniqueId + "').parents('.form-group').first().removeClass('hidden');";
            else
                return "$('#" + Control.UniqueId + "').parents('.form-group').first().addClass('hidden');";

        }

        public override void EmitBindingScript(StringBuilder script)
        {
            //Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);

        }


    }

    public enum bsInputSize { Default, Large, Small, ExtraSmall }

    public enum bsInputType { Text, Checkbox, Radio, List, Lookup }

    public class bsInput : bsControl
    {
        public bsInput(BaseModel model) : base(model) { }

        public Type ValueType = typeof(String);

        public bsInputType Type = bsInputType.Text;

        //public bool? Disabled;
        //public string Disabled_Bind;

        public int? MaxWidth;

        public bool IsRequired;

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

        public void Bind_Disabled(string modelPropertyName)
        {
            AddBinder(new OneWayBinder<bool>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "prop",
                jsSetPropertyName = "disabled"
            });
        }

        public void Bind_Disabled(BinderGetMethod<bool> getValueMethod)
        {
            AddBinder(new OneWayBinder<bool>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "prop",
                jsSetPropertyName = "disabled"
            });
        }

        public void Bind_Visible(string modelPropertyName)
        {
            AddBinder(new bsInputVisibleBinder()
            {
                ModelPropertyName = modelPropertyName
            });
        }

        public void Bind_Visible(BinderGetMethod<bool> getValueMethod)
        {
            AddBinder(new bsInputVisibleBinder()
            {
                ModelGetMethod = getValueMethod
            });
        }


        public void Bind_Validator(string modelValidateMethodName)
        {
            AddBinder(new ValidatorBinder()
            {
                ModelValidateMethodName = modelValidateMethodName
            });
        }

        public void Bind_Validator(BinderValidateMethod modelValidateMethod)
        {
            AddBinder(new ValidatorBinder()
            {
                ModelValidateMethod = modelValidateMethod
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

        bsInputToSchemaObjectBinder schemaObjectBinder;
        public void Bind_Value_To_SchemaObject(string modelPropertyName, Type schemaObjectType)
        {
            Type = bsInputType.Lookup;
            schemaObjectBinder = new bsInputToSchemaObjectBinder()
            {
                ModelPropertyName = modelPropertyName,
                SchemaObjectType = schemaObjectType
            };
            AddBinder(schemaObjectBinder);
        }

        public void Bind_Value_To_SchemaObject(BinderGetMethod<Guid?> getValueMethod, BinderSetMethod<Guid?> setValueMethod, Type schemaObjectType)
        {
            Type = bsInputType.Lookup;
            schemaObjectBinder = new bsInputToSchemaObjectBinder()
            {
                ModelGetMethod = getValueMethod,
                ModelSetMethod = setValueMethod,
                SchemaObjectType = schemaObjectType
            };
            AddBinder(schemaObjectBinder);
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

        public override string GetHtml(StringBuilder script, StringBuilder html)
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


            html.Append("<div class='form-group'>"); // begin form-group

            if (Type == bsInputType.Text)
            {

                AddClass("form-control");

                if (Label != null)
                {

                    html.Append("<label class='col-sm-3 control-label' >" + Label.AsHtml());
                    if (IsRequired)
                        html.Append("<span style='color:#EF6F6C'>&nbsp;*</span>");
                    html.Append("</label>");
                    html.Append("<div class='col-sm-9'>");  // begin col-sm-9
                }

                html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + ">" + /*GetDisplayText(Value) +*/ "</input>");
                html.Append("<small id='" + UniqueId + "-error-text' class='error-text hidden'></small>");

                if (Label != null)
                {
                    html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Type == bsInputType.Checkbox)
            {

                html.Append("<div class='col-sm-offset-3 col-sm-9'>"); // beg col-sm-offset-3 col-sm-9
                html.Append("<div class='checkbox'>");
                html.Append("<label>");

                html.Append("<input id='" + UniqueId + "' type='" + Type.ToString().ToLower() + "' " + GetAttrs() + "></input>");
                html.Append(Label != null ? Label.AsHtmlEx() : "");

                html.Append("</label>");
                html.Append("</div>");
                html.Append("</div>");  // end col-sm-offset-3 col-sm-9
            }
            else
            if (Type == bsInputType.List)
            {

                AddClass("form-control");

                if (Label != null)
                {
                    //html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    html.Append("<label class='col-sm-3 control-label'>"+ Label.AsHtml());
                    if (IsRequired)
                        html.Append("<span style='color:#EF6F6C'>&nbsp;*</span>");
                    html.Append("</label>");
                    //html.Append("</div>");  // end col-sm-3
                    html.Append("<div class='col-sm-8'>");  // begin col-sm-9
                }


                html.Append("<div class='input-group bs-input'>");
                html.Append("<input id='" + UniqueId + "' type='text' " + GetAttrs() + ">" + GetDisplayText(Value).AsHtml() + "</input>");
                html.Append("<span class='input-group-btn'>");
                //html.Append("<div id='" + UniqueId + "-select-btn' class='btn btn-default' type='button'>Выбрать</div>");

                var selectButton = new bsButton(Model);
                selectButton.Text = "Выбрать";
                selectButton.Bind_OnClick(listBinder.SelectButtonClick);
                html.Append(selectButton.GetHtml(new StringBuilder(), new StringBuilder()));


                html.Append("</span>");
                html.Append("</div>");

                if (Label != null)
                {
                    html.Append("</div>");  // end col-sm-9
                }
            }
            else
            if (Type == bsInputType.Lookup)
            {

                AddClass("form-control");

                if (Label != null)
                {
                    //html.Append("<div class='col-sm-3'>"); // begin col-sm-3
                    html.Append("<label class='col-sm-3 control-label'>" + Label.AsHtml());
                    if (IsRequired)
                        html.Append("<span style='color:#EF6F6C'>&nbsp;*</span>");
                    html.Append("</label>");
                    //html.Append("</div>");  // end col-sm-3
                    html.Append("<div class='col-sm-8'>");  // begin col-sm-9
                }


                html.Append("<div class='input-group bs-input'>");
                html.Append("<input id='" + UniqueId + "' type='text' " + GetAttrs() + ">" + GetDisplayText(Value).AsHtml() + "</input>");
                html.Append("<span class='input-group-btn'>");
                //html.Append("<div id='" + UniqueId + "-select-btn' class='btn btn-default' type='button'>Выбрать</div>");

                var selectButton = new bsButton(Model);
                selectButton.Text = "Поиск";
                selectButton.Bind_OnClick(schemaObjectBinder.SelectButtonClick);
                html.Append(selectButton.GetHtml(new StringBuilder(), new StringBuilder()));


                html.Append("</span>");
                html.Append("</div>");

                if (Label != null)
                {
                    html.Append("</div>");  // end col-sm-9
                }
            }

            html.Append("</div>"); // end form-group

            return base.GetHtml(script, html);
        }
    }



}
