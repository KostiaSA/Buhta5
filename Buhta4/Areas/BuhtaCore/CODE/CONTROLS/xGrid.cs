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
        public static MvcHtmlString xGrid(this HtmlHelper helper, xGridSettings settings)
        {
            return new MvcHtmlString(new xGrid(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xGrid(this HtmlHelper helper, Action<xGridSettings> settings)
        {

            return new MvcHtmlString(new xGrid(helper.ViewData.Model, settings).GetHtml());
        }

    }

    public class xGridSettings : xControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public int? Width;
        public string Width_Bind;

        public int? Height;
        public string Height_Bind;

        public string DataSource_Bind;

        List<xGridColumnSettings> columns = new List<xGridColumnSettings>();
        public List<xGridColumnSettings> Columns { get { return columns; } }

        public void AddColumn(Action<xGridColumnSettings> settings)
        {
            var col = new xGridColumnSettings();
            settings(col);
            columns.Add(col);
        }

        public void EmitDataSource_Bind(StringBuilder script, BaseModel model)
        {
            if (DataSource_Bind != null)
            {
                if (!model.BindedProps.ContainsKey(DataSource_Bind))
                {
                    model.BindedProps.Add(DataSource_Bind, model.GetPropertyValue(DataSource_Bind));
                }
                //script.AppendLine("tag." + jqxMethodName + "(" + Model.BindedProps[DataSource_Bind] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged('" + model.BindingId + "', " + DataSource_Bind.AsJavaScript() + ",function(newDataArray){");
                //script.AppendLine("    tag.jqxGrid(newValue);");
                script.AppendLine("  source.localdata=newDataArray;");
                script.AppendLine("  tag.jqxGrid('updatebounddata');");
                script.AppendLine("});");


                var fieldNames = "";
                foreach (var col in Columns)
                    fieldNames += col.Field_Bind + ",";
                fieldNames = fieldNames.WithOutLastChar();

                script.AppendLine("$.connection.hub.start().done(function() {");
                script.AppendLine("  $.connection.bindingHub.server.sendGridDataSourceRequest('" + model.BindingId + "', " + DataSource_Bind.AsJavaScript() + ","+ fieldNames.AsJavaScript() + ");");
                script.AppendLine("});");


            }

        }


    }

    public class xGrid : xControl<xGridSettings>
    {
        public override string GetJqxName()
        {
            return "jqxGrid";
        }

        public xGrid(object model, xGridSettings settings) : base(model, settings) { }
        public xGrid(object model, Action<xGridSettings> settings) : base(model, settings) { }



        //public xGrid(xGridSettings settings)
        //{
        //    Settings = settings;
        //}

        //public xGrid(Action<xGridSettings> settings)
        //{
        //    Settings = new xGridSettings();
        //    settings(Settings);
        //}

        public override string GetHtml()
        {
            //            Script.AppendLine(@"
            //            var source =
            //                        {
            //                localdata: [
            //                    [""Alfreds Futterkiste"", ""Maria Anders"", ""Sales Representative"", ""Obere Str. 57"", ""Berlin"", ""Germany""]
            //                ],
            //                datafields: [
            //                    { name: 'CompanyName', type: 'string', map: '0'},
            //                    { name: 'ContactName', type: 'string', map: '1' },
            //                    { name: 'Title', type: 'string', map: '2' },
            //                    { name: 'Address', type: 'string', map: '3' },
            //                    { name: 'City', type: 'string', map: '4' },
            //                    { name: 'Country', type: 'string', map: '5' }
            //                ],
            //                datatype: 'array'
            //            };
            //            var dataAdapter = new $.jqx.dataAdapter(source);

            //            $('#" + UniqueId + @"').jqxGrid(
            //            {
            //                width: 850,
            //                columns: [
            //                  { text: 'Company Name', datafield: 'CompanyName', width: 200 },
            //                  { text: 'Contact Name', datafield: 'ContactName', width: 150 },
            //                  { text: 'Contact Title', datafield: 'Title', width: 100 },
            //                  { text: 'Address', datafield: 'Address', width: 100 },
            //                  { text: 'City', datafield: 'City', width: 100 },
            //                  { text: 'Country', datafield: 'Country' }
            //                ]
            //            });
            //");
            EmitBeginScript(Script);

            EmitProperty_Px(Script, "width", Settings.Width);
            EmitProperty_Bind(Script, Settings.Width_Bind, "width");

            EmitProperty_Px(Script, "height", Settings.Height);
            EmitProperty_Bind(Script, Settings.Height_Bind, "height");

            EmitProperty(Script, "disabled", Settings.Disabled);
            EmitProperty_Bind(Script, Settings.Disabled_Bind, "disabled");



            Script.AppendLine("var columns=[];");
            Script.AppendLine("var col;");
            foreach (var col in Settings.Columns)
            {
                Script.AppendLine("col={};");
                if (col.Caption != null)
                    Script.AppendLine("col.text=" + col.Caption.AsJavaScript() + ";");
                if (col.Field_Bind != null)
                    Script.AppendLine("col.datafield=" + col.Field_Bind.AsJavaScript() + ";");
                Script.AppendLine("columns.push(col);");
            }

            Script.AppendLine("var fields=[];");
            var index = 0;
            foreach (var col in Settings.Columns)
            {
                col.EmitDataField(Script, index++);
            }

            Script.AppendLine("var source={localdata:[], datatype:'array', datafields:fields};");

            Script.AppendLine("var dataAdapter=new $.jqx.dataAdapter(source);");
            Script.AppendLine("tag." + GetJqxName() + "({columns:columns, source:dataAdapter});");



            Settings.EmitDataSource_Bind(Script, Model);

            //            Script.AppendLine("tag." + GetJqxName() + "('refreshdata');");
            //            Script.AppendLine("tag." + GetJqxName() + "('refresh');");
            //Script.AppendLine(@"source.localdata=[[""жопа0"",""жопа1"",""жопа2""],[""жопа0"",""жопа1"",""жопа2""]];");
            //Script.AppendLine("tag." + GetJqxName() + "('updatebounddata');");


            Html.Append("<div id='" + UniqueId + "'/>");

            return base.GetHtml();
        }



    }
}
