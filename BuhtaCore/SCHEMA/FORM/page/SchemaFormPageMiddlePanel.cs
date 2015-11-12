using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaFormControl))]
    public class SchemaFormPageMiddlePanel : SchemaFormControl
    {

        //private FlowDirection flowDirection;
        //public FlowDirection FlowDirection
        //{
        //    get { return flowDirection; }
        //    set 
        //    { 
        //        flowDirection = value; 
        //        firePropertyChanged("FlowDirection");
        //        if (NativeFlowLayout != null)
        //            NativeFlowLayout.FlowDirection = flowDirection;
        //    }
        //}

//        [JsonIgnore]
//        public FlowLayoutPanel NativeFlowLayout;

        public override void AddDisplayHtmlAttrs(StringBuilder sb)
        {
            base.AddDisplayHtmlAttrs(sb);
            //sb.Append(GetDisplayHtmlAttr("FlowDirection", FlowDirection.ToString()));
        }

        ////public override void Render(Control parentControl)
        ////{
        ////    //NativeFlowLayout = new FlowLayoutPanel();
        ////    //NativeFlowLayout.AutoSize = true;
        ////    //NativeFlowLayout.Parent = parentControl;
        ////    //NativeFlowLayout.FlowDirection = flowDirection;
        ////    //NativeFlowLayout.WrapContents = wrapContents;
        ////    //parentControl.Controls.Add(NativeFlowLayout);

        ////    //foreach (SchemaFormControl schemaControl in Controls)
        ////    //{
        ////    //    schemaControl.Render((Control)NativeFlowLayout);
        ////    //}

        ////}

        public override bool IsContainer()
        {
            return true;
        }

    }
}
