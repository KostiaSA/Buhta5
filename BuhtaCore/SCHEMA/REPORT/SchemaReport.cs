using BLToolkit.Aspects;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    [Export(typeof(SchemaObject))]
    public class SchemaReport : SchemaObject
    {

        public ObservableCollection<SchemaReportBand> Bands { get; set; }

        private byte[] reportDefinition;
        public byte[] ReportDefinition
        {
            get { return reportDefinition; }
            set { reportDefinition = value; firePropertyChanged("ReportDefinition"); }
        }

        public SchemaReport()
        {

            Bands = new ObservableCollection<SchemaReportBand>();
            Bands.CollectionChanged += Bands_CollectionChanged;

        }

        void Bands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Bands");
        }


        public override string GetTypeDisplay
        {
            get
            {
                return "Печатная форма";
            }
        }

        public override void PrepareNew()
        {
            base.PrepareNew();
            Name = "Введите имя новой печатной формы";
        }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new SchemaReportDesigner_page() { EditedRecord = this };
        }


        //public SchemaReportBand GetBandByName(string name)
        //{
        //    foreach (var col in GetAllColumns())
        //        if (col.Name.Equals(name))
        //            return col;
        //    return null;
        //}

        public string GetDisplayName()
        {
            return Name;
        }


        //public List<SchemaReportBand> GetColumns()
        //{
        //    return GetAllColumns().ToList<IViewColumn>();
        //}

        public string GetDisplayNameAndDataType()
        {
            throw new NotImplementedException();
        }


        //public DataTable Open()
        //{
        //    using (var db = App.Schema.SqlDB.GetDbManager())
        //    {
        //        return db.SetCommand(GetSqlText(null)).ExecuteDataTable();
        //    }
        //}


        public List<SchemaReportBand> GetAllBands()
        {
            var retList = new List<SchemaReportBand>();
            foreach (var band in Bands)
            {
                retList.Add(band);
                band.GetAllBands(retList);
            }
            return retList;
        }


        public string GetSql(int? topRows = null)
        {
            var sql = new StringBuilder();

            sql.AppendLine("-- печатная форма " + Name.AsSQL());

            foreach (var band in Bands)
                band.EmitSql(sql, topRows);

#if DEBUG
            File.WriteAllText(@"c:\$\report-" + ID.ToString() + "-" + Name.ToString().TranslateToCorrectFileName() + ".sql", sql.ToString());
#endif

            return sql.ToString();
        }

        public void PostProcessDataset(DataSet dataset)
        {
            dataset.DataSetName = "Данные";
            dataset.Namespace = "Жопа";
            int tableCounter=0;
            foreach (var band in Bands)
            {
                band.PostProcessDataset(dataset, ref tableCounter);
            }
        }


    }


}
