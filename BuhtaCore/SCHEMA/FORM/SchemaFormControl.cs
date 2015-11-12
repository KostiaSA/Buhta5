using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    [JsonObject(IsReference = true)]
    public class SchemaFormControl : TreeList.IVirtualTreeListData, INotifyPropertyChanged
    {

        private SchemaFormControl parentControl;
        [Browsable(false)]
        public SchemaFormControl ParentControl
        {
            get { return parentControl; }
            set { parentControl = value; firePropertyChanged("ParentControl"); }
        }

        private SchemaForm parentForm;
        [Browsable(false)]
        public SchemaForm ParentForm
        {
            get { return parentForm; }
            set { parentForm = value; firePropertyChanged("ParentForm"); }
        }

        private string name;
        [DisplayName("Name"), Description("Имя для cs-script "), Category("  Колонка")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private string description;
        [DisplayName("Description"), Description("Описание"), Category("  Колонка")]
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("Description"); }
        }

        [Browsable(false)]
        public ObservableCollection<SchemaFormControl> Controls { get; private set; }

        public SchemaFormControl()
        {
            Controls = new ObservableCollection<SchemaFormControl>();
            Controls.CollectionChanged += Columns_CollectionChanged;
        }

        public virtual bool IsContainer()
        {
            return true;
        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Columns");
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentControl != null)
                parentControl.firePropertyChanged("Columns");
            if (parentForm != null)
                parentForm.firePropertyChanged("RootControl");
        }

        public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            if (info.Column.FieldName == "Name")
                info.CellData = (info.Node as SchemaFormControl).GetDisplayHtml();
            else
                info.CellData = null;
        }

        public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = Controls;
        }

        public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            throw new NotImplementedException();
        }

        public virtual string GetDisplayName()
        {
            return GetType().Name.Substring(10);
        }

        public virtual string GetDisplayHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<span style='font-size:9pt;font-family:Consolas;color:#A31515'>");
            sb.Append(WebUtility.HtmlEncode("<" + GetDisplayName() + ">"));
            AddDisplayHtmlAttrs(sb);
            sb.Append("</span>");
            return sb.ToString();
        }

        public virtual void AddDisplayHtmlAttrs(StringBuilder sb)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                sb.Append(GetDisplayHtmlAttr("Name", Name));
            }
        }

        public string GetDisplayHtmlAttr(string attrName, string attrValue)
        {
            return "&nbsp;<span style='color:red'>" + WebUtility.HtmlEncode(attrName) + @"</span><span style='color:blue'>=""" + WebUtility.HtmlEncode(attrValue) + @"""</span>";
        }

        public virtual void Render(Control parentControl)
        {
            throw new Exception("Abstract method");
        }

        public virtual void InitializeAfterCreateInDesigner()
        {
            // throw new Exception("Abstract method");
        }
    }

}
