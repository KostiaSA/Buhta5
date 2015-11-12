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

    public interface IViewColumn
    {
        string Name { get; }
        string GetDisplayName();
        string GetDisplayNameAndDataType();
        string GetFullAlias();
        IViewColumn GetParentViewColumn();
        IViewColumn GetSourceView();
        IViewColumn GetSourceViewColumn();
        IViewColumn GetJoinView();
        IViewColumn GetRootColumn();
        IViewColumn GetColumnByName(string name);
        List<IViewColumn> GetColumns();

        SchemaTable GetNativeTable();
        SchemaTableColumn GetNativeTableColumn();
        SchemaQuery GetNativeQuery();
        SchemaQueryBaseColumn GetNativeQueryColumn();
        Таблица_TableRole GetNativeTableRole();
        Колонка_ColumnRole GetNativeTableColumnRole();
        SchemaVirtualTable GetNativeVirtualTable();
        SchemaVirtualTableColumn GetNativeVirtualTableColumn();

        void EmitSelectSql(StringBuilder sql, string indent);
        void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent);

        SchemaTable GetRootNativeTable();

        string Get4PartsTableName();
    }

    [Export(typeof(SchemaObject))]
    public class SchemaQuery : SchemaObject, IViewColumn, IPermissionSupportObject
    {

        //Guid? rootQueryTableID;
        //public Guid? RootQueryTableID
        //{
        //    get { return rootQueryTableID; }
        //    set { rootQueryTableID = value; /*rootQueryTable_cached = null;*/ firePropertyChanged("RootQueryTableID"); }
        //}

        private SchemaQueryRootColumn rootColumn;
        public SchemaQueryRootColumn RootColumn
        {
            get { return rootColumn; }
            set { rootColumn = value; firePropertyChanged("RootColumn"); }
        }

        [JsonIgnore]
        public SchemaTableRow MasterTableRow;

        public ObservableCollection<PivotLayout> PivotLayouts { get; private set; }

        public PivotLayout GetPivotLayoutByID(Guid id)
        {
            foreach (var layout in PivotLayouts)
            {
                if (layout.ID == id)
                    return layout;
            }
            return null;
        }

        public SchemaQuery()
        {

            PivotLayouts = new ObservableCollection<PivotLayout>();
            PivotLayouts.CollectionChanged += PivotLayouts_CollectionChanged;

        }

        void PivotLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("PivotLayouts");
        }


        public override string GetTypeDisplay
        {
            get
            {
                return "Запрос";
            }
        }

        public override void PrepareNew()
        {
            base.PrepareNew();
            RootColumn = new SchemaQueryRootColumn();
            RootColumn.ParentQuery = this;
            Name = "Введите имя нового запроса";
        }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new SchemaQueryDesigner_page() { EditedRecord = this };
        }


        public IViewColumn GetParentViewColumn()
        {
            throw new NotImplementedException();
        }

        public IViewColumn GetSourceView()
        {
            throw new NotImplementedException();
        }

        public IViewColumn GetSourceViewColumn()
        {
            throw new NotImplementedException();
        }


        public IViewColumn GetJoinView()
        {
            throw new NotImplementedException();
        }


        public IViewColumn GetColumnByName(string name)
        {
            if (RootColumn == null)
                return null;
            foreach (var col in RootColumn.GetAllColumns())
                if (col.Name.Equals(name))
                    return col;
            return null;
        }

        // run-time параметры
        public string RuntimeLookupSearchFilterString;
        public Guid? RuntimeRecordIDFilter;
        public string RuntimeIntoTableName;
        public int? RuntimeTop;
        public string RuntimeOffsetRows;
        public string RuntimeFetchRows;



        public string GetDisplayName()
        {
            return Name;
        }


        public List<IViewColumn> GetColumns()
        {
            return RootColumn.GetAllColumns().ToList<IViewColumn>();
        }

        public SchemaTable GetNativeTable()
        {
            return null;
        }

        public SchemaTableColumn GetNativeTableColumn()
        {
            return null;
        }

        public SchemaQuery GetNativeQuery()
        {
            return this;
        }

        public SchemaQueryBaseColumn GetNativeQueryColumn()
        {
            return null;
        }

        public string Get4PartsTableName()
        {
            return "[" + Name + "]";
        }


        public string GetDisplayNameAndDataType()
        {
            throw new NotImplementedException();
        }


        public DataTable Open()
        {
            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                return db.SetCommand(GetSqlText(null)).ExecuteDataTable();
            }
        }

        public void EmitOrderBySql(StringBuilder sql)
        {
            var cols = GetAllColumns()
                     .Where(col => col is SchemaQuerySelectColumn && (col as SchemaQuerySelectColumn).OrderBy != QueryOrderBy.None)
                     .OrderBy(col => (col as SchemaQuerySelectColumn).OrderBy).ToList();
            if (cols.Count > 0)
            {
                sql.AppendLine("ORDER BY");
                foreach (var col in cols)
                {
                    if ((col as SchemaQuerySelectColumn).OrderBy.ToString().StartsWith("Desc"))
                        sql.AppendLine("  [" + col.GetFullAlias() + "] DESC,");
                    else
                        sql.AppendLine("  [" + col.GetFullAlias() + "],");
                }
                sql.RemoveLastChar(3);
                sql.AppendLine();
                if (!string.IsNullOrWhiteSpace(RuntimeOffsetRows))
                {
                    sql.AppendLine("OFFSET " + RuntimeOffsetRows + " ROWS");
                }
                if (!string.IsNullOrWhiteSpace(RuntimeFetchRows))
                {
                    if (string.IsNullOrWhiteSpace(RuntimeOffsetRows))
                        throw new Exception("Запрос "+Name.AsSQL()+": не заполнен RuntimeOffsetRow.");
                    sql.AppendLine("FETCH NEXT " + RuntimeFetchRows + " ROWS ONLY");
                }
            }
            else
            {
               if (!string.IsNullOrWhiteSpace(RuntimeOffsetRows) || !string.IsNullOrWhiteSpace(RuntimeFetchRows))
                   throw new Exception("Запрос " + Name.AsSQL() + ": нельзя использовать RuntimeOffsetRows и/или RuntimeFetchRows без указания сортировки в запросе.");

            }
        }

        public string GetSqlText(List<string> withCTE)
        {
            bool cteMode = withCTE != null;  //режим вложенного sql (Common Table Expression)
            if (!cteMode)
                withCTE = new List<string>();

            var withCTESql = new StringBuilder();
            var commentSql = new StringBuilder();
            var selectSql = new StringBuilder();
            var fromSql = new StringBuilder();
            var joinSql = new StringBuilder();
            var whereSql = new StringBuilder();
            var orderbySql = new StringBuilder();

            whereSql.AppendLine("WHERE");

            commentSql.Append("-- запрос: " + Name + ",  ");
            if (!cteMode)
                commentSql.Append("компьютер: " + System.Environment.MachineName);
            commentSql.AppendLine();


            if (RuntimeTop != null)
                selectSql.AppendLine("SELECT TOP " + RuntimeTop);
            else
                selectSql.AppendLine("SELECT");

            rootColumn.EmitSelectSql(selectSql, "  ");

            if (!string.IsNullOrWhiteSpace(RuntimeIntoTableName))
            {
                selectSql.RemoveLastChar(3);
                selectSql.AppendLine();
                selectSql.AppendLine("INTO [" + RuntimeIntoTableName + "] "); // последний пробел не удалять!
            }

            rootColumn.EmitJoinSql(joinSql, withCTE, "");

            selectSql.RemoveLastChar(3);
            selectSql.AppendLine();

            if (!cteMode && withCTE.Count > 0)
            {
                withCTESql.AppendLine("WITH");

                List<string> duplicates = new List<string>();

                foreach (string cteStr in withCTE)
                {
                    if (!duplicates.Contains(cteStr))
                    {
                        withCTESql.AppendLine(cteStr);
                        duplicates.Add(cteStr);
                    }
                }
                withCTESql.RemoveLastChar(5);
                withCTESql.AppendLine();
            }

            if (MasterTableRow != null)  // фильтруем detail-записи
            {
                var masterColumn = GetRootNativeTable().GetMasterColumn();
                if (masterColumn == null)
                    throw new Exception("Запрос '" + Name + "': в таблице " + GetRootNativeTable().Name + " отсутствует колонка с типом '" + new MasterDataType().GetNameDisplay + "'.");
                whereSql.AppendLine("  [" + GetFullAlias() + "].[" + masterColumn.Name + "]=" + MasterTableRow.ID.AsSQL() + " AND");
            }


            if (!string.IsNullOrWhiteSpace(RuntimeLookupSearchFilterString))
            {
                var searchWords = RuntimeLookupSearchFilterString.Split(' ');
                foreach (var word in searchWords)
                {
                    whereSql.Append("(");
                    foreach (var col in GetAllColumns().Where(col => col is SchemaQuerySelectColumn))
                    {
                        whereSql.Append("[" + col.ParentColumn.GetJoinTableFillAlias() + "].");
                        whereSql.Append("[" + col.Name + "]");
                        whereSql.Append(" LIKE '%" + word.AsLIKE() + "%'");
                        whereSql.Append(" OR ");
                    }
                    whereSql.RemoveLastChar(4);
                    whereSql.AppendLine(") AND");
                }
            }
            if (RuntimeRecordIDFilter != null)
            {
                if (RootColumn.GetJoinView().GetNativeTable() != null)
                    whereSql.AppendLine("[" + RootColumn.GetJoinTableFillAlias() + "].[" + RootColumn.GetJoinView().GetNativeTable().GetPrimaryKeyColumn().Name + "]=" + RuntimeRecordIDFilter.AsSQL() + " AND");
                else
                    whereSql.AppendLine("[" + RootColumn.GetJoinTableFillAlias() + "].[__ID__]=" + RuntimeRecordIDFilter.AsSQL() + " AND");
            }


            if (whereSql.Length > "WHERE\n\n".Length)
                whereSql.RemoveLastChar(5);  // убираем 'AND' с переносом строки
            else
                whereSql.Clear();

            EmitOrderBySql(orderbySql);

            var retSql = commentSql.ToString() + withCTESql.ToString() + selectSql.ToString() + fromSql.ToString() + joinSql.ToString() + whereSql.ToString() + orderbySql.ToString();

#if DEBUG
            File.WriteAllText(@"c:\$\запрос-" + ID.ToString() + "-" + Name.ToString().TranslateToCorrectFileName() + ".txt", retSql);
#endif

            return retSql;
        }


        public void EmitSelectSql(StringBuilder sql, string indent)
        {
            throw new NotImplementedException();
        }

        public void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            throw new NotImplementedException();
        }

        public List<SchemaQueryBaseColumn> GetAllColumns()
        {
            var retList = new List<SchemaQueryBaseColumn>();
            RootColumn.GetAllColumns(retList);
            return retList;
        }


        public string GetFullAlias()
        {
            return RootColumn.GetJoinTableFillAlias();
        }

        public Guid GetRootNativeTableOrRoleID()
        {
            IViewColumn rootView = rootColumn.GetJoinView();
            while (true)
            {
                if (rootView.GetNativeTable() != null)
                    return rootView.GetNativeTable().ID;
                else
                    if (rootView.GetNativeTableRole() != null)
                        return rootView.GetNativeTableRole().ID;
                    else
                        if (rootView.GetNativeVirtualTable() != null)
                            return rootView.GetNativeVirtualTable().ID;

                rootView = rootView.GetRootColumn().GetJoinView();
            }
            throw new Exception("внутрення ошибка B87773C9");
        }


        public SchemaTable GetRootNativeTable()
        {
            IViewColumn rootView = rootColumn.GetJoinView();
            while (true)
            {
                if (rootView.GetNativeTable() != null)
                    return rootView.GetNativeTable();

                rootView = rootView.GetRootColumn().GetJoinView();
            }
            throw new Exception("внутрення ошибка B72223C9");
        }


        public IViewColumn GetRootColumn()
        {
            return rootColumn;
        }


        public Таблица_TableRole GetNativeTableRole()
        {
            return null;
        }

        public Колонка_ColumnRole GetNativeTableColumnRole()
        {
            return null;
        }

        public void GreatePivotFields(PivotGridControl pivotGrid)
        {
            int dataIndex = 0;
            foreach (var col in GetAllColumns().Where(col => col is SchemaQuerySelectColumn))
            {
                PivotGridField field;
                field = pivotGrid.Fields.Add(col.GetFullAlias(), PivotArea.DataArea);
                field.AreaIndex = dataIndex++;
                field.Caption = col.GetFullAlias();
                field.Name = "__" + col.GetFullAlias().AsValidCSharpIdentifier() + "__";
                field.Visible = false;
            }
        }

        public DataTable GreatePivotSampleDataSource()
        {
            var dataTable = new DataTable();

            foreach (var col in GetAllColumns().Where(col => col is SchemaQuerySelectColumn))
            {
                var nativeCol = col.GetSourceTableColumn().GetNativeVirtualTableColumn();
                if (nativeCol != null && nativeCol.DataType.IsNumeric())
                {
                    dataTable.Columns.Add(col.GetFullAlias().AsValidCSharpIdentifier(), typeof(System.Decimal));
                }
                else
                    dataTable.Columns.Add(col.GetFullAlias().AsValidCSharpIdentifier(), typeof(System.String));
            }


            var random = new Random();
            for (int i = 0; i < 2000; i++)
            {
                var row = dataTable.Rows.Add();
                int colIndex = 0;
                foreach (var col in GetAllColumns().Where(col => col is SchemaQuerySelectColumn))
                {
                    var nativeCol = col.GetSourceTableColumn().GetNativeVirtualTableColumn();
                    if (nativeCol != null && nativeCol.DataType.IsNumeric())
                    {
                        row[colIndex++] = (Decimal)random.NextDouble() * 1000;
                    }
                    else
                    {
                        row[colIndex++] = col.GetFullAlias() + " " + (random.Next(1, 10)).ToString("00000");
                    }
                }
            }

            //dataTable.TableName = "Жопа";
            //dataTable.WriteXml(@"c:\$\__test_pivot__.xml");

            return dataTable;
        }


        public SchemaVirtualTable GetNativeVirtualTable()
        {
            return null;
        }

        public SchemaVirtualTableColumn GetNativeVirtualTableColumn()
        {
            return null;
        }
        public override Bitmap GetImage()
        {
            return global::Buhta.Properties.Resources.SchemaQuery_16;
        }

        public bool GetIsSupportReadPermission()
        {
            return false;
        }

        public bool GetIsSupportInsertPermission()
        {
            return false;
        }

        public bool GetIsSupportUpdatePermission()
        {
            return false;
        }

        public bool GetIsSupportDeletePermission()
        {
            return false;
        }

        public bool GetIsSupportOwnedUpdatePermission()
        {
            return false;
        }

        public bool GetIsSupportOwnedDeletePermission()
        {
            return false;
        }

        public virtual string GetPermissionFolder()
        {
            return "Запрос";
        }

        public virtual Guid GetID()
        {
            return ID;
        }

        public virtual string GetFieldName()
        {
            return null;
        }

        public virtual string GetName()
        {
            return GetDisplayName();
        }

        public void GetChildPermissionObjects(List<IPermissionSupportObject> list)
        {

        }
    }


}
