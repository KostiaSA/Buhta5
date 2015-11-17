using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Buhta
{
    public delegate void xControlOnChangeEventHandler<SenderT, NewValueT>(SenderT sender, NewValueT newValue);
    public delegate string xControlOnClickEventHandler<SenderT>(SenderT sender);

    public enum TagInTable { None, AsRow, AsDetail }

    public class xControlSettings
    {
        public TagInTable InTable = TagInTable.None;
        public string ClassAttr;
        public string StyleAttr;

        public string GetClassAttr()
        {
            if (ClassAttr != null)
                return " class='" + ClassAttr + "' ";
            else
                return "";
        }

        public string GetStyleAttr()
        {
            if (StyleAttr != null)
                return " style='" + StyleAttr + "' ";
            else
                return "";
        }

        //public bool? Visible = true;


        //public void BindProp(string modelProp, string controlProp);
        //public void BindFunc(string modelFunc, string controlFunc);
    }

    public class xControl<T> where T : xControlSettings, new()
    {
        public BaseModel Model;
        public T Settings;

        public xControl(object model, T settings)
        {
            Model = (BaseModel)model;
            Settings = settings;
        }

        public xControl(object model, Action<T> _settings)
        {
            Model = (BaseModel)model;
            Settings = new T();
            _settings(Settings);
        }

        //public xControl()
        //{
        //    Settings = new T(); ;
        //}
        //public xControl(T _settings)
        //{
        //    Settings = _settings;
        //}
        //public xControl(Action<T> _settings)
        //{
        //    Settings = new T();
        //    _settings(Settings);
        //}

        public virtual string GetJqxName()
        {
            throw new Exception("метод '" + nameof(GetJqxName) + "' не реализован для " + GetType().Name);
        }

        public void EmitBeginScript(StringBuilder script, bool skipJqxInit = false)
        {
            Script.AppendLine("var tag =$('#" + UniqueId + "');");
            if (!skipJqxInit)
                Script.AppendLine("tag." + GetJqxName() + "({theme:'bootstrap'});");
        }

        public void EmitProperty(StringBuilder script, string jqxPropertyName, object value)
        {
            if (value != null)
                Script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + value.AsJavaScript() + "});");
        }


        //public void EmitProperty_Px(StringBuilder script, string jqxPropertyName, int? value)
        //{
        //    if (value != null)
        //        Script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":'" + value + "px'});");
        //}

        public void EmitProperty_M(StringBuilder script, string jqxMethodName, object value)
        {
            if (value != null)
                Script.AppendLine("tag." + jqxMethodName + "(" + value.AsJavaScript() + ");");
        }


        public void EmitEvent_Bind(StringBuilder script, string modelMethodName, string jqxEventName)
        {
            if (modelMethodName != null)
            {
                Script.AppendLine("tag.on('" + jqxEventName + "',function(event){");
                Script.AppendLine(" var args={tagId:event.target.id};");
                Script.AppendLine(" bindingHub.server.sendEvent(window.name,'" + Model.BindingId + "','" + modelMethodName + "', args );");
                Script.AppendLine("});");

            }

        }

        public void EmitProperty_Bind(StringBuilder script, string modelPropertyName, string jqxPropertyName)
        {
            if (modelPropertyName != null)
            {

                if (!Model.BindedProps.ContainsKey(modelPropertyName))
                {
                    Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
                }
                script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + Model.BindedProps[modelPropertyName] + "});");
                script.AppendLine("signalr.subscribeModelPropertyChanged(window.name,'" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
                script.AppendLine("    tag." + GetJqxName() + "({" + jqxPropertyName + ":newValue});");
                script.AppendLine("});");
            }

        }

        //public void EmitProperty_Bind2Way(StringBuilder script, string modelPropertyName, string jqxPropertyName, string jqxEventName)
        //{
        //    if (modelPropertyName != null)
        //    {

        //        if (!Model.BindedProps.ContainsKey(modelPropertyName))
        //        {
        //            Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
        //        }
        //        script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + Model.BindedProps[modelPropertyName] + "});");
        //        script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
        //        script.AppendLine("    tag." + GetJqxName() + "({" + jqxPropertyName + ":newValue});");
        //        script.AppendLine("});");

        //        script.AppendLine("tag.on('" + jqxEventName + "', function () {");
        //        script.AppendLine("    bindingHub.server.sendBindedValueChanged('{{Model.BindingId}}', '{{settings.BindValueTo}}',tag." + GetJqxName() + "('" + jqxPropertyName + "'));");
        //        script.AppendLine("}); ");

        //    }

        //}

        public void EmitProperty_Bind_M(StringBuilder script, string modelPropertyName, string jqxMethodName)
        {
            if (modelPropertyName != null)
            {
                if (!Model.BindedProps.ContainsKey(modelPropertyName))
                {
                    Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
                }
                script.AppendLine("tag." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged(window.name,'" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
                script.AppendLine("    tag." + jqxMethodName + "(newValue);");
                script.AppendLine("});");
            }

        }

        public void EmitProperty_Bind2Way_M(StringBuilder script, string modelPropertyName, string jqxMethodName, string jqxEventName)
        {
            if (modelPropertyName != null)
            {
                if (!Model.BindedProps.ContainsKey(modelPropertyName))
                {
                    Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
                }
                script.AppendLine("tag." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged(window.name,'" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
                script.AppendLine("    tag." + jqxMethodName + "(newValue);");
                script.AppendLine("});");

                script.AppendLine("tag.on('" + jqxEventName + "', function () {");
                script.AppendLine("    bindingHub.server.sendBindedValueChanged(window.name,'" + Model.BindingId + "', '" + modelPropertyName + "',tag." + jqxMethodName + "()); ");
                script.AppendLine("}); ");

            }

        }

        public virtual string GetHtml()
        {
            return "<script>$(document).ready(function(){ " + Script + "});</script>" + Html;
        }

        string uniqueId;
        public string UniqueId
        {
            get
            {
                if (uniqueId == null)
                {
                    uniqueId = Guid.NewGuid().ToString().Substring(1, 6);
                }
                return uniqueId;
            }
        }

        protected StringBuilder Script = new StringBuilder();
        protected StringBuilder Html = new StringBuilder();
    }
}
