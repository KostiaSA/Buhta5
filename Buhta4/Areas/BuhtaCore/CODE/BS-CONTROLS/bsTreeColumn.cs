using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class bsTreeColumnSettings : bsControlSettings
    {
        public bsTreeColumnSettings(BaseModel model) : base(model) { }

        public GridColumnDataType DataType = GridColumnDataType.String;

        public int? Width;
        public string Width_Bind;

        public bool? Hidden;
        public string Hidden_Bind;

        public string Caption;
        public string Caption_Bind;

        public string Field_Bind;

        public string CellTemplate;
        public string CellTemplateJS;

        //public void EmitDataField(StringBuilder script,int colIndex)
        //{
        //    script.Append("fields.push({");
        //    script.Append("name:"+Field_Bind.AsJavaScript()+",");
        //    script.Append("type:" +  Enum.GetName(typeof(GridColumnDataType),DataType).ToLower().AsJavaScript() + ",");
        //    script.Append("map:'"+colIndex+"'");
        //    script.AppendLine("});");
        //}

        public void EmitColgroupCol(StringBuilder html, StringBuilder script)
        {
            html.Append("<col id='" + UniqueId + "' " + GetAttrs() + ">");
            html.Append("</col>");
        }
    }

    public class bsTreeColumn
    {

        public bsTreeColumnSettings Settings;

        public bsTree Tree { get; private set; }

        public bsTreeColumn()
        {
            //Settings = new bsTreeColumnSettings();
        }

        public string GetHtml()
        {

            //EmitProperty_Px(Script, "width", Settings.Width);
            //EmitProperty_Bind(Script, Settings.Width_Bind, "width");

            return "";
        }


    }
}
