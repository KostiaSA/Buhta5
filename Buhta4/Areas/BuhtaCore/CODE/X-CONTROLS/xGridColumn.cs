using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public enum GridColumnDataType { String=0, Date=1, Number=2, Int=3, Float=4, Bool=5}
    public class xGridColumnSettings : xControlSettings
    {
        public GridColumnDataType DataType=GridColumnDataType.String;

        public int? Width;
        public string Width_Bind;

        public bool? Hidden;
        public string Hidden_Bind;

        public string Caption;
        public string Caption_Bind;

        public string Field_Bind;

        public void EmitDataField(StringBuilder script,int colIndex)
        {
            script.Append("fields.push({");
            script.Append("name:"+Field_Bind.AsJavaScript()+",");
            script.Append("type:" +  Enum.GetName(typeof(GridColumnDataType),DataType).ToLower().AsJavaScript() + ",");
            script.Append("map:'"+colIndex+"'");
            script.AppendLine("});");
        }
    }

    public class xGridColumn 
    {

        public xGridColumnSettings Settings;

        public xGrid Grid { get; private set; }

        public xGridColumn()
        {
            Settings = new xGridColumnSettings();
        }

        public string GetHtml()
        {

            //EmitProperty_Px(Script, "width", Settings.Width);
            //EmitProperty_Bind(Script, Settings.Width_Bind, "width");

            return "";
        }


    }
}
