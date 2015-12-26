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
            return new MvcHtmlString(tag.GetHtml());
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


        public override string GetHtml()
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

            //  Script.AppendLine("var editor=Ace( $('#" + UniqueId + "')[0]," + init.ToJson() + ");");

            Script.AppendLine("var editor = ace.edit('" + UniqueId + "');");
            Script.AppendLine("$('#" + UniqueId + "')[0]['editor']=editor;");
            Script.AppendLine("editor.setTheme('ace/theme/sqlserver');");
            Script.AppendLine("editor.getSession().setMode('ace/mode/sqlserver');");
     //       Script.AppendLine("editor.setOptions({maxLines: Infinity});");
            Script.AppendLine("editor.renderer.setShowGutter(false);");


            if (Value != null)
                Script.AppendLine("editor.setValue(" + Value.AsJavaScript() + ", 1);");

            //Script.AppendLine("$('#" + UniqueId + "').css('height',(document.documentElement.clientHeight- $('#" + UniqueId + "').offset().top-10).toString()+'px');");
            //Script.AppendLine("console.log('ace-height='+(document.documentElement.clientHeight- $('#" + UniqueId + "').parent.offset().top-10).toString()+'px');");


            //            Script.AppendLine(@"
            //editor.on('change', function() {
            //  var lineHeight = 16; // assuming a 16px line height
            //  $('#" + UniqueId + @"')[0].style.height = lineHeight * editor.getSession().getDocument().getLength() + 'px';
            //  editor.resize();
            //});
            //        ");

            Html.Append("<div id='" + UniqueId + "' " + GetAttrs() + "></div>");
            return base.GetHtml();
        }


    }
}
