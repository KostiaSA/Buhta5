using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public enum bsGridColumnSort { none = 0, asc1 = 1, asc2 = 2, asc3 = 3, desc1 = -1, desc2 = -2, desc3 = -3 }

    public class bsGridColumn : bsControl
    {
        public bsGridColumn(BaseModel model) : base(model) { }

        public int? Width;
        public string Width_Bind;

        public bool? Hidden;

        public string Caption;
        public string Caption_Bind;

        public string Field_Bind;

        // пример "<small style='opacity:0.3'>{{ID}}</small>"
        public string CellTemplate;

        public string CellTemplateJS;

        public bsGridColumnSort Sort;

        public void EmitColgroupCol(StringBuilder html, StringBuilder script)
        {
            html.Append("<col id='" + UniqueId + "' " + GetAttrs() + ">");
            html.Append("</col>");
        }
    }

}
