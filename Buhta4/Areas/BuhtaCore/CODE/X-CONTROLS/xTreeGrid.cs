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
        public static MvcHtmlString xTreeGrid(this HtmlHelper helper, xTreeGridSettings settings)
        {
            return new MvcHtmlString(new xTreeGrid(helper.ViewData.Model, settings).GetHtml());
        }

        public static MvcHtmlString xTreeGrid(this HtmlHelper helper, Action<xTreeGridSettings> settings)
        {

            return new MvcHtmlString(new xTreeGrid(helper.ViewData.Model, settings).GetHtml());
        }

    }


    public class xTreeGridSettings : xControlSettings
    {
        public bool? Disabled;
        public string Disabled_Bind;

        public int? Width;
        public string Width_Bind;

        public int? Height;
        public string Height_Bind;

        public bool? AutoHeight;
        public string AutoHeight_Bind;

        public bool? Pageable;
        public string Pageable_Bind;

        public int? PageSize;
        public string PageSize_Bind;

        public string KeyDataField;
        public string ParentDataField;

        public string DataSource_Bind;

        List<xTreeGridColumnSettings> columns = new List<xTreeGridColumnSettings>();
        public List<xTreeGridColumnSettings> Columns { get { return columns; } }

        public void AddColumn(Action<xTreeGridColumnSettings> settings)
        {
            var col = new xTreeGridColumnSettings();
            settings(col);
            columns.Add(col);
        }

        //public void EmitDataSource_Bind(StringBuilder script, BaseModel model)
        //{
        //    if (DataSource_Bind != null)
        //    {
        //        if (!model.BindedProps.ContainsKey(DataSource_Bind))
        //        {
        //            model.BindedProps.Add(DataSource_Bind, model.GetPropertyValue(DataSource_Bind));
        //        }
        //        //script.AppendLine("tag." + jqxMethodName + "(" + Model.BindedProps[DataSource_Bind] + ");");
        //        script.AppendLine("signalr.subscribeModelPropertyChanged(window.name,'" + model.BindingId + "', " + DataSource_Bind.AsJavaScript() + ",function(newDataArray){");
        //        //script.AppendLine("    tag.jqxTreeGrid(newValue);");
        //        script.AppendLine("  source.localdata=newDataArray;");
        //        script.AppendLine("  tag.jqxTreeGrid('updatebounddata');");
        //        script.AppendLine("});");


        //        var fieldNames = "";
        //        foreach (var col in Columns)
        //            fieldNames += col.Field_Bind + ",";
        //        fieldNames = fieldNames.WithOutLastChar();

        //        script.AppendLine("$.connection.hub.start().done(function() {");
        //        script.AppendLine("  $.connection.bindingHub.server.sendGridDataSourceRequest(window.name,'" + model.BindingId + "', " + DataSource_Bind.AsJavaScript() + "," + fieldNames.AsJavaScript() + ");");
        //        script.AppendLine("});");


        //    }

        //}


    }

    public class xTreeGrid : xControl<xTreeGridSettings>
    {
        public override string GetJqxName()
        {
            return "jqxTreeGrid";
        }

        public xTreeGrid(object model, xTreeGridSettings settings) : base(model, settings) { }
        public xTreeGrid(object model, Action<xTreeGridSettings> settings) : base(model, settings) { }



        //public xTreeGrid(xTreeGridSettings settings)
        //{
        //    Settings = settings;
        //}

        //public xTreeGrid(Action<xTreeGridSettings> settings)
        //{
        //    Settings = new xTreeGridSettings();
        //    settings(Settings);
        //}

        public void EmitDataSource_Bind(StringBuilder script, BaseModel model)
        {
            if (Settings.DataSource_Bind != null)
            {
                if (!model.BindedProps.ContainsKey(Settings.DataSource_Bind))
                {
                    model.BindedProps.Add(Settings.DataSource_Bind, model.GetPropertyValue(Settings.DataSource_Bind));
                }
                //script.AppendLine("tag." + jqxMethodName + "(" + Model.BindedProps[DataSource_Bind] + ");");
                script.AppendLine("signalr.subscribeModelPropertyChanged(window.name,'" + model.BindingId + "', " + Settings.DataSource_Bind.AsJavaScript() + ",function(newDataArray){");
                //script.AppendLine("    tag.jqxTreeGrid(newValue);");
                script.AppendLine("  source.localdata=newDataArray;");
                script.AppendLine("  tag." + GetJqxName() + "('updateBoundData');");
                script.AppendLine("});");


                var fieldNames = "";
                foreach (var col in Settings.Columns)
                    fieldNames += col.Field_Bind + ",";
                fieldNames = fieldNames.WithOutLastChar();

                script.AppendLine("$.connection.hub.start().done(function() {");
                script.AppendLine("  $.connection.bindingHub.server.sendGridDataSourceRequest(window.name,'" + model.BindingId + "', " + Settings.DataSource_Bind.AsJavaScript() + "," + fieldNames.AsJavaScript() + ");");
                script.AppendLine("});");


            }

        }


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

            //            $('#" + UniqueId + @"').jqxTreeGrid(
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

            EmitProperty(Script, "autoheight", Settings.AutoHeight);
            EmitProperty_Bind(Script, Settings.AutoHeight_Bind, "autoheight");

            EmitProperty(Script, "pageable", Settings.Pageable);
            EmitProperty_Bind(Script, Settings.Pageable_Bind, "pageable");

            EmitProperty(Script, "pagesize", Settings.PageSize);
            EmitProperty_Bind(Script, Settings.PageSize_Bind, "pagesize");

            Script.AppendLine("var columns=[];");
            Script.AppendLine("var col;");
            foreach (var col in Settings.Columns)
            {
                Script.AppendLine("col={};");
                if (col.Caption != null)
                    Script.AppendLine("col.text=" + col.Caption.AsJavaScript() + ";");
                if (col.Field_Bind != null)
                    Script.AppendLine("col.datafield=" + col.Field_Bind.AsJavaScript() + ";");

                if (col.Width != null)
                    Script.AppendLine("col.width=" + col.Width + ";");

                if (col.Hidden != null)
                    Script.AppendLine("col.hidden=" + col.Hidden.AsJavaScript() + ";");


                Script.AppendLine("columns.push(col);");
            }

            Script.AppendLine("var fields=[];");
            var index = 0;
            foreach (var col in Settings.Columns)
            {
                col.EmitDataField(Script, index++);
            }

            string keyDataFieldStr = "";
            if (Settings.KeyDataField != null)
            {
                keyDataFieldStr = ",id:'" + Settings.KeyDataField + "'";
            }

            string hierarchyStr = "";
            if (Settings.ParentDataField != null)
            {
                if (Settings.KeyDataField == null)
                    throw new Exception(nameof(xTreeGrid) + ": не заполнен " + nameof(Settings.KeyDataField));
                hierarchyStr = ",hierarchy:{keyDataField:{name:'" + Settings.KeyDataField + "'},parentDataField:{name:'" + Settings.ParentDataField + "'}},";
            }

            Script.AppendLine("var source={localdata:[], datatype:'array'" + keyDataFieldStr + ", datafields:fields" + hierarchyStr + "};");

            Script.AppendLine("var dataAdapter=new $.jqx.dataAdapter(source);");
            Script.AppendLine("tag." + GetJqxName() + "({columns:columns, source:dataAdapter});");



            EmitDataSource_Bind(Script, Model);

            //            Script.AppendLine("tag." + GetJqxName() + "('refreshdata');");
            //            Script.AppendLine("tag." + GetJqxName() + "('refresh');");
            //Script.AppendLine(@"source.localdata=[[""жопа0"",""жопа1"",""жопа2""],[""жопа0"",""жопа1"",""жопа2""]];");
            //Script.AppendLine("tag." + GetJqxName() + "('updatebounddata');");


            Html.Append("<div id='" + UniqueId + "' " + Settings.GetClassAttr() + Settings.GetStyleAttr() + "/>");

            return base.GetHtml();
        }



    }
}
