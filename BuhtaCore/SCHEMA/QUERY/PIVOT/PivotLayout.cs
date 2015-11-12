using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class PivotLayout :  INotifyPropertyChanged, ISupportInitialize
    {

        [Browsable(false)]
        public Guid ID { get; set; }

        private string name;
        [DisplayName("Имя")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private SchemaQuery parentQuery;
        public SchemaQuery ParentQuery
        {
            get { return parentQuery; }
            set { parentQuery = value; firePropertyChanged("ParentQuery"); }
        }

        private string layoutXML;
        public string LayoutXML
        {
            get { return layoutXML; }
            set { layoutXML = value; firePropertyChanged("LayoutXML"); }
        }


        public PivotLayout()
        {
            //            Columns = new ObservableCollection<SchemaQueryBaseColumn>();
            //          Columns.CollectionChanged += Columns_CollectionChanged;
        }


        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentQuery != null)
                parentQuery.firePropertyChanged("RootColumn");

        }

    }


}
