using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class xInputSettings : xControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public int? Width;
        public string Width_Bind;

        public int? Height;
        public string Height_Bind;

        public string Text;
        public string Text_Bind;
        public string Text_OnChange_Bind;

        public string Label;
        public string Label_Bind;

        public event InputControlOnChangeEventHandler OnChange;

        public void FireOnChange(xInput sender, object newValue)
        {
            if (OnChange != null)
                OnChange(sender, newValue);
        }

    }

    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString xInput(this HtmlHelper helper, xInputSettings settings)
        {
            return new MvcHtmlString(new xInput(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xInput(this HtmlHelper helper, Action<xInputSettings> settings)
        {

            return new MvcHtmlString(new xInput(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public delegate void InputControlOnChangeEventHandler(xInput sender, object newValue);

    public class xInput : xControl<xInputSettings>
    {
        public override string GetJqxName()
        {
            return "jqxInput";
        }

        public xInput(object model, xInputSettings settings) : base(model, settings) { }
        public xInput(object model, Action<xInputSettings> settings) : base(model, settings) { }


        public override string GetHtml()
        {
            EmitBeginScript(Script);


            EmitProperty_Px(Script, "width", Settings.Width);
            EmitProperty_Bind(Script, Settings.Width_Bind, "width");

            EmitProperty_Px(Script, "height", Settings.Height);
            EmitProperty_Bind(Script, Settings.Height_Bind, "height");

            EmitProperty(Script, "disabled", Settings.Disabled);
            EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");

            EmitProperty_M(Script, "val", Settings.Text);
            EmitProperty_Bind2Way_M(Script, Settings.Text_Bind, "val", "change");
            EmitEvent_Bind(Script, Settings.Text_OnChange_Bind, "change");

            //EmitProperty(Script, "label?", Settings.Disabled);
            //EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");

            var labelHtml = "";
            if (Settings.Label != null)
                labelHtml = "<div>"+ Settings.Label + "</div>";

            if (Settings.InTable == TagInTable.None)
                Html.Append(labelHtml+"<input type='text'  id='" + UniqueId + "'/>");
            else
            if (Settings.InTable == TagInTable.AsDetail)
                Html.Append("<td class='x-form-td'>" + labelHtml+ "</td><td class='x-form-td'><input type='text'  id='" + UniqueId + "'/></td>");
            else
            if (Settings.InTable == TagInTable.AsRow)
                Html.Append("<tr class='x-form-tr'><td class='x-form-td'>" + labelHtml+ "</td><td class='x-form-td'><input type ='text' id='" + UniqueId + "'/></td></tr>");
            else
                throw new Exception(nameof(xInput)+": неизвестный "+ nameof(Settings) +"."+ nameof(Settings.InTable));
            return base.GetHtml();

        }

        //        public string GetHtml_old()
        //        {
        //            var script = @"
        //<script>
        //    $(document).ready(function() {
        //        var countries = new Array('Россия', ""Армения"", 'Algeria', 'Andorra', 'Angola');
        //        var tag=$('#{{id}}');
        //        tag.jqxInput({ placeHolder: 'Enter a Country', height: {{settings.Height}}, width: {{settings.Width}}, minLength: 1,  source: countries, value: {{{value}}} });

        //        tag.on('change', function () { 
        //            //var value = $('#{{id}}').val(); alert('Ok');
        //            bindingHub.server.sendBindedValueChanged('{{settings.Model.BindingId}}', '{{settings.BindValueTo}}',tag.val());

        //            });

        //            bindingHub.client.receiveBindedValueChanged = function (modelBindingID, propertyName, newValue) {
        //               if (modelBindingID=='{{settings.Model.BindingId}}' && propertyName=='{{settings.BindValueTo}}'){
        //                 tag.val(newValue);
        //                //alert('Ok-'+name+' '+message);
        //               };
        //            }; 

        //        $.connection.hub.start().done(function () {
        //            bindingHub.server.subscribeBindedValueChanged('{{settings.Model.BindingId}}', '{{settings.BindValueTo}}');

        //        });

        //    });
        //</script>
        //   ";
        //            settings.Model.OnChangeByHuman += (sender, propertyName, newValue) =>
        //            {
        //                if (propertyName == settings.BindValueTo)
        //                    settings.FireOnChange(this, newValue);
        //            };

        //            var value= HttpUtility.JavaScriptStringEncode(settings.Model.GetPropertyValue(settings.BindValueTo).ToString(), true);

        //            script = Render.StringToString(script, new { id = UniqueId, settings = settings, value= value});

        //            var tag = @"<input type = 'text' id = '{{id}}' />";
        //            tag = Render.StringToString(tag, new { id = UniqueId });

        //            return tag + script;
        //        }



    }
}
