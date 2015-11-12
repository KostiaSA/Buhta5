
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTableDetail : INotifyPropertyChanged
    {
        public Guid ID;

        private string name;
        [DisplayName(" Имя табличной части"), Description(""), Category(" Табличная часть")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private SchemaTable table;
        [Browsable(false)]
        public SchemaTable Table
        {
            get { return table; }
            set { table = value; firePropertyChanged("Table"); }
        }

        [DisplayName("Бизнес-операции"), Description(""), Category("Бизнес-операции")]
        public ObservableCollection<SchemaTableOper> Opers { get; private set; }

        public SchemaTableDetail()
        {
            Opers = new ObservableCollection<SchemaTableOper>();
            Opers.CollectionChanged += Opers_CollectionChanged;
        }

        public virtual void Validate(StringBuilder error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error.AppendLine("У детали не заполнено поле 'Имя'.");
                Name = "";
            }

            if (DetailTableID == null)
                error.AppendLine("У детали '" + Name + "' не заполнена соответствующая таблица.");

            if (DetailQueryID == null)
                error.AppendLine("У детали '" + Name + "' не заполнен запрос для отображения.");

            if (Opers.Count > 0 && DetailTableID != null)
            {
                if (GetDetailTable(true).GetColumnByRole(RoleConst.ВложеннаяТаблица_БизнесОперация)==null)
                    error.AppendLine("У деталь-таблицы '" + GetDetailTable().Name + "' должна быть колонка с ролью 'ВложеннаяТаблица.БизнесОперация'.");

            }

        }

        void Opers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var oper in Opers)
                if (oper.TableDetail == null)
                    oper.TableDetail = this;

            firePropertyChanged("Opers");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (table != null)
                table.firePropertyChanged("Details");

        }


        private Guid? detailTableID;
        ////[Editor(typeof(ForeingTableSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(ForeingTableSelectorTypeConverter))]
        [DisplayName("Деталь-таблица"), Description(""), Category(" Табличная часть")]
        public Guid? DetailTableID
        {
            get { return detailTableID; }
            set { detailTableID = value; firePropertyChanged("DetailTableID"); detailTable_cached = null; }
        }

        private Guid? detailQueryID;
        ////[Editor(typeof(SchemaFormDataGridQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(SchemaFormDataGridQuerySelectorTypeConverter))]
        [DisplayName("Запрос для просмотра"), Description(""), Category(" Табличная часть")]
        public Guid? DetailQueryID
        {
            get { return detailQueryID; }
            set { detailQueryID = value; firePropertyChanged("DetailQueryID"); }
        }

        SchemaTable detailTable_cached;
        public SchemaTable GetDetailTable(bool noCached=false)
        {
            if (noCached)
                detailTable_cached = null;

            if (DetailTableID == null)
                return null;

            if (detailTable_cached != null)
                return detailTable_cached;

            detailTable_cached = App.Schema.GetObject<SchemaTable>((Guid)DetailTableID);

            return detailTable_cached;
        }


        ////public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////        info.CellData = info.Node.ToString();
        ////    else
        ////        info.CellData = "";
        ////}

        ////public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    info.Children = (info.Node as SchemaTableDetail).Opers;

        ////}

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}

        public override string ToString()
        {
            return Name;
        }
    }
}
