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

    public class SchemaQueryJoinColumn : SchemaQueryBaseColumn
    {
        Guid? foreingQueryTableID;
        [Editor(typeof(SchemaTableOrQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaTableOrQuerySelectorTypeConverter))]
        [DisplayName("Внешняя таблица/запрос")]
        public Guid? ForeingQueryTableID
        {
            get { return foreingQueryTableID; }
            set { foreingQueryTableID = value; joinView_cached = null; firePropertyChanged("SourceQueryTableID"); }
        }

        public ObservableCollection<SchemaQueryBaseColumn> Columns { get; private set; }

        public SchemaQueryJoinColumn()
        {
            Columns = new ObservableCollection<SchemaQueryBaseColumn>();
            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        public override void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = Columns;
        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Columns");
        }

        IViewColumn joinView_cached;
        public override IViewColumn GetJoinView()
        {
            if (foreingQueryTableID == null)
            {
                if (GetSourceView() != null && GetSourceView().GetColumnByName(Name) != null)
                    return GetSourceView().GetColumnByName(Name).GetJoinView();
                else
                    return null;
            }
            else
            {
                if (joinView_cached != null)
                    return joinView_cached;

                if (SchemaBaseRole.Roles.ContainsKey((Guid)foreingQueryTableID))
                    joinView_cached = (IViewColumn)SchemaBaseRole.Roles[(Guid)foreingQueryTableID];
                else
                    joinView_cached = (IViewColumn)App.Schema.GetObject<SchemaObject>((Guid)foreingQueryTableID);

                return joinView_cached;
            }
        }


        public override void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            if (foreingQueryTableID == null)
            {
                if (info.Column.FieldName == "Name")
                {
                    if (GetJoinView() != null)
                        info.CellData = Name + " -> " + GetJoinView().GetDisplayName();
                    else
                        info.CellData = Name + " -> ?";
                }
                else
                    if (info.Column.FieldName == "Position")
                        info.CellData = (info.Node as SchemaQueryBaseColumn).Position;
                    else
                        info.CellData = null;
            }
            else
            {
                if (info.Column.FieldName == "Name")
                {
                    if (GetJoinView() != null)
                        info.CellData = Name + " -> " + GetJoinView().GetDisplayName();
                    else
                        info.CellData = Name + " -> ?";
                }
                else
                    if (info.Column.FieldName == "Position")
                        info.CellData = (info.Node as SchemaQueryBaseColumn).Position;
                    else
                        info.CellData = null;
            }
        }

        //public override IViewColumn GetJoinView()
        //{
        //    if (GetSourceView() != null && GetSourceView().GetColumnByName(Name) != null)
        //        return GetSourceView().GetColumnByName(Name).GetJoinView();
        //    else
        //        return null;
        //}

        public virtual string GetJoinTableFillAlias()
        {
            if (!string.IsNullOrWhiteSpace(Alias))
                return ParentColumn.GetJoinTableFillAlias() + "_" + Alias;
            else
                return ParentColumn.GetJoinTableFillAlias() + "_" + Name;
        }

        public virtual string GetJoinTableFillAlias2()
        {
            if (!string.IsNullOrWhiteSpace(Alias))
                return ParentColumn.GetJoinTableFillAlias2() + "_" + Alias;
            else
                return ParentColumn.GetJoinTableFillAlias2() + "_" + Name;
        }

        public override void EmitSelectSql(StringBuilder sql, string indent)
        {
            foreach (var col in Columns)
                col.EmitSelectSql(sql, indent);

        }

        public override void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            if (GetJoinView().GetNativeTableRole() != null)
            {
                var sb = new StringBuilder();
                sb.Append("[" + GetJoinView().Name + "](");

                foreach (var col in GetJoinView().GetColumns())
                    //if (col is SchemaQuerySelectColumn)
                    sb.Append("[" + col.GetFullAlias() + "],");
                sb.Append("[__ID__],");
                sb.RemoveLastChar();

                sb.Append(") AS");
                sb.AppendLine("(");
                sb.Append(GetJoinView().GetNativeTableRole().GetSqlText(withCTE, null));
                sb.AppendLine("),");
                withCTE.Add(sb.ToString());
            }
            else
                if (GetJoinView().GetNativeQuery() != null)
                {
                    var sb = new StringBuilder();
                    sb.Append("[" + GetJoinView().Name + "](");

                    foreach (var col in GetJoinView().GetColumns())
                        if (col is SchemaQuerySelectColumn)
                            sb.Append("[" + col.GetFullAlias() + "],");
                    sb.Append("[__ID__],");
                    sb.RemoveLastChar();

                    sb.Append(") AS");
                    sb.AppendLine("(");
                    sb.Append(GetJoinView().GetNativeQuery().GetSqlText(withCTE));
                    sb.AppendLine("),");
                    withCTE.Add(sb.ToString());
                }

            sql.AppendLine(indent + "LEFT JOIN " + GetJoinView().Get4PartsTableName() + " AS [" + GetJoinTableFillAlias() + "]");

            foreach (var col in Columns)
                col.EmitJoinSql(sql, withCTE, indent + "  ");

            var keyFieldName = "__ID__";
            if (GetJoinView().GetNativeTable() != null)
                keyFieldName = GetJoinView().GetNativeTable().GetPrimaryKeyColumn().Name;

            sql.AppendLine(indent + "ON [" + GetJoinTableFillAlias() + "].[" + keyFieldName + "] = [" + ParentColumn.GetJoinTableFillAlias() + "].[" + Name + "]");
        }

        public override void GetAllColumns(List<SchemaQueryBaseColumn> list)
        {
            list.Add(this);
            foreach (var col in Columns)
                col.GetAllColumns(list);
        }

        //public override void EmitJoinOnSql(StringBuilder sql, string indent)
        //{
        //    sql.AppendLine("FROM");
        //    sql.AppendLine("  [" + GetJoinView().Name + "] AS [" + GetJoinTableFillAlias() + "]");

        //    foreach (var col in Columns)
        //        col.EmitJoinSql(sql, indent + "  ");
        //}

        //public override IView GetJoinQueryTable()
        //{
        //    return GetSourceColumn().GetQueryTable();
        //}

        //public virtual IView GetSourceTable()
        //{
        //    return ParentColumn.GetJoinQueryTable();
        //}
    }


}
