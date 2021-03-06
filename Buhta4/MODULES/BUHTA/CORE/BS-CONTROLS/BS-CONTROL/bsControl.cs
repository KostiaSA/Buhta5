﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Buhta
{
    public delegate void bsControlOnChangeEventHandler<SenderT, NewValueT>(SenderT sender, NewValueT newValue);
    public delegate string bsControlOnClickEventHandler<SenderT>(SenderT sender);

    public class bsControl
    {


        BaseModel model;
        public bsControl(BaseModel _model)
        {
            model = _model;
        }

        public BaseModel Model { get { return model; } }

        public List<BaseBinder> Binders = new List<BaseBinder>();
        protected void AddBinder(BaseBinder binder)
        {
            binder.Control = this;
            Binders.Add(binder);
        }

        protected void EmitBinders(StringBuilder script)
        {
            foreach (var binder in Binders)
            {
                Model.RegisterBinder(binder);
                binder.EmitBindingScript(script);
            }

        }


        List<string> classes = new List<string>();
        Dictionary<string, string> styles = new Dictionary<string, string>();
        Dictionary<string, string> attrs = new Dictionary<string, string>();

        public Dictionary<string, string> Styles { get { return styles; } }

        string uniqueId;
        public string UniqueId
        {
            get
            {
                if (uniqueId == null)
                {
                    uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
                }
                return uniqueId;
            }
            set
            {
                uniqueId = value;
            }
        }

        public List<bsWrapper> Wrappers = new List<bsWrapper>();

        public List<string> Classes { get { return classes; } }

        public void AddClass(string className)
        {
            var classNames = className.Split(' ');
            foreach (string cls in classNames)
            {
                if (!classes.Contains(cls))
                    classes.Add(cls);
            }
        }

        bool isAutoHeightToScreenBottom;
        string autoHeight_bootstrapTabId;
        int autoHeight_bottomPadding;
        public void ForceAutoHeightToScreenBottom(int bottomPadding, string bootstrapTabId = null)
        {
            isAutoHeightToScreenBottom = true;
            autoHeight_bottomPadding = bottomPadding;
            autoHeight_bootstrapTabId = bootstrapTabId;
        }

        public void AddStyle(string styleName, string styleValue)
        {
            styleName = styleName.ToLower();
            if (styles.ContainsKey(styleName))
                throw new Exception("Стиль '" + styleName + "' уже был добавлен");
            styles.Add(styleName, styleValue);
        }

        public void AddAttr(string attrName, string attrValue)
        {
            attrName = attrName.ToLower();
            if (attrName == "class")
                throw new Exception("Аттрибут 'class' надо добавлять методом  '" + nameof(AddClass) + "'");
            if (attrName == "style")
                throw new Exception("Аттрибут 'style' надо добавлять методом '" + nameof(AddStyle) + "'");
            if (attrs.ContainsKey(attrName))
                attrs[attrName] = attrValue; //throw new Exception("Аттрибут '" + attrName + "' уже был добавлен");
            else
                attrs.Add(attrName, attrValue);
        }

        public string GetAttrs()
        {
            var sb = new StringBuilder();
            if (classes.Count > 0 || classes.Count > 0)
            {
                sb.Append(@"class=""");
                foreach (var cls in classes)
                {
                    if (!classes.Contains(cls))
                        sb.Append(cls + " ");
                }
                foreach (var cls in classes)
                    sb.Append(cls + " ");
                sb.RemoveLastChar();
                sb.Append(@""" ");
            }
            if (styles.Keys.Count > 0 || styles.Keys.Count > 0)
            {
                sb.Append(@"style=""");
                foreach (var stl in styles)
                {
                    if (!styles.ContainsKey(stl.Key))
                        sb.Append(stl.Key + ":" + stl.Value + ";");
                }
                foreach (var stl in styles)
                    sb.Append(stl.Key + ":" + stl.Value + ";");
                sb.Append(@""" ");
            }

            foreach (var attr in attrs)
            {
                if (!attrs.ContainsKey(attr.Key))
                    sb.Append(attr.Key + "=" + attr.Value.AsJavaScript() + ";");
            }

            foreach (var attr in attrs)
            {
                sb.Append(attr.Key + "=" + attr.Value.AsJavaScript() + ";");
            }

            return sb.ToString();
        }

        public void Bind_OnTrueClass(string modelPropertyName, string className)
        {
            AddBinder(new ToggleClassBinder()
            {
                IsToogleOnTrue = true,
                ModelPropertyName = modelPropertyName,
                ClassName = className
            });
        }

        public void Bind_OnTrueClass(BinderGetMethod<bool> getValueMethod, string className)
        {
            AddBinder(new ToggleClassBinder
            {
                IsToogleOnTrue = true,
                ModelGetMethod = getValueMethod,
                ClassName = className
            });

        }
        public void Bind_OnFalseClass(string modelPropertyName, string className)
        {
            AddBinder(new ToggleClassBinder()
            {
                IsToogleOnTrue = false,
                ModelPropertyName = modelPropertyName,
                ClassName = className
            });
        }

        public void Bind_OnFalseClass(BinderGetMethod<bool> getValueMethod, string className)
        {
            AddBinder(new ToggleClassBinder
            {
                IsToogleOnTrue = false,
                ModelGetMethod = getValueMethod,
                ClassName = className
            });
        }

        public virtual void Bind_Text(string modelPropertyName)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "text"
            });
        }

        public virtual void Bind_Text(BinderGetMethod<string> getValueMethod)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "text"
            });
        }

        public string GetHtml()
        {
            var script = new StringBuilder();
            var html = new StringBuilder();
            EmitScriptAndHtml(script, html);

            var wrapperBeg = new StringBuilder();
            var wrapperEnd = new StringBuilder();

            foreach (var w in Wrappers)
                w.EmitHtml(wrapperBeg, wrapperEnd);


            if (script.Length > 0)
                return wrapperBeg.ToString() + "<script>\ndocReady(function(){\n" + script + "});\n</script>" + html + wrapperEnd.ToString();
            else
                return wrapperBeg.ToString() + html.ToString() + wrapperEnd.ToString();
        }

        public virtual void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {

            EmitBinders(script);

            if (isAutoHeightToScreenBottom)
            {
                // для невидимых(пока) закладок делаем установку height на событие Show, иначе позиция невидимого элемента вычисляется неверно
                if (autoHeight_bootstrapTabId != null)
                    script.AppendLine("$('#" + autoHeight_bootstrapTabId + "').on('shown.bs.tab', function(e) {");

                script.AppendLine("$('#" + UniqueId + "').css('height',(document.documentElement.clientHeight- $('#" + UniqueId + "').offset().top-" + autoHeight_bottomPadding + ").toString()+'px');");

                if (autoHeight_bootstrapTabId != null)
                    script.AppendLine("});");
            }

            //var wrapperBeg = new StringBuilder();
            //var wrapperEnd = new StringBuilder();

            //foreach (var w in Wrappers)
            //    w.EmitHtml(wrapperBeg, wrapperEnd);


            //if (script.Length > 0)
            //    //                return wrapperBeg.ToString() + "<script>\n$(document).ready( function(){\n $.connection.hub.start().done(function () { var tag =$('#" + UniqueId + "');\n" + Script + "})});\n</script>" + Html + wrapperEnd.ToString();
            //    return wrapperBeg.ToString() + "<script>\ndocReady(function(){\n" + script + "});\n</script>" + html + wrapperEnd.ToString();
            //else
            //    return wrapperBeg.ToString() + html.ToString() + wrapperEnd.ToString();
        }


    }

    public enum bsWrapCol { none = 0, col_1 = 1, col_2 = 2, col_3 = 3, col_4 = 4, col_5 = 5, col_6 = 6, col_7 = 7, col_8 = 8, col_9 = 9, col_10 = 10, col_11 = 11, col_12 = 12 }
    public class bsWrapper
    {

        public string Tag = "div";
        public bsWrapCol Col_xs = bsWrapCol.none;
        public bsWrapCol Col_sm = bsWrapCol.none;
        public bsWrapCol Col_md = bsWrapCol.none;
        public bsWrapCol Col_lg = bsWrapCol.none;
        public bool Form = false;
        public bool Row = false;
        public bool FormGroup = false;
        public bool InputGroup = false;
        public bool Container = false;
        public void EmitHtml(StringBuilder wBeg, StringBuilder wEnd)
        {
            wBeg.Append("<" + Tag + " class='");
            if (Col_xs != bsWrapCol.none)
                wBeg.Append("col-xs-" + Col_xs + " ");
            if (Col_sm != bsWrapCol.none)
                wBeg.Append("col-sm-" + Col_sm + " ");
            if (Col_md != bsWrapCol.none)
                wBeg.Append("col-md-" + Col_md + " ");
            if (Col_lg != bsWrapCol.none)
                wBeg.Append("col-lg-" + Col_lg + " ");
            if (Form)
                wBeg.Append("form ");
            if (Row)
                wBeg.Append("row ");
            if (FormGroup)
                wBeg.Append("form-group ");
            if (InputGroup)
                wBeg.Append("input-group ");
            if (Container)
                wBeg.Append("container ");
            wBeg.Append("'>");
            wEnd.Insert(0, "</" + Tag + ">");
        }

    }

}
