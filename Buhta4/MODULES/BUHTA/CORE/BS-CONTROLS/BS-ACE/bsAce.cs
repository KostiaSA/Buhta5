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

        public static MvcHtmlString bsAce(this HtmlHelper helper, Action<bsAce> settings)
        {
            var tag = new bsAce(helper.ViewData.Model as BaseModel);
            settings(tag);

            (helper.ViewData.Model as BaseModel).Helper = helper;

            var script = new StringBuilder();
            var html = new StringBuilder();

            return new MvcHtmlString(tag.GetHtml());
            //return new MvcHtmlString(tag.EmitScriptAndHtml(script, html));
        }

    }

    public enum AceMode { JavaScript, CSharp, Html, Css, Sql }
    public class bsAce : bsControl
    {
        public bsAce(BaseModel model) : base(model) { }

        public string Value;
        public AceMode Mode;
        public bool IsReadOnly;

        public override void Bind_Text(string modelPropertyName)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelPropertyName = modelPropertyName,
                jsSetMethodName = "first()[0]['editor'].setValue"
            });
        }

        public override void Bind_Text(BinderGetMethod<string> getValueMethod)
        {
            AddBinder(new OneWayBinder<string>()
            {
                ModelGetMethod = getValueMethod,
                jsSetMethodName = "first()[0]['editor'].setValue"
            });
        }


        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {

            //JsObject init = new JsObject();
            //if (Value != null)
            //    init.AddProperty("value", Value);
            //if (IsReadOnly)
            //    init.AddProperty("readonly", IsReadOnly);
            //switch (Mode)
            //{
            //    case AceMode.JavaScript:
            //        init.AddProperty("mode", "javascript");
            //        break;
            //    case AceMode.Sql:
            //        init.AddProperty("mode", "text/x-sql");
            //        break;
            //    case AceMode.CSharp:
            //        init.AddProperty("mode", "clike");
            //        break;
            //    case AceMode.Css:
            //        init.AddProperty("mode", "css");
            //        break;
            //    case AceMode.Html:
            //        init.AddProperty("mode", "htmlmixed");
            //        break;
            //    default:
            //        throw new Exception(nameof(bsAce) + ": неизвестный " + nameof(Mode) + " '" + Mode.ToNameString() + "'");
            //}

            //  script.AppendLine("var editor=Ace( $('#" + UniqueId + "')[0]," + init.ToJson() + ");");

            script.AppendLine("var editor = ace.edit('" + UniqueId + "');");
            script.AppendLine("$('#" + UniqueId + "')[0]['editor']=editor;");
            script.AppendLine("editor.setTheme('ace/theme/sqlserver');");
            script.AppendLine("editor.getSession().setMode('ace/mode/sqlserver');");
     //       script.AppendLine("editor.setOptions({maxLines: Infinity});");
            script.AppendLine("editor.renderer.setShowGutter(false);");


            if (Value != null)
                script.AppendLine("editor.setValue(" + Value.AsJavaScript() + ", 1);");

            //script.AppendLine("$('#" + UniqueId + "').css('height',(document.documentElement.clientHeight- $('#" + UniqueId + "').offset().top-10).toString()+'px');");
            //script.AppendLine("console.log('ace-height='+(document.documentElement.clientHeight- $('#" + UniqueId + "').parent.offset().top-10).toString()+'px');");


            //            script.AppendLine(@"
            //editor.on('change', function() {
            //  var lineHeight = 16; // assuming a 16px line height
            //  $('#" + UniqueId + @"')[0].style.height = lineHeight * editor.getSession().getDocument().getLength() + 'px';
            //  editor.resize();
            //});
            //        ");

            html.Append("<div id='" + UniqueId + "' " + GetAttrs() + "></div>");
            base.EmitScriptAndHtml(script, html);
        }


    }
}
