using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Buhta
{
    public delegate void bsControlOnChangeEventHandler<SenderT, NewValueT>(SenderT sender, NewValueT newValue);
    public delegate string bsControlOnClickEventHandler<SenderT>(SenderT sender);

    public class bsControlSettings
    {
        protected StringBuilder Script = new StringBuilder();
        protected StringBuilder Html = new StringBuilder();

        BaseModel model;
        public bsControlSettings(BaseModel _model)
        {
            model = _model;
        }

        public BaseModel Model { get { return model; } }

        protected List<dynamic> Binders = new List<dynamic>();
        protected void AddBinder(dynamic binder)
        {

            binder.Control = this;
            Binders.Add(binder);

        }

        protected void EmitBinders(StringBuilder script)
        {
            foreach (dynamic binder in Binders)
                binder.EmitBindingScript_M(script);
        }


        List<string> classes = new List<string>();
        Dictionary<string, string> styles = new Dictionary<string, string>();
        Dictionary<string, string> attrs = new Dictionary<string, string>();

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

        public TagInTable InTable = TagInTable.None;
        //public string ClassAttr;
        //public string StyleAttr;
        public List<bsWrapper> Wrappers = new List<bsWrapper>();

        public void AddClass(string className)
        {
            if (!classes.Contains(className))
                classes.Add(className);
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

        public void EmitProperty_M(StringBuilder script, string jqxMethodName, object value)
        {
            if (value != null)
                Script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + value.AsJavaScript() + ");");
        }


        public void EmitEvent_Bind(StringBuilder script, string modelMethodName, string jqxEventName)
        {
            if (modelMethodName != null)
            {
                Script.AppendLine("$('#" + UniqueId + "').on('" + jqxEventName + "',function(event){");
                Script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
                Script.AppendLine(" bindingHub.server.sendEvent('" + Model.BindingId + "','" + modelMethodName + "', args );");
                Script.AppendLine("});");

            }

        }
        public void EmitProperty_Bind_M(StringBuilder script, string modelPropertyName, string jqxMethodName)
        {
            if (modelPropertyName != null)
            {
                if (!Model.BindedProps.ContainsKey(modelPropertyName))
                {
                    Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
                }
                script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
                script.AppendLine("    $('#" + UniqueId + "')" + jqxMethodName + "(newValue);");
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
                script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
                script.AppendLine("    $('#" + UniqueId + "')" + jqxMethodName + "(newValue);");
                script.AppendLine("});");

                script.AppendLine("$('#" + UniqueId + "')on('" + jqxEventName + "', function () {");
                script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + modelPropertyName + "',$('#" + UniqueId + "')" + jqxMethodName + "()); ");
                script.AppendLine("}); ");

            }

        }

        public void EmitProperty_Bind2Way_Checked(StringBuilder script, BaseBinder binder, string jqxEventName)
        {
            if (binder != null)
            {
                Model.RegisterBinder(binder);
                binder.LastSendedText = Model.GetPropertyDisplayText(binder);
                script.AppendLine("$('#" + UniqueId + "').prop('checked'," + binder.LastSendedText + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',function(newValue){");
                script.AppendLine("    $('#" + UniqueId + "').prop('checked',newValue==='true');");
                script.AppendLine("});");

                script.AppendLine("$('#" + UniqueId + "').on('" + jqxEventName + "', function () {");
                script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',$('#" + UniqueId + "').prop('checked')); ");
                script.AppendLine("}); ");

            }

        }

        public void EmitProperty_StringBinder(StringBuilder script, StringBinder binder, string jqxMethodName)
        {
            if (binder != null)
            {
                Model.RegisterBinder(binder); 
                binder.LastSendedText = binder.GetDisplayText();// Model.GetPropertyDisplayText(binder);
                script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + binder.LastSendedText.AsJavaScript() + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',function(newValue){");
                script.AppendLine("    $('#" + UniqueId + "')." + jqxMethodName + "(newValue);");
                script.AppendLine("});");

                //script.AppendLine("tag.on('" + jqxEventName + "', function () {");
                //script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',tag.prop('checked')); ");
                //script.AppendLine("}); ");

            }

        }

        public virtual string GetHtml()
        {
            var wrapperBeg = new StringBuilder();
            var wrapperEnd = new StringBuilder();

            foreach (var w in Wrappers)
                w.EmitHtml(wrapperBeg, wrapperEnd);


            if (Script.Length > 0)
                //                return wrapperBeg.ToString() + "<script>\n$(document).ready( function(){\n $.connection.hub.start().done(function () { var tag =$('#" + UniqueId + "');\n" + Script + "})});\n</script>" + Html + wrapperEnd.ToString();
                return wrapperBeg.ToString() + "<script>\ndocReady(function(){\n" + Script + "});\n</script>" + Html + wrapperEnd.ToString();
            else
                return wrapperBeg.ToString() + Html.ToString() + wrapperEnd.ToString();
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

    //public class bsControl<T> where T : bsControlSettings, new()
    //{
    //    public BaseModel Model;
    //    public T Settings;

    //    protected StringBuilder Script = new StringBuilder();
    //    protected StringBuilder Html = new StringBuilder();

    //    //private List<string> classes = new List<string>();
    //    //private Dictionary<string, string> styles = new Dictionary<string, string>();
    //    //private Dictionary<string, string> attrs = new Dictionary<string, string>();

    //    //protected void AddClass(string className)
    //    //{
    //    //    if (!classes.Contains(className))
    //    //        classes.Add(className);
    //    //}

    //    //protected void AddStyle(string styleName, string styleValue)
    //    //{
    //    //    styleName = styleName.ToLower();
    //    //    if (styles.ContainsKey(styleName))
    //    //        throw new Exception("Стиль '" + styleName + "' уже был добавлен");
    //    //    styles.Add(styleName, styleValue);
    //    //}

    //    //protected void AddAttr(string attrName, string attrValue)
    //    //{
    //    //    attrName = attrName.ToLower();
    //    //    if (attrName == "class")
    //    //        throw new Exception("Аттрибут 'class' надо добавлять методом  '" + nameof(AddClass) + "'");
    //    //    if (attrName == "style")
    //    //        throw new Exception("Аттрибут 'style' надо добавлять методом '" + nameof(AddStyle) + "'");
    //    //    if (attrs.ContainsKey(attrName))
    //    //        attrs[attrName] = attrValue; //throw new Exception("Аттрибут '" + attrName + "' уже был добавлен");
    //    //    else
    //    //        attrs.Add(attrName, attrValue);
    //    //}

    //    public bsControl(object model, T settings)
    //    {
    //        Model = (BaseModel)model;
    //        Settings = settings;
    //    }

    //    public bsControl(object model, Action<T> _settings)
    //    {
    //        Model = (BaseModel)model;
    //        Settings = new T();
    //        _settings(Settings);
    //    }

    //    //public string GetClassAttr()
    //    //{
    //    //    if (Class.Length>0)
    //    //        return " class='" + Class + "' ";
    //    //    else
    //    //        return "";
    //    //}

    //    //public string GetStyleAttr()
    //    //{
    //    //    if (Style.Length>0)
    //    //        return " style='" + Style + "' ";
    //    //    else
    //    //        return "";
    //    //}

    //    //public string GetAttrs()
    //    //{
    //    //    var sb = new StringBuilder();
    //    //    if (classes.Count > 0 || Settings.classes.Count > 0)
    //    //    {
    //    //        sb.Append(@"class=""");
    //    //        foreach (var cls in classes)
    //    //        {
    //    //            if (!Settings.classes.Contains(cls))
    //    //                sb.Append(cls + " ");
    //    //        }
    //    //        foreach (var cls in Settings.classes)
    //    //            sb.Append(cls + " ");
    //    //        sb.RemoveLastChar();
    //    //        sb.Append(@""" ");
    //    //    }
    //    //    if (styles.Keys.Count>0 || Settings.styles.Keys.Count > 0)
    //    //    {
    //    //        sb.Append(@"style=""");
    //    //        foreach (var stl in styles)
    //    //        {
    //    //            if (!Settings.styles.ContainsKey(stl.Key))
    //    //                sb.Append(stl.Key + ":" + stl.Value + ";");
    //    //        }
    //    //        foreach (var stl in Settings.styles)
    //    //            sb.Append(stl.Key + ":" + stl.Value + ";");
    //    //        sb.Append(@""" ");
    //    //    }

    //    //    foreach (var attr in attrs)
    //    //    {
    //    //        if (!Settings.attrs.ContainsKey(attr.Key))
    //    //            sb.Append(attr.Key + "=" + attr.Value.AsJavaScript() + ";");
    //    //    }

    //    //    foreach (var attr in Settings.attrs)
    //    //    {
    //    //        sb.Append(attr.Key + "=" + attr.Value.AsJavaScript() + ";");
    //    //    }

    //    //    return sb.ToString();
    //    //}



    //    //public virtual string GetJqxName()
    //    //{
    //    //    throw new Exception("метод '" + nameof(GetJqxName) + "' не реализован для " + GetType().Name);
    //    //}

    //    public void EmitBeginScript(StringBuilder script, bool skipJqxInit = false)
    //    {
    //        //Script.AppendLine("var tag =$('#" + UniqueId + "');");
    //        //if (!skipJqxInit)
    //        //    Script.AppendLine("tag." + GetJqxName() + "({theme:'bootstrap'});");
    //    }

    //    //public void EmitProperty(StringBuilder script, string jqxPropertyName, object value)
    //    //{
    //    //    if (value != null)
    //    //        Script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + value.AsJavaScript() + "});");
    //    //}


    //    //public void EmitProperty_Px(StringBuilder script, string jqxPropertyName, int? value)
    //    //{
    //    //    if (value != null)
    //    //        Script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":'" + value + "px'});");
    //    //}

    //    public void EmitProperty_M(StringBuilder script, string jqxMethodName, object value)
    //    {
    //        if (value != null)
    //            Script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + value.AsJavaScript() + ");");
    //    }


    //    public void EmitEvent_Bind(StringBuilder script, string modelMethodName, string jqxEventName)
    //    {
    //        if (modelMethodName != null)
    //        {
    //            Script.AppendLine("$('#" + UniqueId + "').on('" + jqxEventName + "',function(event){");
    //            Script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
    //            Script.AppendLine(" bindingHub.server.sendEvent('" + Model.BindingId + "','" + modelMethodName + "', args );");
    //            Script.AppendLine("});");

    //        }

    //    }

    //    //public void EmitProperty_Bind(StringBuilder script, string modelPropertyName, string jqxPropertyName)
    //    //{
    //    //    if (modelPropertyName != null)
    //    //    {

    //    //        if (!Model.BindedProps.ContainsKey(modelPropertyName))
    //    //        {
    //    //            Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
    //    //        }
    //    //        script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + Model.BindedProps[modelPropertyName] + "});");
    //    //        script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
    //    //        script.AppendLine("    tag." + GetJqxName() + "({" + jqxPropertyName + ":newValue});");
    //    //        script.AppendLine("});");
    //    //    }

    //    //}

    //    //public void EmitProperty_Bind2Way(StringBuilder script, string modelPropertyName, string jqxPropertyName, string jqxEventName)
    //    //{
    //    //    if (modelPropertyName != null)
    //    //    {

    //    //        if (!Model.BindedProps.ContainsKey(modelPropertyName))
    //    //        {
    //    //            Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
    //    //        }
    //    //        script.AppendLine("tag." + GetJqxName() + "({" + jqxPropertyName + ":" + Model.BindedProps[modelPropertyName] + "});");
    //    //        script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
    //    //        script.AppendLine("    tag." + GetJqxName() + "({" + jqxPropertyName + ":newValue});");
    //    //        script.AppendLine("});");

    //    //        script.AppendLine("tag.on('" + jqxEventName + "', function () {");
    //    //        script.AppendLine("    bindingHub.server.sendBindedValueChanged('{{Model.BindingId}}', '{{settings.BindValueTo}}',tag." + GetJqxName() + "('" + jqxPropertyName + "'));");
    //    //        script.AppendLine("}); ");

    //    //    }

    //    //}

    //    public void EmitProperty_Bind_M(StringBuilder script, string modelPropertyName, string jqxMethodName)
    //    {
    //        if (modelPropertyName != null)
    //        {
    //            if (!Model.BindedProps.ContainsKey(modelPropertyName))
    //            {
    //                Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
    //            }
    //            script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
    //            script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
    //            script.AppendLine("    $('#" + UniqueId + "')." + jqxMethodName + "(newValue);");
    //            script.AppendLine("});");
    //        }

    //    }

    //    public void EmitProperty_Bind2Way_M(StringBuilder script, string modelPropertyName, string jqxMethodName, string jqxEventName)
    //    {
    //        if (modelPropertyName != null)
    //        {
    //            if (!Model.BindedProps.ContainsKey(modelPropertyName))
    //            {
    //                Model.BindedProps.Add(modelPropertyName, Model.GetPropertyValue(modelPropertyName).AsJavaScript());
    //            }
    //            script.AppendLine("$('#" + UniqueId + "')." + jqxMethodName + "(" + Model.BindedProps[modelPropertyName] + ");");
    //            script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + modelPropertyName + "',function(newValue){");
    //            script.AppendLine("    $('#" + UniqueId + "')." + jqxMethodName + "(newValue);");
    //            script.AppendLine("});");

    //            script.AppendLine("$('#" + UniqueId + "').on('" + jqxEventName + "', function () {");
    //            script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + modelPropertyName + "',$('#" + UniqueId + "')." + jqxMethodName + "()); ");
    //            script.AppendLine("}); ");

    //        }

    //    }

    //    public void EmitProperty_Bind2Way_Checked(StringBuilder script, BaseBinder binder, string jqxEventName)
    //    {
    //        if (binder != null)
    //        {
    //            Model.RegisterBinder(binder);
    //            binder.LastSendedText = Model.GetPropertyDisplayText(binder);
    //            script.AppendLine("$('#" + UniqueId + "').prop('checked'," + binder.LastSendedText + ");");
    //            script.AppendLine("signalr.subscribeModelPropertyChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',function(newValue){");
    //            script.AppendLine("    $('#" + UniqueId + "').prop('checked',newValue==='true');");
    //            script.AppendLine("});");

    //            script.AppendLine("$('#" + UniqueId + "').on('" + jqxEventName + "', function () {");
    //            script.AppendLine("    bindingHub.server.sendBindedValueChanged('" + Model.BindingId + "', '" + binder.PropertyName + "',$('#" + UniqueId + "').prop('checked')); ");
    //            script.AppendLine("}); ");

    //        }

    //    }


    //    public virtual string GetHtml()
    //    {
    //        var wrapperBeg = new StringBuilder();
    //        var wrapperEnd = new StringBuilder();

    //        foreach (var w in Settings.Wrappers)
    //            w.EmitHtml(wrapperBeg, wrapperEnd);


    //        if (Script.Length > 0)
    //            return wrapperBeg.ToString() + "<script>\ndocReady((function(){\n" + Script + "});\n</script>" + Html + wrapperEnd.ToString();
    //        else
    //            return wrapperBeg.ToString() + Html.ToString() + wrapperEnd.ToString();
    //    }

    //    //string uniqueId;
    //    //public string UniqueId
    //    //{
    //    //    get
    //    //    {
    //    //        if (uniqueId == null)
    //    //        {
    //    //            uniqueId = Guid.NewGuid().ToString().Substring(1, 6);
    //    //        }
    //    //        return uniqueId;
    //    //    }
    //    //}

    //}
}
