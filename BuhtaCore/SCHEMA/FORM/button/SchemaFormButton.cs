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
    public class SchemaFormButton : SchemaFormControl
    {
        private string text;
        public string Text
        {
            get { return text; }
            set 
            { 
                text = value; 
                firePropertyChanged("Text");
                ////if (NativeButton != null)
                ////    NativeButton.Text = text;
            }
        }

        private SchemaAction action;
        [DisplayName("Action"), Description("Тип действия при нажатии кнопки"), Category(" Action")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SchemaAction Action
        {
            get { return action; }
            set { action = value; firePropertyChanged("Action"); }
        }

        ////[JsonIgnore]
        ////public xButton NativeButton;

        public override void AddDisplayHtmlAttrs(StringBuilder sb)
        {
            base.AddDisplayHtmlAttrs(sb);
            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(GetDisplayHtmlAttr("Text",Text));
            }
            if (Action!=null)
            {
                sb.Append(GetDisplayHtmlAttr("Action", Action.GetFullName()));
            }
        }

        //public override void Render(Control parentControl)
        //{
        //    NativeButton = new xButton();
        //    NativeButton.Parent = parentControl;
        //    NativeButton.Text = text;
        //    NativeButton.Click += NativeButton_Click;

        //    parentControl.Controls.Add(NativeButton);
        //}

        void NativeButton_Click(object sender, EventArgs e)
        {
            if (Action != null)
                Action.Execute();
        }

        public override bool IsContainer()
        {
            return false;
        }

    }
}
