using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    /*
CREATE TABLE [dbo].[__undo__](
	[SessionID] [uniqueidentifier] NOT NULL,
	[ParentSessionID] [uniqueidentifier] NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[TableID] [uniqueidentifier] NOT NULL,
	[RecordID] [uniqueidentifier] NOT NULL,
	[UndoSql] [nvarchar](max) NOT NULL,
	[TimeStamp] [timestamp] NOT NULL,
	[Time] [datetime] NOT NULL CONSTRAINT [DF___undo___Time]  DEFAULT (getdate())
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
     */

    public enum CrudMode { View = 0, Add = 1, Edit = 2, Delete = 3 }
    public delegate void OnSaveChangesEventHandler();
    public delegate void OnInitNewRecordEventHandler();
    public delegate void SaveChangesEventHandler(SchemaTableRow sender);

    public class SchemaTableRow
    {
        public enum ValueState { Unbind = 0, Unchanged = 1, Changed = 2, BindError = 3, UserError = 4, Empty = 5 }

        public Guid? EditSessionID;
        public SchemaTableRow MasterTableRow;
        public SchemaTable Table;
        public object ID;
        public CrudMode CrudMode;
        public Guid? NewBussinesOperID;

        public Dictionary<string, object> Values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, object> NewValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public event EventHandler OnLoadData;
        public event EventHandler OnSaveData;
        public event EventHandler OnCancelChanges;
        public event SaveChangesEventHandler BeforeSaveChanges;

        public object this[string columnName]
        {
            get
            {
                if (NewValues.ContainsKey(columnName))
                    return NewValues[columnName];
                else
                    if (Values.ContainsKey(columnName))
                        return Values[columnName];
                    else
                        if (Table.GetColumnByName(columnName) == null)
                            throw new Exception("Не найдена колонка '" + columnName + "' таблице '" + Table.Name + "'");
                        else
                            if (CrudMode == Buhta.CrudMode.Add)
                                return null;
                            else
                                throw new Exception("внутрення ошибка 0D5DAF60");

            }
            set
            {
                if (Table.GetColumnByName(columnName) == null)
                    throw new Exception("Не найдена колонка '" + columnName + "' таблице '" + Table.Name + "'");

                if (NewValues.ContainsKey(columnName))
                    NewValues[columnName] = value;
                else
                    NewValues.Add(columnName, value);

                if (Values.ContainsKey(columnName) && ((value == null && Values[columnName] == null) || (value != null && value.Equals(Values[columnName]))))
                    NewValues.Remove(columnName);

            }

        }

        bool needSave;
        public void SetNeedSave()
        {
            needSave = true;
        }

        public bool GetNeedSave()
        {
            return needSave || NewValues.Count > 0;
        }

        public ValueState GetValueState(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return ValueState.Unbind;
            else
                if (Table.GetColumnByName(columnName) == null)
                    return ValueState.BindError;
                else
                    if (NewValues.ContainsKey(columnName))
                    {
                        if (NewValues[columnName] == null || NewValues[columnName].ToString() == "")
                            return ValueState.Empty;
                        else
                            return ValueState.Changed;
                    }
                    else
                        if (Values.ContainsKey(columnName))
                        {
                            if (Values[columnName] == null || Values[columnName].ToString() == "")
                                return ValueState.Empty;
                            else
                                return ValueState.Unchanged;
                        }
                        else
                            return ValueState.Empty;

        }

        void CheckParams()
        {
            if (Table == null)
                throw new Exception("SchemaTableRow: не заполнено св-во 'Table'");

            if (ID.Equals(Guid.Empty))
                throw new Exception("SchemaTableRow: не заполнено св-во 'ID'");
        }

        public void LoadData()
        {
            CheckParams();
            if (CrudMode == CrudMode.View || CrudMode == CrudMode.Edit)
            {
                var sql = new StringBuilder();
                sql.AppendLine("SELECT ");

                foreach (var col in Table.Columns)
                {
                    //if (col.Name != Table.GetPrimaryKeyColumn().Name)
                        sql.AppendLine("  [" + col.Name + "],");
                }
                sql.RemoveLastChar(3);
                sql.AppendLine();
                sql.AppendLine("FROM " + Table.Get4PartsTableName());
                sql.AppendLine("WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL());

                using (var db = App.Schema.SqlDB.GetDbManager())
                {
                    var dataTable = db.SetCommand(sql.ToString()).ExecuteDataTable();
                    if (dataTable.Rows.Count == 0)
                        throw new Exception("Запись не найдена в таблице " + Table.Get4PartsTableName() + ", " + Table.GetPrimaryKeyColumn().Name + "='" + ID + "'");

                    foreach (var col in Table.Columns)
                    {
                        if (col.Name != Table.GetPrimaryKeyColumn().Name)
                            Values.Add(col.Name, dataTable.Rows[0][col.Name]);
                    }
                }
            }
            if (OnLoadData != null)
                OnLoadData(this, null);


        }

        public void SaveData()
        {
            CheckParams();
            if (CrudMode == CrudMode.View)
            {
                throw new Exception("Запись только на чтение в таблице '" + Table.Name + "', ID='" + ID + "'");
            }
            else
                if (CrudMode == CrudMode.Add)
                {
                    if (MasterTableRow != null)
                    {
                        var masterColumn = Table.GetMasterColumn();
                        if (masterColumn == null)
                            throw new Exception("'В таблице '" + Table.Name + "' отсутствует колонка с ролью '^ВложеннаяТаблица.Мастер'.");

                        this[masterColumn.Name] = MasterTableRow.ID;
                    }
                    if (NewBussinesOperID != null)
                    {
                        var bussinesOperColumn = Table.GetColumnByRole(RoleConst.ВложеннаяТаблица_БизнесОперация);
                        if (bussinesOperColumn == null)
                            throw new Exception("'В таблице '" + Table.Name + "' отсутствует колонка с ролью '^ВложеннаяТаблица.БизнесОперация'.");

                        this[bussinesOperColumn.Name] = NewBussinesOperID;
                    }
                    if (!NewValues.ContainsKey(Table.GetPrimaryKeyColumn().Name))
                        NewValues.Add(Table.GetPrimaryKeyColumn().Name, ID);

                    if (!NewValues.ContainsKey("__changeuser__"))
                        NewValues.Add("__changeuser__", App.UserID);
                    else
                        NewValues["__changeuser__"] = App.UserID;

                    if (!NewValues.ContainsKey("__changetime__"))
                        NewValues.Add("__changetime__", DateTime.Now);

                    if (NewValues.Values.Count > 0)
                    {

                        var sql = new StringBuilder();
                        sql.AppendLine("BEGIN TRAN");

                        sql.AppendLine("INSERT " + Table.Get4PartsTableName() + " (");

                        foreach (var col in Table.Columns)
                        {
                            if (NewValues.ContainsKey(col.Name))
                                sql.AppendLine("  [" + col.Name + "],");
                        }

                        sql.RemoveLastChar(3);
                        sql.AppendLine(")");
                        sql.AppendLine("VALUES (");
                        foreach (var col in Table.Columns)
                        {
                            if (NewValues.ContainsKey(col.Name))
                                if (col.Name == "__changetime__")
                                    sql.AppendLine("GetDate(),");
                                else
                                    sql.AppendLine(NewValues[col.Name].AsSQL() + ",");
                        }
                        sql.RemoveLastChar(3);
                        sql.AppendLine(")");

                        //// __undo__
                        //if (MasterTableRow != null)
                        //{
                        //    var undo_sql = "DELETE FROM [" + Table.Name + "] WHERE ID=" + ID.AsSQL();

                        //    sql.AppendLine("INSERT [__EditSession__]([SessionID],[ParentSessionID],[TableID],[RecordID],[UserID],[UndoSql])");
                        //    sql.AppendLine("VALUES (" + EditSessionID.AsSQL() + "," + MasterTableRow.EditSessionID.AsSQL() + "," + Table.ID.AsSQL() + "," +
                        //                                ID.AsSQL() + "," + App.UserID.AsSQL() + "," + undo_sql.AsSQL() + ")");
                        //}
                        //else
                        //{
                        //    foreach (var detailTable in App.Schema.GetAllDetailTablesSamples())
                        //    {
                        //        sql.AppendLine("DELETE FROM [" + detailTable.Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + ID.Reverse().AsSQL());
                        //    }
                        //    sql.Append("EXEC __EditSession__Commit ");
                        //    sql.Append("@sessionID=" + EditSessionID.AsSQL() + ",");
                        //    sql.AppendLine("@parentSessionID=NULL");
                        //}
                        sql.AppendLine("COMMIT");


                        if (MasterTableRow != null && MasterTableRow.BeforeSaveChanges != null)
                            MasterTableRow.BeforeSaveChanges(this);

                        if (BeforeSaveChanges != null)
                            BeforeSaveChanges(this);

                        using (var db = App.Schema.SqlDB.GetDbManager())
                        {
                            int updatedRows = db.SetCommand(sql.ToString()).ExecuteNonQuery();
                            if (updatedRows == 0)
                                throw new Exception("Ошибка добавления записи в таблицу '" + Table.Name + "', ID='" + ID + "'");
                        }
                        CrudMode = Buhta.CrudMode.Edit;

                        foreach (var columnName in NewValues.Keys)
                        {
                            if (Values.ContainsKey(columnName))
                                Values[columnName] = NewValues[columnName];
                            else
                                Values.Add(columnName, NewValues[columnName]);
                        }
                        NewValues.Clear();
                    }
                }
                else
                    if (CrudMode == CrudMode.Edit)
                    {
                        if (NewValues.ContainsKey(Table.GetPrimaryKeyColumn().Name))
                            throw new Exception("Запрещено изменять ID в таблице '" + Table.Name + "', ID='" + ID + "'");

                        //var log = new StringBuilder();
                        //var log_col = new StringBuilder();
                        //var log_sel = new StringBuilder();

                        //var logTable = Table.GetLogTable();

                        var sql = new StringBuilder();

                        //if (logTable != null)
                        //{
                        //    sql.AppendLine("BEGIN TRAN");
                        //}


                        if (!NewValues.ContainsKey("__changeuser__"))
                            NewValues.Add("__changeuser__", App.UserID);
                        else
                            NewValues["__changeuser__"] = App.UserID;

                        if (!NewValues.ContainsKey("__changetime__"))
                            NewValues.Add("__changetime__", DateTime.Now);

                        sql.AppendLine("BEGIN TRAN");

                        var updateSql = new StringBuilder();
                        bool needUpdate = false;
                        if (NewValues.Values.Count > 0)
                        {
                            updateSql.AppendLine("UPDATE " + Table.Get4PartsTableName() + " SET");

                            foreach (var col in Table.Columns)
                            {
                                if (col.Name != Table.GetPrimaryKeyColumn().Name && NewValues.ContainsKey(col.Name))
                                {
                                    updateSql.Append("  [" + col.Name + "] = ");
                                    needUpdate = true;
                                    if (col.Name == "__changetime__")
                                        updateSql.Append("GetDate()");
                                    else
                                    {
                                        if (NewValues[col.Name] == null)
                                            updateSql.Append("NULL");
                                        else
                                            updateSql.Append(NewValues[col.Name].AsSQL());
                                    }
                                    updateSql.AppendLine(",");

                                    //if (logTable!=null)
                                    //{
                                    //    log_col.Append("[" + col.Name + "],");
                                    //    log_sel.Append("[" + col.Name + "],");
                                    //}
                                }
                            }

                            updateSql.RemoveLastChar(3);
                            updateSql.AppendLine();
                            updateSql.AppendLine("WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL());
                        }

                        if (needUpdate)
                            sql.Append(updateSql);
                        //if (MasterTableRow != null)
                        //{
                        //    var undo_sql = new StringBuilder();
                        //    undo_sql.AppendLine("UPDATE [" + Table.Name + "] SET");

                        //    foreach (var col in Table.Columns)
                        //    {
                        //        if (col.Name != "ID" && NewValues.ContainsKey(col.Name))
                        //        {
                        //            undo_sql.Append("  [" + col.Name + "] = ");
                        //            if (Values[col.Name] == null)
                        //                undo_sql.Append("NULL");
                        //            else
                        //                undo_sql.Append(Values[col.Name].AsSQL());
                        //            undo_sql.AppendLine(",");
                        //        }
                        //    }
                        //    undo_sql.RemoveLastChar(3);
                        //    undo_sql.AppendLine();
                        //    undo_sql.AppendLine("WHERE ID=" + ID.AsSQL());

                        //    sql.AppendLine("INSERT [__EditSession__]([SessionID],[ParentSessionID],[TableID],[RecordID],[UserID],[UndoSql])");
                        //    sql.AppendLine("VALUES (" + EditSessionID.AsSQL() + "," + MasterTableRow.EditSessionID.AsSQL() + "," + Table.ID.AsSQL() + "," +
                        //                                ID.AsSQL() + "," + App.UserID.AsSQL() + "," + undo_sql.AsSQL() + ")");

                        //}
                        //else
                        //{
                        //    foreach (var detailTable in App.Schema.GetAllDetailTablesSamples())
                        //    {
                        //        sql.AppendLine("DELETE FROM [" + detailTable.Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + ID.Reverse().AsSQL());
                        //    }

                        //    sql.Append("EXEC __EditSession__Commit ");
                        //    sql.Append("@sessionID=" + EditSessionID.AsSQL() + ",");
                        //    sql.AppendLine("@parentSessionID=NULL");
                        //}
                        sql.AppendLine("COMMIT");


                        //if (logTable != null)
                        //{
                        //    log_col.Append("["+SchemaTable.logPrefix+""+"]");


                        //    log.AppendLine("INSERT [" + logTable.Name + "](" + log_col.ToString() + ")");
                        //    log.AppendLine("SELECT " + log_sel.ToString() + " FROM [" + Table.Name + "] WHERE ID=" + ID.AsSQL());

                        //    sql.Insert(0, log.ToString());
                        //    sql.AppendLine("COMMIT");
                        //}

                        if (MasterTableRow != null && MasterTableRow.BeforeSaveChanges != null)
                            MasterTableRow.BeforeSaveChanges(this);

                        if (BeforeSaveChanges != null)
                            BeforeSaveChanges(this);

                        using (var db = App.Schema.SqlDB.GetDbManager())
                        {
                            int updatedRows = db.SetCommand(sql.ToString()).ExecuteNonQuery();
                            if (updatedRows == 0)
                                throw new Exception("Ошибка изменения записи в таблице '" + Table.Name + "', ID='" + ID + "'");
                        }

                        if (BeforeSaveChanges != null)
                            BeforeSaveChanges(this);
                        foreach (var columnName in NewValues.Keys)
                        {
                            if (Values.ContainsKey(columnName))
                                Values[columnName] = NewValues[columnName];
                            else
                                Values.Add(columnName, NewValues[columnName]);
                        }
                        NewValues.Clear();
                    }

            if (Table.GetIsProvodkaGenerationExists())
                Table.ExecGenStoredProc(ID);

            if (OnSaveData != null)
                OnSaveData(this, null);

        }


        public void CancelAddChanges()
        {
            CheckParams();
            if (CrudMode == CrudMode.Add || CrudMode == Buhta.CrudMode.Edit)
            {
                var sql = new StringBuilder();
                sql.AppendLine("BEGIN TRAN");
                sql.AppendLine("DECLARE @id sql_variant");
                sql.AppendLine("SET @id=" + ID.AsSQL());

                Table.EmitCancelAddChangesSql(sql, "@id", MasterTableRow == null ? null : MasterTableRow.Table, "");

                sql.AppendLine("COMMIT");
                using (var db = App.Schema.SqlDB.GetDbManager())
                {
                    db.SetCommand(sql.ToString()).ExecuteNonQuery();
                }
                NewValues.Clear();
            }
            if (OnCancelChanges != null)
                OnCancelChanges(this, null);

        }

        public void DeleteRecord()
        {
            CheckParams();
            if (CrudMode == CrudMode.View)
            {
                throw new Exception("Запись только на чтение в таблице '" + Table.Name + "', ID='" + ID + "'");
            }
            else
            {
                if (MasterTableRow != null && MasterTableRow.BeforeSaveChanges != null)
                    MasterTableRow.BeforeSaveChanges(this);

                if (BeforeSaveChanges != null)
                    BeforeSaveChanges(this);

                if (MasterTableRow == null || Table.Details.Count > 0)
                    SaveCopyToLog();

                var sql = new StringBuilder();
                sql.AppendLine("BEGIN TRAN");
                sql.AppendLine("DECLARE @id sql_variant");
                sql.AppendLine("SET @id=" + ID.AsSQL());

                Table.EmitCancelAddChangesSql(sql, "@id", MasterTableRow == null ? null : MasterTableRow.Table, "");

                sql.AppendLine("COMMIT");


                using (var db = App.Schema.SqlDB.GetDbManager())
                {
                    db.SetCommand(sql.ToString()).ExecuteNonQuery();
                }

            }

            if (OnSaveData != null)
                OnSaveData(this, null);

        }

        public bool LockRecord()
        {
            if (MasterTableRow == null)
            {
                var sql = new StringBuilder();

                sql.AppendLine("UPDATE [" + Table.Name + "] SET ");
                sql.AppendLine("  __lockuser__=" + App.UserID.AsSQL() + ",");
                sql.AppendLine("  __locktime__=GetDate()");
                sql.AppendLine("WHERE ["+Table.GetPrimaryKeyColumn().Name+"]=" + ID.AsSQL() + " AND __lockuser__ IS NULL");

                sql.AppendLine("SELECT __lockuser__ FROM " + Table.Get4PartsTableName() + " WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL());

                using (var db = App.Schema.SqlDB.GetDbManager())
                {
                    var result = db.SetCommand(sql.ToString()).ExecuteScalar<Guid>();
                    return result == App.UserID;
                }
            }
            else
                return true;
        }

        public void UnlockRecord()
        {
            if (MasterTableRow == null)
            {
                var sql = new StringBuilder();

                sql.AppendLine("UPDATE " + Table.Get4PartsTableName() + " SET ");
                sql.AppendLine("  __lockuser__=NULL,");
                sql.AppendLine("  __locktime__=NULL");
                sql.AppendLine("WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL() + " AND __lockuser__ = " + App.UserID.AsSQL());

                using (var db = App.Schema.SqlDB.GetDbManager())
                {
                    db.SetCommand(sql.ToString()).ExecuteScalar();
                }
            }
        }

        public void SaveCopyToLog()
        {
            var sql = new StringBuilder();
            sql.AppendLine("BEGIN TRAN");
            sql.AppendLine("DECLARE @id sql_variant");
            sql.AppendLine("SET @id=" + ID.AsSQL());

            //            if (Table.Details.Count > 0)
            {
                sql.AppendLine("DECLARE @master_timestamp binary(8)");
                sql.AppendLine("SELECT @master_timestamp=__timestamp__ FROM " + Table.Get4PartsTableName() + " WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL());
            }

            Table.EmitSaveToLogSql(sql, "@id", MasterTableRow == null ? null : MasterTableRow.Table, "");

            sql.AppendLine("COMMIT");
            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }
        }

        // восстановление последней записи из лога
        public void CancelEditChanges()
        {
            CheckParams();

            var sql = new StringBuilder();
            sql.AppendLine("BEGIN TRAN");
            sql.AppendLine("DECLARE @id sql_variant");
            sql.AppendLine("SET @id=" + ID.AsSQL());

            //            if (Table.Details.Count > 0)
            {
                sql.AppendLine("DECLARE @master_timestamp binary(8)");
                sql.AppendLine("SELECT @master_timestamp=MAX(__timestamp__) FROM " + Table.GetLogTable().Get4PartsTableName() + " WHERE [" + Table.GetPrimaryKeyColumn().Name + "]=" + ID.AsSQL());
            }

            // ----------------- удаление нового
            Table.EmitCancelAddChangesSql(sql, "@id", MasterTableRow == null ? null : MasterTableRow.Table, "");

            // ----------------- восстановление старого
            Table.EmitRestoreFromLogSql(sql, "@id", MasterTableRow == null ? null : MasterTableRow.Table, "");

            sql.AppendLine("COMMIT");
            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

            if (OnCancelChanges != null)
                OnCancelChanges(this, null);

        }

    }
}
