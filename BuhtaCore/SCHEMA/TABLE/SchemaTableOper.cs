using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTableOper : INotifyPropertyChanged, TreeList.IVirtualTreeListData
    {
        [Browsable(false)]
        public Guid ID { get; set; }

        private string num;
        [DisplayName(" Номер"), Description("Произвольный номер бизнес-операции"), Category(" ")]
        public string Num
        {
            get { return num; }
            set { num = value; firePropertyChanged("Num"); }
        }

        private string name;
        [DisplayName("Название"), Description("Произвольное название бизнес-операции"), Category(" ")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private SchemaTableDetail tableDetail;
        [Browsable(false)]
        public SchemaTableDetail TableDetail
        {
            get { return tableDetail; }
            set { tableDetail = value;/* firePropertyChanged("TableDetail");*/ }
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (ID == Guid.Empty)
                ID = Guid.NewGuid();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (tableDetail != null)
                tableDetail.firePropertyChanged("Opers");

        }

        public ObservableCollection<SchemaTableProvodka> Provodkas { get; private set; }

        public SchemaTableOper()
        {

            Provodkas = new ObservableCollection<SchemaTableProvodka>();
            Provodkas.CollectionChanged += Provodkas_CollectionChanged;
        }

        void Provodkas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var prov in Provodkas)
                if (prov.OwnerTableOper == null)
                    prov.OwnerTableOper = this;
            firePropertyChanged("Provodkas");
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Num != null)
                sb.Append("[" + Num + "]  ");
            if (Name != null)
                sb.Append(Name);
            return sb.ToString();
        }


        public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            if (info.Column.FieldName == "Name")
                info.CellData = info.Node.ToString();
            else
                info.CellData = "";
        }

        public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = (info.Node as SchemaTableOper).Provodkas;
        }

        public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
