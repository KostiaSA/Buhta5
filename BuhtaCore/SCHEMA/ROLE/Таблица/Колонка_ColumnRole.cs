using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class Колонка_ColumnRole : SchemaBaseRole, IViewColumn
    {
        public Type TableRoleType;
        public Таблица_TableRole Table;
        public bool IsRequiredColumn;
        public bool IsMultiColumn;
        public bool IsIndexed;
        public bool IsNotNullable;
        public SqlDataType DataType;
        public string NewColumnName;
        public Dictionary<string, SqlDataType> AllowedDataTypes = new Dictionary<string, SqlDataType>();

        public Колонка_ColumnRole()
            : base()
        {
            ID = RoleConst.Таблица_Колонка;
            Name = "^Таблица.Колонка";
            Description = "Простая колонка";
            Position = 0;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Новая колонка";

            TableRoleType = typeof(Таблица_TableRole);
        }





        public string GetDisplayName()
        {
            throw new NotImplementedException();
        }

        public string GetDisplayNameAndDataType()
        {
            return Name + ":  " + DataType.GetNameDisplay;
        }

        public string GetFullAlias()
        {
            return Name;
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

        IViewColumn joinView_cached;
        public IViewColumn GetJoinView()
        {
            if (DataType is ForeingKeyDataType)
            {
                if (joinView_cached != null)
                    return joinView_cached;

                if (SchemaBaseRole.Roles.ContainsKey((Guid)(DataType as ForeingKeyDataType).RefTableID))
                    joinView_cached = (IViewColumn)SchemaBaseRole.Roles[(Guid)(DataType as ForeingKeyDataType).RefTableID];
                else
                    joinView_cached = (IViewColumn)App.Schema.GetObject<SchemaObject>((Guid)(DataType as ForeingKeyDataType).RefTableID);

                return joinView_cached;

            }
            else
                return Table;
        }

        public IViewColumn GetRootColumn()
        {
            throw new NotImplementedException();
        }

        public IViewColumn GetColumnByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<IViewColumn> GetColumns()
        {
            throw new NotImplementedException();
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
            return null;
        }

        public Таблица_TableRole GetNativeTableRole()
        {
            return null;
        }

        public Колонка_ColumnRole GetNativeTableColumnRole()
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

        public void EmitSelectSql(StringBuilder sql, string indent)
        {
            throw new NotImplementedException();
        }

        public void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            throw new NotImplementedException();
        }

        public SchemaTable GetRootNativeTable()
        {
            throw new NotImplementedException();
        }

        public string Get4PartsTableName()
        {
            throw new NotImplementedException();
        }
    }
}
