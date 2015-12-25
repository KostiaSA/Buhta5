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

        public static MvcHtmlString bsCodeMirror(this HtmlHelper helper, Action<bsCodeMirror> settings)
        {
            var tag = new bsCodeMirror(helper.ViewData.Model as BaseModel);
            settings(tag);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            return new MvcHtmlString(tag.GetHtml());
        }

    }

    public enum CodeMirrorMode { JavaScript, CSharp, Html, Css, Sql }
    public class bsCodeMirror : bsControl
    {
        public bsCodeMirror(BaseModel model) : base(model) { }

        public string Value;
        public CodeMirrorMode Mode;
        public bool IsReadOnly;

        public override string GetHtml()
        {

            JsObject init = new JsObject();
            if (Value != null)
                init.AddProperty("value", Value);
            if (IsReadOnly)
                init.AddProperty("readonly", IsReadOnly);
            switch (Mode)
            {
                case CodeMirrorMode.JavaScript:
                    init.AddProperty("mode", "javascript");
                    break;
                case CodeMirrorMode.Sql:
                    init.AddProperty("mode", "text/x-sql");
                    break;
                case CodeMirrorMode.CSharp:
                    init.AddProperty("mode", "clike");
                    break;
                case CodeMirrorMode.Css:
                    init.AddProperty("mode", "css");
                    break;
                case CodeMirrorMode.Html:
                    init.AddProperty("mode", "htmlmixed");
                    break;
                default:
                    throw new Exception(nameof(bsCodeMirror) + ": неизвестный " + nameof(Mode) + " '" + Mode.ToNameString() + "'");
            }

            Script.AppendLine("var editor=CodeMirror( $('#" + UniqueId + "')[0]," + init.ToJson() + ");");
            Script.AppendLine("setTimeout(editor.refresh, 0)");

            Html.Append(@"
<style>
.CodeMirror { 
  border:1px solid #eee; 
  height:auto; 
}
.CodeMirror-scroll { 
  overflow-y:hidden; 
  overflow-x:auto; 
}
</style>
");
            Html.Append("<div id='" + UniqueId + "' " + GetAttrs() + ">");
            return base.GetHtml();
        }


    }
}
