using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    [Export(typeof(SchemaFormControl))]
    public class SchemaFormPage : SchemaFormControl
    {

        public override void InitializeAfterCreateInDesigner()
        {
            var top = new SchemaFormPageTopPanel();
            top.ParentControl = this;
            Controls.Add(top);

            var middle = new SchemaFormPageMiddlePanel();
            middle.ParentControl = this;
            Controls.Add(middle);

            var bottom = new SchemaFormPageBottomPanel();
            bottom.ParentControl = this;
            Controls.Add(bottom);
        }

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

        [JsonIgnore]
        public SchemaFormPageNativeControl NativeControl;

        public override void AddDisplayHtmlAttrs(StringBuilder sb)
        {
            base.AddDisplayHtmlAttrs(sb);
            //sb.Append(GetDisplayHtmlAttr("FlowDirection", FlowDirection.ToString()));
        }

        public override void Render(Control parentControl)
        {
            NativeControl = new SchemaFormPageNativeControl();
            NativeControl.Parent = parentControl;
            NativeControl.Dock = DockStyle.Fill;
            parentControl.Controls.Add(NativeControl);

            foreach (SchemaFormControl schemaControl in Controls)
            {
                if (schemaControl is SchemaFormPageTopPanel)
                {
                    var topPanel = schemaControl as SchemaFormPageTopPanel;
                    foreach (var ctl in topPanel.Controls)
                    {
                        ctl.Render(NativeControl.topButtonsPanel);
                    }
                    NativeControl.topButtonsPanel.PerformLayout();
                }
                else
                    if (schemaControl is SchemaFormPageMiddlePanel)
                    {
                        var middlePanel = schemaControl as SchemaFormPageMiddlePanel;
                        if (middlePanel.Controls.Count > 1)
                        {
                            MessageBox.Show("ОШИБКА: PageMiddlePanel содержит более одного элемента.");
                        }
                        if (middlePanel.Controls.Count > 0)
                        {
                            middlePanel.Controls[0].Render(NativeControl.middlePanel);
                            if (NativeControl.middlePanel.Controls.Count > 0)
                                NativeControl.middlePanel.Controls[0].Dock = DockStyle.Fill;

                        }
                    }
                    else
                        if (schemaControl is SchemaFormPageBottomPanel)
                        {

                        }
                        else
                            MessageBox.Show("ОШИБКА: Недопустимый элемент '" + schemaControl.GetType().Name + "' в SchemaFormPage.");

                //schemaControl.Render((Control)NativeFlowLayout);
            }

        }

        public override bool IsContainer()
        {
            return true;
        }

    }
}
