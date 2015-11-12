using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaTableProvodka : INotifyPropertyChanged
    {
        [Browsable(false)]
        public Guid ID { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private SchemaTableOper ownerTableOper;
        [Browsable(false)]
        public SchemaTableOper OwnerTableOper
        {
            get { return ownerTableOper; }
            set { ownerTableOper = value; firePropertyChanged("OwnerTableOper"); }
        }

        private SchemaTableProvodka ownerProvodka;
        [Browsable(false)]
        public SchemaTableProvodka OwnerProvodka
        {
            get { return ownerProvodka; }
            set { ownerProvodka = value; firePropertyChanged("OwnerProvodka"); }
        }


        private Guid? dbRegistrID;
        ////[Editor(typeof(SchemaTableSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(ForeingTableSelectorTypeConverter))]
        public Guid? DbRegistrID
        {
            get { return dbRegistrID; }
            set { dbRegistrID = value; firePropertyChanged("DbRegistrID"); }
        }

        private Guid? krRegistrID;
        ////[Editor(typeof(SchemaTableSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(ForeingTableSelectorTypeConverter))]
        public Guid? KrRegistrID
        {
            get { return krRegistrID; }
            set { krRegistrID = value; firePropertyChanged("KrRegistrID"); }
        }

        public ObservableCollection<SchemaTableProvodkaField> DbRegistrFields { get; private set; }
        public ObservableCollection<SchemaTableProvodkaField> KrRegistrFields { get; private set; }

        public ObservableCollection<SchemaTableProvodka> Provodkas { get; private set; }

        public SchemaTableProvodka()
        {

            Provodkas = new ObservableCollection<SchemaTableProvodka>();
            Provodkas.CollectionChanged += Provodkas_CollectionChanged;

            DbRegistrFields = new ObservableCollection<SchemaTableProvodkaField>();
            DbRegistrFields.CollectionChanged += DbRegistrFields_CollectionChanged;

            KrRegistrFields = new ObservableCollection<SchemaTableProvodkaField>();
            KrRegistrFields.CollectionChanged += KrRegistrFields_CollectionChanged;
        }

        void DbRegistrFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var field in DbRegistrFields)
                if (field.Provodka == null)
                    field.Provodka = this;
            firePropertyChanged("Provodkas");
        }

        void Provodkas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var prov in Provodkas)
                if (prov.OwnerProvodka == null)
                    prov.OwnerProvodka = this;
            firePropertyChanged("Provodkas");
        }

        void KrRegistrFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var field in KrRegistrFields)
                if (field.Provodka == null)
                    field.Provodka = this;
            firePropertyChanged("Provodkas");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (ID == Guid.Empty)
                ID = Guid.NewGuid();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (ownerTableOper != null)
                ownerTableOper.firePropertyChanged("Provodkas");
            if (ownerProvodka != null)
                ownerProvodka.firePropertyChanged("Provodkas");

        }

        private Guid? detailQueryID;
        ////[Editor(typeof(SchemaFormDataGridQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(SchemaFormDataGridQuerySelectorTypeConverter))]
        public Guid? DetailQueryID
        {
            get { return detailQueryID; }
            set { detailQueryID = value; firePropertyChanged("DetailQueryID"); }
        }

        private string begDate;
        public string BegDate
        {
            get { return begDate; }
            set { begDate = value; firePropertyChanged("BegDate"); }
        }

        private string endDate;
        public string EndDate
        {
            get { return endDate; }
            set { endDate = value; firePropertyChanged("EndDate"); }
        }

        private string dateSource;
        public string DateSource
        {
            get { return dateSource; }
            set { dateSource = value; firePropertyChanged("DateSource"); }
        }

        public override string ToString()
        {
            return Name;
        }

        ////public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////        info.CellData = info.Node.ToString();
        ////    else
        ////        info.CellData = "<пусто>";
        ////}

        ////public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    info.Children = Provodkas;
        ////}

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}
    }
}
