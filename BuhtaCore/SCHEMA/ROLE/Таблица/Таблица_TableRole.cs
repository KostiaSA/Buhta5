using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class Таблица_TableRole : SchemaBaseRole, IViewColumn
    {
        public List<Колонка_ColumnRole> AllColumns;
        public List<Колонка_ColumnRole> Columns;
        public Таблица_TableRole()
            : base()
        {
            ID = RoleConst.Таблица;
            Name = "^Таблица";
            Description = "Простая sql-таблица";
            Position = 1000000;
        }

        public Guid? DefaultQueryID;

        public Guid? LookupQueryID;

        public override void Initialize()
        {
            base.Initialize();
            AllColumns = new List<Колонка_ColumnRole>();
            Columns = new List<Колонка_ColumnRole>();

            foreach (var role in Roles)
            {
                if (role.Value is Колонка_ColumnRole)
                {
                    if ((this.GetType().IsSubclassOf((role.Value as Колонка_ColumnRole).TableRoleType)) ||
                        (role.Value as Колонка_ColumnRole).TableRoleType == (this.GetType()))
                    {
                        var column = (Колонка_ColumnRole)Activator.CreateInstance(role.Value.GetType());
                        column.Table = this;
                        column.Initialize();
                        AllColumns.Add(column);
                    }
                    if ((role.Value as Колонка_ColumnRole).TableRoleType == this.GetType())
                    {
                        var column = (Колонка_ColumnRole)Activator.CreateInstance(role.Value.GetType());
                        column.Table = this;
                        column.Initialize();
                        Columns.Add(column);
                    }
                }
            }

            foreach (var table in App.Schema.GetSampleObjects<SchemaTable>().Where(tbl => tbl.TableRoles.Contains(ID)))
            {
                var column = new Колонка_ColumnRole();
                column.Table = this;
                column.Initialize();
                column.ID = Guid.Empty;
                column.Name = table.DisplayName;
                column.Description = "";
                column.Position = 0;
                column.IsRequiredColumn = false;
                column.IsMultiColumn = false;
                column.IsIndexed = false;
                column.DataType = new ForeingKeyDataType() { RefTableID = table.ID };
                column.NewColumnName = table.DisplayName;
                Columns.Add(column);
                AllColumns.Add(column);
            }

        }

        ////public override void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    var list = new List<SchemaBaseRole>();

        ////    // колонки таблицы
        ////    foreach (var role in Roles)
        ////    {
        ////        if (role.Value is Колонка_ColumnRole)
        ////        {
        ////            if ((role.Value as Колонка_ColumnRole).TableRoleType == this.GetType())
        ////            {
        ////                list.Add(role.Value);
        ////            }
        ////        }
        ////    }

        ////    // таблицы-наследники
        ////    foreach (var role in Roles)
        ////    {
        ////        if (role.Value.GetType().BaseType.Equals(this.GetType()))
        ////        {
        ////            list.Add(role.Value);
        ////        }
        ////    }
        ////    if (list.Count > 0)
        ////        info.Children = list;
        ////    else
        ////        info.Children = null;
        ////}

        public override string DisplayName { get { return GetDisplayName(); } }


        public string GetDisplayName()
        {
            return Name;
        }

        public string GetDisplayNameAndDataType()
        {
            throw new NotImplementedException();
        }

        public string GetFullAlias()
        {
            throw new NotImplementedException();
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

        public IViewColumn GetRootColumn()
        {
            throw new NotImplementedException();
        }

        public IViewColumn GetColumnByName(string name)
        {
            foreach (var col in GetColumns())
            {
                if (col.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return col;
                }
            }
            return null;
        }

        public List<IViewColumn> GetColumns()
        {
            return AllColumns.Where(col => !col.IsMultiColumn).ToList<IViewColumn>();
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
            return this;
        }

        public Колонка_ColumnRole GetNativeTableColumnRole()
        {
            return null;
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
            return "[" + Name + "]";
        }

        public string GetSqlText(List<string> withCTE, Guid? recordID)
        {
            var sql = new StringBuilder();
            foreach (var table in App.Schema.GetSampleObjects<SchemaTable>().Where(table => table.TableRoles.Contains(this.ID)))
            {
                if (sql.Length != 0)
                    sql.AppendLine("UNION ALL");

                sql.AppendLine("SELECT");
                //var selectSql = new StringBuilder();

                foreach (var roleCol in AllColumns.Where(col => !col.IsMultiColumn))
                {
                    if (roleCol.DataType is ForeingKeyDataType)
                    {
                        if ((roleCol.DataType as ForeingKeyDataType).RefTableID == table.ID)
                        {
                            sql.AppendLine("  [" + table.GetPrimaryKeyColumn().Name + "] AS [" + roleCol.Name + "],");
                            goto m1;
                        }
                    }
                    else
                        foreach (var tableCol in table.Columns)
                        {
                            if (tableCol.ColumnRoles.Contains(roleCol.ID))
                            {
                                sql.AppendLine("  [" + tableCol.Name + "] AS [" + roleCol.Name + "],");
                                goto m1;
                            }
                        }
                    if (roleCol.DataType is ForeingKeyDataType)
                        sql.AppendLine("  '00000000-0000-0000-0000-000000000000' AS [" + roleCol.Name + "],"); // не NULL - обход странного глюка с линкованными серверами
                    else
                        sql.AppendLine("  NULL AS [" + roleCol.Name + "],");
                m1: ;
                }

                sql.AppendLine("  [" + table.GetPrimaryKeyColumn().Name + "] AS __ID__");
                //sql.RemoveLastChar(3);
                //sql.AppendLine();
                sql.AppendLine("FROM " + table.Get4PartsTableName());
                if (recordID != null)
                    sql.AppendLine("WHERE [" + table.GetPrimaryKeyColumn().Name + "]=" + recordID.AsSQL());
            }
            return sql.ToString();
            //bool cteMode = withCTE != null;  //режим вложенного sql (Common Table Expression)
            //if (!cteMode)
            //    withCTE = new List<string>();

            //var withCTESql = new StringBuilder();
            //var commentSql = new StringBuilder();
            //var selectSql = new StringBuilder();
            //var fromSql = new StringBuilder();
            //var joinSql = new StringBuilder();
            //var whereSql = new StringBuilder();
            //var orderbySql = new StringBuilder();

            //whereSql.AppendLine("WHERE");

            //commentSql.Append("-- роль: " + Name + ",  ");
            //if (!cteMode)
            //    commentSql.Append("компьютер: " + System.Environment.MachineName);
            //commentSql.AppendLine();


            //selectSql.AppendLine("SELECT");

            //rootColumn.EmitSelectSql(selectSql, "  ");
            //rootColumn.EmitJoinSql(joinSql, withCTE, "");

            //selectSql.RemoveLastChar(3);
            //selectSql.AppendLine();

            //if (!cteMode && withCTE.Count > 0)
            //{
            //    withCTESql.AppendLine("WITH");

            //    List<string> duplicates = new List<string>();

            //    foreach (string cteStr in withCTE)
            //    {
            //        if (!duplicates.Contains(cteStr))
            //        {
            //            withCTESql.AppendLine(cteStr);
            //            duplicates.Add(cteStr);
            //        }
            //    }
            //    //for (int i = withCTE.Count - 1; i >= 0; i--)
            //    //{
            //    //    withCTESql.AppendLine(withCTE[i]);
            //    //}
            //    withCTESql.RemoveLastChar(5);
            //    withCTESql.AppendLine();
            //}

            //if (MasterTableRow != null)  // фильтруем detail-записи
            //{
            //    var masterColumn = GetRootNativeTable().GetMasterColumn();
            //    if (masterColumn == null)
            //        throw new Exception("Запрос '" + Name + "': в таблице " + GetRootNativeTable().Name + " отсутствует колонка с типом '" + new MasterDataType().GetNameDisplay + "'.");
            //    whereSql.AppendLine("  [" + GetFullAlias() + "].[" + masterColumn.Name + "]=" + MasterTableRow.ID.AsSQL() + " AND");
            //}
            //if (whereSql.Length > "WHERE\n\n".Length)
            //    whereSql.RemoveLastChar(5);  // убираем 'AND' с переносом строки
            //else
            //    whereSql.Clear();

            //return commentSql.ToString() + withCTESql.ToString() + selectSql.ToString() + fromSql.ToString() + joinSql.ToString() + whereSql.ToString() + orderbySql.ToString();
        }
    }
}
