using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    //   [Export(typeof(SchemaObject))]
    public partial class SchemaTable : SchemaObject, ISupportInitialize, IViewColumn
    {

        public string GetGenStoredProcName()
        {
            return "__генерация__" + Name + "__";
        }

        public void CreateGenStoredProc()
        {
            var sql = new StringBuilder();
            sql.AppendLine("IF OBJECT_ID('dbo." + GetGenStoredProcName() + "') IS NULL");
            sql.AppendLine("EXEC('CREATE PROCEDURE " + GetGenStoredProcName() + " AS SET NOCOUNT ON;')");

            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
                db.SetCommand(GetGenStoredProcSql()).ExecuteNonQuery();
            }

        }

        public void ExecGenStoredProc(object recordID)
        {
            var sql = " EXEC [" + GetGenStoredProcName() + "] " + recordID.AsSQL();

            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

        }

        public string GetGenStoredProcSql()
        {
            var sql = new StringBuilder();

            sql.AppendLine("ALTER PROCEDURE [" + GetGenStoredProcName() + "](@masterID uniqueidentifier) AS");
            sql.AppendLine("BEGIN");
            sql.AppendLine("  SET NOCOUNT ON");

            EmitCreateTempTables(sql, "  ");

            foreach (var detail in Details)
            {
                foreach (var oper in detail.Opers)
                {
                    foreach (var prov in oper.Provodkas)
                    {
                        EmitGenerateProvodkaToTempTable(sql, "  ", prov);
                    }

                }
            }

            sql.AppendLine("  BEGIN TRAN");
            EmitDeleteOldProvodkas(sql, "    ");
            EmitSaveRegistersFromTempTables(sql, "    ");
            sql.AppendLine("  COMMIT");

            sql.AppendLine("END");


            return sql.ToString();
        }

        string GetRegistrTempTableName()
        {
            if (!TableRoles.Contains(RoleConst.Регистр))
                throw new Exception("internal error D9F0EFA4");
            return "[#" + Name + "]";
        }

        void EmitCreateRegisterTempTable(StringBuilder sql, string indent, SchemaTable registrTable)
        {
            sql.AppendLine(indent + "CREATE TABLE " + registrTable.GetRegistrTempTableName() + "(");
            foreach (var col in registrTable.Columns)
            {
                sql.AppendLine(indent + "  [" + col.Name + "] " + col.DataType.GetDeclareSql() + ",");
            }
            sql.RemoveLastChar(3);
            sql.AppendLine();
            sql.AppendLine(indent + ")");
        }

        void EmitCreateTempTables(StringBuilder sql, string indent)
        {
            var registers = GetGenRegistersHashSet();

            foreach (var registerID in registers)
            {
                EmitCreateRegisterTempTable(sql, indent, App.Schema.GetSampleObject<SchemaTable>((Guid)registerID));
            }

        }


        public bool GetIsProvodkaGenerationExists()
        {
            foreach (var detail in Details)
            {
                foreach (var oper in detail.Opers)
                {
                    foreach (var prov in oper.Provodkas)
                    {
                        if (prov.DbRegistrID != null)
                        {
                            return true;
                        }
                        else
                            if (prov.KrRegistrID != null)
                            {
                                return true;
                            }
                    }

                }
            }
            return false;
        }

        HashSet<Guid> GetGenRegistersHashSet()
        {
            var registers = new HashSet<Guid>();

            foreach (var detail in Details)
            {
                foreach (var oper in detail.Opers)
                {
                    foreach (var prov in oper.Provodkas)
                    {
                        if (prov.DbRegistrID != null)
                        {
                            if (!registers.Contains((Guid)prov.DbRegistrID))
                            {
                                registers.Add((Guid)prov.DbRegistrID);
                            }
                        }
                        if (prov.KrRegistrID != null)
                        {
                            if (!registers.Contains((Guid)prov.KrRegistrID))
                            {
                                registers.Add((Guid)prov.KrRegistrID);
                            }
                        }
                    }

                }
            }
            return registers;
        }

        void EmitSaveRegistersFromTempTables(StringBuilder sql, string indent)
        {
            var registers = GetGenRegistersHashSet();
            foreach (var registerID in registers)
            {
                EmitSaveRegistrFromTempTable(sql, indent, App.Schema.GetSampleObject<SchemaTable>(registerID));
            }
        }

        void EmitDeleteOldProvodkas(StringBuilder sql, string indent)
        {

            sql.AppendLine(indent + "-- удаление старых проводок");
            sql.AppendLine(indent + "DECLARE @provodkaRecordID_for_delete uniqueidentifier");
            sql.AppendLine(indent + "DECLARE @registrID_for_delete uniqueidentifier");
            sql.AppendLine(indent + "DECLARE cursor_for_delete CURSOR LOCAL FAST_FORWARD");
            sql.AppendLine(indent + "FOR ");
            sql.AppendLine(indent + "  SELECT  provodkaRecordID, registrID");
            sql.AppendLine(indent + "  FROM " + GetProvodkasTable().Get4PartsTableName());
            sql.AppendLine(indent + "  WHERE tableRecordID=@masterID");

            sql.AppendLine(indent + "OPEN cursor_for_delete");
            sql.AppendLine(indent + "FETCH NEXT FROM cursor_for_delete INTO @provodkaRecordID_for_delete, @registrID_for_delete");

            sql.AppendLine(indent + "WHILE @@FETCH_STATUS = 0");
            sql.AppendLine(indent + "BEGIN");
            sql.AppendLine(indent + "    EXEC __Удаление_проводки__ @provodkaRecordID_for_delete, @registrID_for_delete");
            sql.AppendLine(indent + "    FETCH NEXT FROM cursor_for_delete INTO @provodkaRecordID_for_delete, @registrID_for_delete");
            sql.AppendLine(indent + "END");

            sql.AppendLine(indent + "CLOSE cursor_for_delete");
            sql.AppendLine(indent + "DEALLOCATE cursor_for_delete ");
            sql.AppendLine(indent + "DELETE FROM " + GetProvodkasTable().Get4PartsTableName() + " WHERE tableRecordID=@masterID");
            sql.AppendLine();

            //foreach (var registrTable in App.Schema.GetSampleObjects<SchemaTable>().Where(table => table.TableRoles.Contains(RoleConst.Регистр)))
            //  sql.AppendLine(indent + "DELETE FROM " + registrTable.Get4PartsTableName() + " WHERE [" + registrTable.GetColumnByRole(RoleConst.Регистр_Мастер).Name + "]=@masterID");

        }

        void EmitSaveRegistrFromTempTable(StringBuilder sql, string indent, SchemaTable registrTable)
        {
            sql.Append(indent + "INSERT " + registrTable.Get4PartsTableName() + "(");
            foreach (var col in registrTable.Columns)
                sql.Append("[" + col.Name + "],");
            sql.RemoveLastChar(1);
            sql.Append(") SELECT ");
            foreach (var col in registrTable.Columns)
                sql.Append("[" + col.Name + "],");
            sql.RemoveLastChar(1);
            sql.AppendLine(" FROM " + registrTable.GetRegistrTempTableName());

            sql.Append(indent + "INSERT " + GetProvodkasTable().Get4PartsTableName() + "(tableRecordID,provodkaRecordID,registrID)");
            sql.AppendLine(" SELECT @masterID,[" + registrTable.GetPrimaryKeyColumn().Name + "]," + registrTable.ID.AsSQL() + " FROM " + registrTable.GetRegistrTempTableName());

        }


        void EmitGenerateProvodkaToTempTable(StringBuilder sql, string indent, SchemaTableProvodka prov)
        {
            var masterTable = this;
            var detailTable = prov.OwnerTableOper.TableDetail.GetDetailTable();
            var operID = prov.OwnerTableOper.ID;

            if (prov.DbRegistrID != null)
            {
                var registr = App.Schema.GetSampleObject<SchemaTable>((Guid)prov.DbRegistrID);
                var registrFields = prov.DbRegistrFields;
                EmitGenerateProvodkaDbKrPartToTempTable(sql, indent, prov, DbKrSaldo.Дебет, registr, registrFields);
            }

            if (prov.KrRegistrID != null)
            {
                var registr = App.Schema.GetSampleObject<SchemaTable>((Guid)prov.KrRegistrID);
                var registrFields = prov.KrRegistrFields;
                EmitGenerateProvodkaDbKrPartToTempTable(sql, indent, prov, DbKrSaldo.Кредит, registr, registrFields);
            }

        }

        void EmitGenerateProvodkaDbKrPartToTempTable(StringBuilder sql, string indent, SchemaTableProvodka prov, DbKrSaldo dbKr, SchemaTable registr, ObservableCollection<SchemaTableProvodkaField> registrFields)
        {
            var masterTable = this;
            var detailTable = prov.OwnerTableOper.TableDetail.GetDetailTable();
            var operID = prov.OwnerTableOper.ID;

            sql.AppendLine(indent + "-- " + dbKr + ", регистр: " + registr.Name.AsSQL() + ",  деталь: " + prov.OwnerTableOper.TableDetail.Name.AsSQL() + ",  операция: '[" + prov.OwnerTableOper.Num + "]  " + prov.OwnerTableOper.Name + "',  проводка: " + prov.Name.AsSQL());
            sql.AppendLine(indent + "INSERT " + registr.GetRegistrTempTableName() + "(");

            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Таблица_Ключ).Name + "],");
            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_ДбКр).Name + "],");
            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_КонфигДеталь).Name + "],");
            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_КонфигБизнесОперация).Name + "],");
            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_КонфигПроводка).Name + "],");

            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_Мастер).Name + "],");
            sql.AppendLine(indent + "  [" + registr.GetColumnByRole(RoleConst.Регистр_Деталь).Name + "],");

            foreach (var field in registrFields.Where(field => registr.GetColumnByName(field.RegistrFieldName) != null))
            {
                sql.AppendLine(indent + "  [" + field.RegistrFieldName + "],");
            }

            sql.RemoveLastChar(3);
            sql.AppendLine();

            sql.AppendLine(indent + ")");
            sql.AppendLine(indent + "SELECT");

            sql.AppendLine(indent + "  NewID(),");
            sql.AppendLine(indent + "  '" + dbKr.ToString()[0] + "',");
            sql.AppendLine(indent + "  " + prov.OwnerTableOper.TableDetail.ID.AsSQL() + ",  -- деталь: " + prov.OwnerTableOper.TableDetail.Name);
            sql.AppendLine(indent + "  " + prov.OwnerTableOper.ID.AsSQL() + ",  -- бизнес-операция: " + prov.OwnerTableOper.Name);
            sql.AppendLine(indent + "  " + prov.ID.AsSQL() + ",  -- проводка: " + prov.Name);


            sql.AppendLine(indent + "  @masterID,");
            sql.AppendLine(indent + "  " + detailTable.Get4PartsTableName() + ".[" + detailTable.GetColumnByRole(RoleConst.Таблица_Ключ).Name + "],");

            foreach (var field in registrFields.Where(field => registr.GetColumnByName(field.RegistrFieldName) != null))
            {
                if (field.DataFieldName == "<пусто>")
                    sql.AppendLine(indent + "  NULL,");
                else
                {
                    if (dbKr == DbKrSaldo.Кредит && registr.GetColumnByName(field.RegistrFieldName).ColumnRoles.Contains(RoleConst.Регистр_Мера))
                        sql.AppendLine(indent + "  -" + detailTable.Get4PartsTableName() + ".[" + field.DataFieldName + "],");
                    else
                        sql.AppendLine(indent + "  " + detailTable.Get4PartsTableName() + ".[" + field.DataFieldName + "],");
                }
            }

            sql.RemoveLastChar(3);
            sql.AppendLine();


            sql.AppendLine(indent + "FROM " + detailTable.Get4PartsTableName());
            sql.AppendLine(indent + "WHERE " + detailTable.Get4PartsTableName() + ".[" + detailTable.GetColumnByRole(RoleConst.ВложеннаяТаблица_БизнесОперация).Name + "]=" + operID.AsSQL() + " AND ");
            sql.AppendLine(indent + "      " + detailTable.Get4PartsTableName() + ".[" + detailTable.GetColumnByRole(RoleConst.ВложеннаяТаблица_Мастер).Name + "]=@masterID");
        }

    }
}
