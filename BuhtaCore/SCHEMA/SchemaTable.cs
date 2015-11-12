using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Buhta
{
    //public enum SchemaTableRole_old { None = 0, Справочник = 1, Документ = 2, Деталь = 3, Регистр = 4 }
    //[Export(typeof(SchemaObject))]
    public partial class SchemaTable : SchemaObject
    {

        public ObservableCollection<SchemaTableColumn> Columns { get; private set; }

        [JsonIgnore]
        public bool IsLogTable;

        [JsonIgnore]
        public bool IsProvodkasTable;

        private bool isNotSyncronize;
        public bool IsNotSyncronize
        {
            get { return isNotSyncronize; }
            set { isNotSyncronize = value; firePropertyChanged("IsNotSyncronize"); }
        }

        private Guid? externalDatabase;
        public Guid? ExternalDatabase
        {
            get { return externalDatabase; }
            set { externalDatabase = value; firePropertyChanged("ExternalDatabase"); }
        }

        private Guid? defaultQueryID;
        public Guid? DefaultQueryID
        {
            get { return defaultQueryID; }
            set { defaultQueryID = value; firePropertyChanged("DefaultQueryID"); }
        }

        private Guid? lookupQueryID;
        public Guid? LookupQueryID
        {
            get { return lookupQueryID; }
            set { lookupQueryID = value; firePropertyChanged("LookupQueryID"); }
        }

        private bool isSubconto;
        public bool IsSubconto
        {
            get { return isSubconto; }
            set { isSubconto = value; firePropertyChanged("IsSubconto"); }
        }

        private string subcontoName;
        public string SubcontoName
        {
            get { return subcontoName; }
            set { subcontoName = value; firePropertyChanged("SubcontoName"); }
        }

        private string formLayout;
        public string FormLayout
        {
            get { return formLayout; }
            set { formLayout = value; firePropertyChanged("FormLayout"); }
        }

        private bool isProvodkaOwner;
        public bool IsProvodkaOwner // может иметь движения по регистрам
        {
            get { return isProvodkaOwner; }
            set { isProvodkaOwner = value; firePropertyChanged("IsProvodkaOwner"); }
        }

        private bool isUserEditable;
        public bool IsUserEditable // может редактироваться с созданием лога
        {
            get { return isUserEditable; }
            set { isUserEditable = value; firePropertyChanged("IsUserEditable"); }
        }



        public SchemaTable()
        {


            Columns = new ObservableCollection<SchemaTableColumn>();
            Columns.CollectionChanged += Columns_CollectionChanged;


        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var column in Columns)
                if (column.Table == null)
                    column.Table = this;
            firePropertyChanged("Columns");
        }



        public override void Validate(StringBuilder error)
        {
            base.Validate(error);

            if (Name != null && Name.Length > 128)
                error.AppendLine("Имя таблицы длинее 128 символов.");


        }

        void TableRoles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("TableRoles");
        }


        public override void firePropertyChanged(string propertyName)
        {
            base.firePropertyChanged(propertyName);
        }



        public override string GetTypeDisplay
        {
            get
            {
                return "Таблица";
            }
        }

        public override void PrepareNew()
        {
            //            if (TableRoles.Count == 0)
            //              TableRoles.Add(RoleConst.Таблица);  // таблица

        }
    }

}


