using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaReportBand : ISupportInitialize, TreeList.IVirtualTreeListData, INotifyPropertyChanged
    {

        private string name;
        [DisplayName("Имя")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private Guid? queryID;
        [Editor(typeof(SchemaReportBandQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaReportBandQuerySelectorTypeConverter))]
        [DisplayName("Запрос")]
        public Guid? QueryID
        {
            get { return queryID; }
            set { queryID = value; firePropertyChanged("QueryID"); }
        }

        private SchemaReportBand parentBand;
        [Browsable(false)]
        public SchemaReportBand ParentBand
        {
            get { return parentBand; }
            set { parentBand = value; firePropertyChanged("ParentBand"); }
        }

        private SchemaReport parentReport;
        [Browsable(false)]
        public SchemaReport ParentReport
        {
            get { return parentReport; }
            set { parentReport = value; firePropertyChanged("ParentReport"); }
        }

        [Browsable(false)]
        public ObservableCollection<SchemaReportBand> Bands { get; set; }

        public virtual void GetAllBands(List<SchemaReportBand> list)
        {
            foreach (var band in Bands)
            {
                list.Add(band);
                band.GetAllBands(list);
            }
        }

        public virtual List<SchemaReportBand> GetAllBands()
        {
            var list = new List<SchemaReportBand>();
            GetAllBands(list);
            return list;
        }


        public SchemaReportBand()
        {
            Bands = new ObservableCollection<SchemaReportBand>();
            Bands.CollectionChanged += Bands_CollectionChanged;
        }

        void Bands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Bands");
        }


        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }

        public virtual void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            if (info.Column.FieldName == "Name")
                info.CellData = (info.Node as SchemaReportBand).Name;
            else
                if (info.Column.FieldName == "Query")
                    info.CellData = App.Schema.GetSampleObject<SchemaQuery>((info.Node as SchemaReportBand).QueryID).DisplayName;
                else
                    info.CellData = null;
        }

        public virtual void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = Bands;
        }

        public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentBand != null)
                parentBand.firePropertyChanged("Bands");
            if (parentReport != null)
                parentReport.firePropertyChanged("Bands");

        }


        public string GetDisplayName()
        {
            return Name;
        }

        public string GetDisplayNameAndDataType()
        {
            return "GetDisplayNameAndDataType()";
        }


        public SchemaReport GetReport()
        {
            if (ParentReport != null)
                return ParentReport;
            else
                return ParentBand.GetReport();
        }

        public void EmitSql(StringBuilder sql, int? topRows = null)
        {
            if (QueryID == null)
                throw new Exception("Печатная форма " + GetReport().Name.AsSQL() + ", деталь " + Name.AsSQL() + ": не заполнен QueryID");

            var query = App.Schema.GetObject<SchemaQuery>((Guid)QueryID);
            if (query == null)
                throw new Exception("Печатная форма " + GetReport().Name.AsSQL() + ", деталь " + Name.AsSQL() + ": не найден запрос по ID=" + QueryID.AsSQL());

            query.RuntimeTop = topRows;
            query.RuntimeIntoTableName = "#" + Name;

            sql.AppendLine(query.GetSqlText(null));
            sql.AppendLine("SELECT * FROM [#" + Name + "] AS [" + Name + "]");
            query.EmitOrderBySql(sql);
            sql.AppendLine(";");

            foreach (var band in Bands)
                band.EmitSql(sql, topRows);

        }

        public void PostProcessDataset(DataSet dataset, ref int tableCounter)
        {
            dataset.Tables[tableCounter].TableName = Name;
            tableCounter++;
            foreach (var band in Bands)
            {
                band.PostProcessDataset(dataset, ref tableCounter);
            }
        }

    }

    public class SchemaReportBandQuerySelectorTypeConverter : TypeConverter
    {
        private TypeConverter mTypeConverter;

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "";

            if (mTypeConverter == null)
                mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

            if (context != null && destinationType == typeof(string))
            {
                var query = App.Schema.GetObject<SchemaQuery>((Guid)value);
                return query == null ? "<null>" : query.Name;
            }
            return mTypeConverter.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class SchemaReportBandQuerySelectorEditor : ObjectSelectorEditor
    {
        protected override void FillTreeWithData(System.ComponentModel.Design.ObjectSelectorEditor.Selector theSel,
          ITypeDescriptorContext theCtx, IServiceProvider theProvider)
        {
            base.FillTreeWithData(theSel, theCtx, theProvider);  //clear the selection

            //    jsqlTableColumn aCtl = (jsqlTableColumn)theCtx.Instance;

            //foreach (Type tableType in mixUtil.GetAllSubclassTypes(typeof(mixTable)))
            //{
            //SelectorNode aNd = new SelectorNode(tableType.FullName, tableType);
            //theSel.Nodes.Add(aNd);
            //}

            foreach (SchemaObject_cache query in App.Schema.Objects_cache.Values)
            {
                if (query.RootClass == typeof(SchemaQuery).Name)
                {
                    SelectorNode aNd = new SelectorNode(query.Name, query.ID);
                    theSel.Nodes.Add(aNd);
                }
            }

        }

    }

}
