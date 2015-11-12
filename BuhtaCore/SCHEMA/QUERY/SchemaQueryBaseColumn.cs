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
    public enum QueryOrderBy { None = 0, Asc1 = 1, Asc2 = 3, Asc3 = 5, Asc4 = 7, Asc5 = 9, Asc6 = 11, Desc1 = 2, Desc2 = 4, Desc3 = 6, Desc4 = 8, Desc5 = 10, Desc6 = 12 }

    public class SchemaQueryBaseColumn : IViewColumn, ISupportInitialize, TreeList.IVirtualTreeListData, INotifyPropertyChanged
    {
        public const string Колонка_category = "  Колонка";

        private string name;
        [DisplayName("Имя"), Description("Имя колонки"), Category(Колонка_category)]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private string alias;
        [DisplayName("Alias"), Description("Псевдоним колонки"), Category(Колонка_category)]
        public string Alias
        {
            get { return alias; }
            set { alias = value; firePropertyChanged("Alias"); }
        }



        private SchemaQueryJoinColumn parentColumn;
        [Browsable(false)]
        public SchemaQueryJoinColumn ParentColumn
        {
            get { return parentColumn; }
            set { parentColumn = value; firePropertyChanged("ParentColumn"); }
        }

        private SchemaQuery parentQuery;
        [Browsable(false)]
        public SchemaQuery ParentQuery
        {
            get { return parentQuery; }
            set { parentQuery = value; firePropertyChanged("ParentQuery"); }
        }


        private int position;
        [Browsable(false)]
        public int Position
        {
            get { return position; }
            set { position = value; firePropertyChanged("Position"); }
        }

        public virtual void GetAllColumns(List<SchemaQueryBaseColumn> list)
        {
            list.Add(this);
        }

        public virtual List<SchemaQueryBaseColumn> GetAllColumns()
        {
            var list = new List<SchemaQueryBaseColumn>();
            GetAllColumns(list);
            return list;
        }

        public virtual string GetFullAlias()
        {
            if (!string.IsNullOrWhiteSpace(Alias))
                return Alias;
            else
                return (ParentColumn.GetJoinTableFillAlias2() + "_" + Name).Substring(1);

        }

        public string Get4PartsTableName()
        {
            return "[?Get4PartsTableName?]";
        }

        public SchemaQueryBaseColumn()
        {
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
                info.CellData = (info.Node as SchemaQueryBaseColumn).Name;
            else
                if (info.Column.FieldName == "Alias")
                    info.CellData = (info.Node as SchemaQueryBaseColumn).Alias;
                else
                    if (info.Column.FieldName == "Position")
                        info.CellData = (info.Node as SchemaQueryBaseColumn).Position;
                    else
                        info.CellData = null;
        }

        public virtual void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = null;
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
            if (parentColumn != null)
                parentColumn.firePropertyChanged("Columns");
            if (parentQuery != null)
                parentQuery.firePropertyChanged("RootColumn");

        }


        public virtual IViewColumn GetParentViewColumn()
        {
            return ParentColumn;
        }

        public virtual IViewColumn GetSourceView()
        {
            return ParentColumn.GetJoinView();
        }

        public virtual IViewColumn GetSourceViewColumn()
        {
            return GetSourceView().GetColumnByName(Name);
        }

        public virtual IViewColumn GetSourceTableColumn()
        {
            IViewColumn ret = GetSourceViewColumn();
            while (true)
            {
                if (ret.GetNativeTableColumn() != null)
                    return ret.GetNativeTableColumn();
                else
                    if (ret.GetNativeVirtualTableColumn() != null)
                        return ret.GetNativeVirtualTableColumn();
                    else
                        if (ret.GetNativeTableColumnRole() != null)
                            return ret.GetNativeTableColumnRole();
                ret = ret.GetSourceViewColumn();
            }
        }

        public virtual IViewColumn GetJoinView()
        {
            throw new NotImplementedException();
        }


        public IViewColumn GetColumnByName(string name)
        {
            return null;
        }

        public string GetDisplayName()
        {
            return Name;
        }


        public virtual List<IViewColumn> GetColumns()
        {
            return null;
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
            return null;
        }

        public SchemaQueryBaseColumn GetNativeQueryColumn()
        {
            return this;
        }

        public SchemaVirtualTable GetNativeVirtualTable()
        {
            return null;
        }

        public SchemaVirtualTableColumn GetNativeVirtualTableColumn()
        {
            return null;
        }


        public string GetDisplayNameAndDataType()
        {
            return GetFullAlias();
        }


        public virtual void EmitSelectSql(StringBuilder sql, string indent)
        {
            sql.Append(indent);
            sql.Append("[" + ParentColumn.GetJoinTableFillAlias() + "].");
            sql.Append("[" + Name + "]");
            sql.Append(" AS [" + GetFullAlias() + "],");
            sql.AppendLine();
        }

        public virtual void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            return;
        }



        public SchemaTable GetRootNativeTable()
        {
            throw new NotImplementedException();
        }


        public IViewColumn GetRootColumn()
        {
            throw new NotImplementedException();
        }

        public Таблица_TableRole GetNativeTableRole()
        {
            return null;
        }

        public Колонка_ColumnRole GetNativeTableColumnRole()
        {
            return null;
        }

        public SqlDataType GetNativeDataType()
        {
            if (GetNativeTableColumn() != null)
                return GetNativeTableColumn().DataType;
            else
                if (GetNativeTableColumnRole() != null)
                    return GetNativeTableColumnRole().DataType;
                else
                    if (GetNativeVirtualTableColumn() != null)
                        return GetNativeVirtualTableColumn().DataType;
                    else
                        return null;
        }
    }


}
