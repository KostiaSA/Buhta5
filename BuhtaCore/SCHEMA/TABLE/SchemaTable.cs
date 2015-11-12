using DevExpress.XtraTab;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    //public enum SchemaTableRole_old { None = 0, Справочник = 1, Документ = 2, Деталь = 3, Регистр = 4 }
    [Export(typeof(SchemaObject))]
    public partial class SchemaTable : SchemaObject, ISupportInitialize, IViewColumn, IPermissionSupportObject
    {
        [JsonIgnore]
        public bool IsLogTable;

        [JsonIgnore]
        public bool IsProvodkasTable;

        //private SchemaTableRole_old role;
        //public SchemaTableRole_old Role
        //{
        //    get { return role; }
        //    set { role = value; firePropertyChanged("Role"); }
        //}

        private bool isNotSyncronize;
        public bool IsNotSyncronize
        {
            get { return isNotSyncronize; }
            set { isNotSyncronize = value; firePropertyChanged("IsNotSyncronize"); }
        }

        private Guid? externalDatabase;
        public Guid? ExternalDatabase
        {
            get { return externalDatabase; }
            set { externalDatabase = value; firePropertyChanged("ExternalDatabase"); }
        }

        private Guid? defaultQueryID;
        public Guid? DefaultQueryID
        {
            get { return defaultQueryID; }
            set { defaultQueryID = value; firePropertyChanged("DefaultQueryID"); }
        }

        private Guid? lookupQueryID;
        public Guid? LookupQueryID
        {
            get { return lookupQueryID; }
            set { lookupQueryID = value; firePropertyChanged("LookupQueryID"); }
        }

        private bool isSubconto;
        public bool IsSubconto
        {
            get { return isSubconto; }
            set { isSubconto = value; firePropertyChanged("IsSubconto"); }
        }

        private string subcontoName;
        public string SubcontoName
        {
            get { return subcontoName; }
            set { subcontoName = value; firePropertyChanged("SubcontoName"); }
        }

        private string formLayout;
        public string FormLayout
        {
            get { return formLayout; }
            set { formLayout = value; firePropertyChanged("FormLayout"); }
        }

        private bool isProvodkaOwner;
        public bool IsProvodkaOwner // может иметь движения по регистрам
        {
            get { return isProvodkaOwner; }
            set { isProvodkaOwner = value; firePropertyChanged("IsProvodkaOwner"); }
        }

        private bool isUserEditable;
        public bool IsUserEditable // может редактироваться с созданием лога
        {
            get { return isUserEditable; }
            set { isUserEditable = value; firePropertyChanged("IsUserEditable"); }
        }

        public ObservableCollection<Guid> TableRoles { get; private set; }

        public ObservableCollection<SchemaTableColumn> Columns { get; private set; }

        public ObservableCollection<SchemaTableDetail> Details { get; private set; }

        public ObservableCollection<SchemaTableLinkedReport> LinkedReports { get; private set; }

        public void CreateInternalColumns()
        {
            SchemaTableColumn c;

            // автодобавление пока отключено
            //foreach (var tableRoleID in TableRoles)
            //{
            //    foreach (var roleCol in (SchemaBaseRole.Roles[tableRoleID] as Таблица_TableRole).Columns)
            //    {
            //        if (roleCol.IsRequiredColumn && GetColumnByRole(roleCol.ID) == null)
            //        {
            //            c = new SchemaTableColumn();
            //            c.Name = roleCol.NewColumnName;
            //            c.DataType = roleCol.DataType.Clone();
            //            c.DataType.Column = c;
            //            c.IsNotNullable = roleCol.IsNotNullable;
            //            c.Table = this;
            //            c.ColumnRoles.Add(roleCol.ID);
            //            Columns.Add(c);
            //        }
            //    }
            //}
            if (IsUserEditable)
            {
                if (!TableRoles.Contains(RoleConst.ВложеннаяТаблица))
                {
                    if (GetColumnByName("__lockuser__") == null)
                    {
                        c = new SchemaTableSystemColumn();
                        c.Position = Columns.Count;
                        c.Name = "__lockuser__";
                        c.DataType = new GuidDataType() { Column = c };
                        c.IsNotNullable = false;
                        c.Table = this;
                        Columns.Add(c);
                    }

                    if (GetColumnByName("__locksession__") == null)
                    {
                        c = new SchemaTableSystemColumn();
                        c.Position = Columns.Count;
                        c.Name = "__locksession__";
                        c.DataType = new GuidDataType() { Column = c };
                        c.IsNotNullable = false;
                        c.Table = this;
                        Columns.Add(c);
                    }

                    if (GetColumnByName("__locktime__") == null)
                    {
                        c = new SchemaTableSystemColumn();
                        c.Position = Columns.Count;
                        c.Name = "__locktime__";
                        c.DataType = new DateTimeDataType() { Column = c };
                        c.IsNotNullable = false;
                        c.Table = this;
                        Columns.Add(c);
                    }

                    if (GetColumnByName("__changeuser__") == null)
                    {
                        c = new SchemaTableSystemColumn();
                        c.Position = Columns.Count;
                        c.Name = "__changeuser__";
                        c.DataType = new GuidDataType() { Column = c };
                        c.IsNotNullable = false;
                        c.Table = this;
                        Columns.Add(c);
                    }

                    if (GetColumnByName("__changetime__") == null)
                    {
                        c = new SchemaTableSystemColumn();
                        c.Position = Columns.Count;
                        c.Name = "__changetime__";
                        c.DataType = new DateTimeDataType() { Column = c };
                        c.IsNotNullable = false;
                        c.Table = this;
                        Columns.Add(c);
                    }
                }

                if (GetColumnByName("__timestamp__") == null)
                {
                    c = new SchemaTableSystemColumn();
                    c.Position = Columns.Count;
                    c.Name = "__timestamp__";
                    c.DataType = new TimestampDataType() { Column = c };
                    c.IsNotNullable = false;
                    c.Table = this;
                    Columns.Add(c);
                }
            }
        }

        public SchemaTable()
        {
            Columns = new ObservableCollection<SchemaTableColumn>();
            Columns.CollectionChanged += Columns_CollectionChanged;

            Details = new ObservableCollection<SchemaTableDetail>();
            Details.CollectionChanged += Details_CollectionChanged;

            LinkedReports = new ObservableCollection<SchemaTableLinkedReport>();
            LinkedReports.CollectionChanged += LinkedReports_CollectionChanged;

            TableRoles = new ObservableCollection<Guid>();
            TableRoles.CollectionChanged += TableRoles_CollectionChanged;

        }

        public override void Validate(StringBuilder error)
        {
            base.Validate(error);

            if (Name != null && Name.Length > 128)
                error.AppendLine("Имя таблицы длинее 128 символов.");

            if (TableRoles.Count == 0)
                error.AppendLine("Не указана ни одна роль таблицы.");

            foreach (var helperTable in App.Schema.HelperTables)
            {
                if (Name != null && Name.Equals(helperTable.Name, StringComparison.OrdinalIgnoreCase))
                    error.AppendLine("Таблица не может называться " + Name.AsSQL() + ", есть такая системная таблица.");
            }

            if (Name != null && Name.StartsWith(logPrefix, StringComparison.OrdinalIgnoreCase))
                error.AppendLine("Имя таблицы не может начинаться с '" + logPrefix + "'");


            // уникальность имен колонок
            var uniqueList = new HashSet<string>();
            foreach (var tableCol in Columns)
            {
                if (uniqueList.Contains(tableCol.Name))
                    error.AppendLine("Есть несколько колонок с именем " + tableCol.Name.AsSQL());
                else
                    uniqueList.Add(tableCol.Name);
            }

            // валидация колонок
            foreach (var tableCol in Columns)
                tableCol.Validate(error);

            // валидация деталей
            foreach (var tableDetail in Details)
                tableDetail.Validate(error);

            // валидация привязнных печатных форм
            foreach (var linkedReport in LinkedReports)
                linkedReport.Validate(error);

            foreach (var tableRoleID in TableRoles)
            {
                if (!SchemaBaseRole.Roles.ContainsKey(tableRoleID))
                {
                    error.AppendLine("У таблицы указана неверная роль: " + tableRoleID.AsSQL());
                }
                else
                {
                    var role = SchemaBaseRole.Roles[tableRoleID];
                    if (role is Таблица_TableRole)
                    {
                        var tableRole = role as Таблица_TableRole;

                        // проверка наличия обязательных ролевых полей
                        foreach (var roleCol in tableRole.Columns.Where(col => col.IsRequiredColumn))
                        {
                            foreach (var tableCol in Columns)
                            {
                                if (tableCol.ColumnRoles.Contains(roleCol.ID))
                                    goto m1;
                            }
                            error.AppendLine("Отсутствует обязательная колонка с ролью: " + roleCol.Name.AsSQL());
                        m1: ;
                        }

                        // проверка наличия обязательных ролевых полей
                        foreach (var roleCol in tableRole.Columns.Where(col => !col.IsMultiColumn))
                        {
                            int count = 0;
                            foreach (var tableCol in Columns)
                            {
                                if (tableCol.ColumnRoles.Contains(roleCol.ID))
                                    count++;
                            }
                            if (count > 1)
                                error.AppendLine("Есть несколько колонок с уникальной ролью: " + roleCol.Name.AsSQL());
                        }
                    }
                    else
                    {
                        error.AppendLine("У таблицы указана неверная роль: " + role.Name.AsSQL());
                    }
                }
            }
        }

        void TableRoles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("TableRoles");
        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var column in Columns)
                if (column.Table == null)
                    column.Table = this;
            firePropertyChanged("Columns");
        }

        void Details_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var detail in Details)
                if (detail.Table == null)
                    detail.Table = this;
            firePropertyChanged("Details");
        }

        void LinkedReports_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var reports in LinkedReports)
                if (reports.Table == null)
                    reports.Table = this;
            firePropertyChanged("LinkedReports");
        }

        public override void firePropertyChanged(string propertyName)
        {
            columnsByName.Clear();
            logTable = null;
            cached_PrimaryKeyColumn = null;
            base.firePropertyChanged(propertyName);
        }


        public override BaseEdit_Page GetEditForm_page()
        {
            return new SchemaTableDesigner_page() { EditedRecord = this };
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Таблица";
            }
        }

        public override void PrepareNew()
        {
            if (TableRoles.Count == 0)
                TableRoles.Add(RoleConst.Таблица);  // таблица

        }


        public SchemaTableColumn GetColumnByRole(Guid? roleID)
        {
            if (roleID == null)
                return null;

            foreach (var column in Columns)
            {
                if (column.ColumnRoles.Contains((Guid)roleID))
                    return column;
            };

            return null;
        }


        public SchemaTableColumn GetMasterColumn()
        {
            return GetColumnByRole(RoleConst.ВложеннаяТаблица_Мастер);
        }

        public bool IsColumnExists(string columnName)
        {
            return GetColumnByName(columnName) != null;
        }

        Dictionary<string, SchemaTableColumn> columnsByName = new Dictionary<string, SchemaTableColumn>(StringComparer.OrdinalIgnoreCase);
        public SchemaTableColumn GetColumnByName(string columnName)
        {
            if (columnsByName.Values.Count == 0)
            {
                foreach (var column in Columns)
                    columnsByName.Add(column.Name, column);
            }

            SchemaTableColumn retColumn;
            if (columnsByName.TryGetValue(columnName, out retColumn))
                return retColumn;
            else
                return null;
        }

        void ISupportInitialize.BeginInit()
        {

        }

        public override void EndInit()
        {
        }

        public IViewColumn GetParentViewColumn()
        {
            return null;
        }

        public IViewColumn GetSourceView()
        {
            return this;
        }

        public IViewColumn GetSourceViewColumn()
        {
            return null;
        }


        public IViewColumn GetJoinView()
        {
            return this;
        }


        IViewColumn IViewColumn.GetColumnByName(string name)
        {
            return GetColumnByName(name);
        }

        [JsonIgnore]
        [Browsable(false)]
        public override string DisplayName
        {
            get
            {
                return GetDisplayName();
            }
        }

        public override string GetSchemaDesignerDisplayName()
        {
            return GetDisplayName();
        }

        public string GetDisplayName()
        {
            var extDb = App.Schema.GetSampleObject<SchemaExternalDatabase>(ExternalDatabase);
            if (extDb != null)
                return extDb.Name + "." + Name;
            else
                return Name;
        }


        public List<IViewColumn> GetColumns()
        {
            return Columns.ToList<IViewColumn>();
        }


        public SchemaTable GetNativeTable()
        {
            return this;
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
            return null;
        }

        public string GetDisplayNameAndDataType()
        {
            throw new NotImplementedException();
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


        public string GetFullAlias()
        {
            throw new NotImplementedException();
        }

        public TableRecordBaseEditForm_page OpenRecordEditPage(object recordID, CrudMode mode, SchemaTableRow masterTableRow)
        {
            if (!IsUserEditable)
                throw new Exception("Таблица " + Name + " не может редактироваться (!IsUserEditable) ");

            XtraTabPage tabPage;
            tabPage = new XtraTabPage();

            Guid? newBussinesOperID = null;

            if (masterTableRow != null)
            {
                foreach (var detail in masterTableRow.Table.Details)
                {
                    if (detail.DetailTableID == ID)
                    {
                        if (detail.Opers.Count > 0)
                        {
                            if (mode == CrudMode.Add)
                            {
                                if (detail.Opers.Count == 1)
                                    newBussinesOperID = detail.Opers[0].ID;
                                else
                                    MessageBox.Show("Выбор бизнес-операции");
                            }
                            else
                            {
                                MessageBox.Show("Да-редактирование");
                            }
                        }
                    }
                }
            }



            var page = new TableRecordEditForm_page();
            page.Dock = DockStyle.Fill;
            page.NewBussinesOperID = newBussinesOperID;
            page.MasterTableRow = masterTableRow;
            page.Table = this;
            page.RecordID = recordID;
            page.CrudMode = mode;

            page.AfterClose += new Base_page.AfterCloseEventHandler(sender_page =>
            {
                //tabPages.Remove(focusedElement.ID);
                //App.MainTabControl.SelectedTabPage = this.Parent as XtraTabPage;
                //treeList.RefreshDataSource();
                //treeList.SetFocusedNode(editedNode);
            });

            tabPage.Controls.Add(page);
            tabPage.Text = "Запись";
            App.MainTabControl.TabPages.Add(tabPage);
            page.LoadData();
            //tabPages.Add(focusedElement.ID, tabPage);

            App.MainTabControl.SelectedTabPage = tabPage;
            return page;
        }


        public SchemaTable GetRootNativeTable()
        {
            return this;
        }


        public IViewColumn GetRootColumn()
        {
            return null;
        }



        SchemaTable logTable;
        public const string logPrefix = "__log__";
        public SchemaTable GetLogTable()
        {
            if (IsLogTable || IsProvodkasTable)
                throw new Exception("internal error 98AB23A5");

            if (logTable == null)
            {
                logTable = App.Schema.GetObject<SchemaTable>(ID);

                logTable.ID = Guid.NewGuid();
                logTable.IsLogTable = true;
                logTable.Name = logPrefix + Name;

                var toDelete = new List<SchemaTableColumn>();
                foreach (var col in logTable.Columns)
                {
                    if (col.Name == "__lockuser__" || col.Name == "__locksession__" || col.Name == "__locktime__")
                        toDelete.Add(col);

                    if (col.Name == "__timestamp__")
                        col.DataType = new BinaryDataType() { MaxLength = 8 };

                }

                toDelete.ForEach(col => logTable.Columns.Remove(col));

                if (logTable.TableRoles.Contains(RoleConst.ВложеннаяТаблица))
                    if (GetColumnByName("__mastertimestamp__") == null)
                    {
                        var c = new SchemaTableColumn();
                        c.Position = Columns.Count;
                        c.Name = "__mastertimestamp__";
                        c.DataType = new BinaryDataType() { MaxLength = 8 };
                        c.IsNotNullable = false;
                        c.Table = logTable;
                        logTable.Columns.Add(c);
                    }


            }
            return logTable;
        }

        SchemaTable provodkasTable;
        public const string provodkasPrefix = "__проводки__";
        public SchemaTable GetProvodkasTable()
        {
            if (IsLogTable || IsProvodkasTable)
                throw new Exception("internal error 23A598AB");

            //if (!TableRoles.Contains(RoleConst.ВложеннаяТаблица))
            //    throw new Exception("internal error 86D606F2");

            if (provodkasTable == null)
            {
                provodkasTable = new SchemaTable();

                provodkasTable.ID = Guid.NewGuid();
                provodkasTable.IsProvodkasTable = true;
                provodkasTable.IsLogTable = false;
                provodkasTable.Name = provodkasPrefix + Name;

                var c = new SchemaTableColumn();
                c.Name = "tableRecordID";
                c.DataType = new GuidDataType() { Column = c };
                c.IsNotNullable = true;
                c.Table = provodkasTable;
                provodkasTable.Columns.Add(c);

                c = new SchemaTableColumn();
                c.Name = "provodkaRecordID";
                c.DataType = new GuidDataType() { Column = c };
                c.IsNotNullable = true;
                c.Table = provodkasTable;
                provodkasTable.Columns.Add(c);

                c = new SchemaTableColumn();
                c.Name = "registrID";
                c.DataType = new GuidDataType() { Column = c };
                c.IsNotNullable = true;
                c.Table = provodkasTable;
                provodkasTable.Columns.Add(c);
            }
            return provodkasTable;
        }

        public override Bitmap GetImage()
        {
            if (TableRoles.Contains(RoleConst.Регистр))
                return global::Buhta.Properties.Resources.RegistrRole;
            else
                return global::Buhta.Properties.Resources.SchemaTable_16;
        }

        public void EmitSaveToLogSql(StringBuilder sql, string id_variable, SchemaTable masterTable, string indent)
        {
            sql.Append(indent + "INSERT [" + GetLogTable().Name + "] (");

            foreach (var col in GetLogTable().Columns)
            {
                if (col.Name == "__lockuser__" || col.Name == "__locksession__" || col.Name == "__locktime__") continue;

                sql.Append("  [" + col.Name + "],");
            }

            sql.RemoveLastChar(1);
            sql.AppendLine(")");
            sql.Append(indent + "SELECT ");
            foreach (var col in GetLogTable().Columns)
            {
                if (col.Name == "__lockuser__" || col.Name == "__locksession__" || col.Name == "__locktime__") continue;
                if (col.Name == "__mastertimestamp__")
                    sql.Append("  @master_timestamp,");
                //sql.Append("  ( SELECT __timestamp__  FROM [" + masterTable.Name + "] WHERE ID=[" + Name + "].[" + GetMasterColumn().Name + "]),");
                else
                    sql.Append("  [" + col.Name + "],");
            }
            sql.RemoveLastChar(1);
            sql.AppendLine(" FROM [" + Name + "] WHERE ID=" + id_variable);

            int detNum = App.Random.Next(1, int.MaxValue / 2);
            foreach (var detail in Details)
            {
                detNum++;
                var detailTable = detail.GetDetailTable();
                sql.AppendLine(indent + "DECLARE @detid" + detNum + " uniqueidentifier");
                sql.AppendLine(indent + "DECLARE curs" + detNum + " CURSOR LOCAL STATIC FOR");
                sql.AppendLine(indent + "SELECT ID FROM [" + detailTable.Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + id_variable);
                sql.AppendLine(indent + "OPEN curs" + detNum);
                sql.AppendLine(indent + "FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "WHILE @@FETCH_STATUS = 0");
                sql.AppendLine(indent + "BEGIN");
                detailTable.EmitSaveToLogSql(sql, "@detid" + detNum, this, indent + "  ");
                sql.AppendLine(indent + "  FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "END");
                sql.AppendLine(indent + "CLOSE curs" + detNum);
                sql.AppendLine(indent + "DEALLOCATE curs" + detNum);

            }

        }

        public void EmitCancelAddChangesSql(StringBuilder sql, string id_variable, SchemaTable masterTable, string indent)
        {

            int detNum = App.Random.Next(1, int.MaxValue / 2);
            foreach (var detail in Details)
            {
                detNum++;
                var detailTable = detail.GetDetailTable();
                sql.AppendLine(indent + "DECLARE @detid" + detNum + " uniqueidentifier");
                sql.AppendLine(indent + "DECLARE curs" + detNum + " CURSOR LOCAL STATIC FOR");
                sql.AppendLine(indent + "SELECT ID FROM [" + detailTable.Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + id_variable);
                sql.AppendLine(indent + "OPEN curs" + detNum);
                sql.AppendLine(indent + "FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "WHILE @@FETCH_STATUS = 0");
                sql.AppendLine(indent + "BEGIN");
                detailTable.EmitCancelAddChangesSql(sql, "@detid" + detNum, this, indent + "  ");
                sql.AppendLine(indent + "  FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "END");
                sql.AppendLine(indent + "CLOSE curs" + detNum);
                sql.AppendLine(indent + "DEALLOCATE curs" + detNum);

            }
            sql.AppendLine(indent + "DELETE /*1*/ FROM [" + Name + "] WHERE ID=" + id_variable);

        }


        //public void EmitDeleteBeforeRestoreFromLogSql(StringBuilder sql, string id_variable, SchemaTable masterTable, string indent)
        //{

        //    int detNum = App.Random.Next(1, int.MaxValue / 2);
        //    foreach (var detail in Details)
        //    {
        //        detNum++;
        //        var detailTable = detail.GetDetailTable();
        //        sql.AppendLine(indent + "DECLARE @detid" + detNum + " uniqueidentifier");
        //        sql.AppendLine(indent + "DECLARE curs" + detNum + " CURSOR LOCAL STATIC FOR");
        //        sql.AppendLine(indent + "SELECT ID FROM [" + detailTable.Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + id_variable);
        //        sql.AppendLine(indent + "OPEN curs" + detNum);
        //        sql.AppendLine(indent + "FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
        //        sql.AppendLine(indent + "WHILE @@FETCH_STATUS = 0");
        //        sql.AppendLine(indent + "BEGIN");
        //        detailTable.EmitCancelAddChangesSql(sql, "@detid" + detNum, this, indent + "  ");
        //        sql.AppendLine(indent + "  FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
        //        sql.AppendLine(indent + "END");
        //        sql.AppendLine(indent + "CLOSE curs" + detNum);
        //        sql.AppendLine(indent + "DEALLOCATE curs" + detNum);

        //    }
        //    if (masterTable != null)
        //        sql.AppendLine(indent + "DELETE FROM [" + Name + "] WHERE ID=" + id_variable);
        //    else
        //        sql.AppendLine(indent + "DELETE FROM [" + Name + "] WHERE ID=" + id_variable);

        //}

        public void EmitRestoreFromLogSql(StringBuilder sql, string id_variable, SchemaTable masterTable, string indent)
        {


            int detNum = App.Random.Next(1, int.MaxValue / 2);
            foreach (var detail in Details)
            {
                detNum++;
                var detailTable = detail.GetDetailTable();
                sql.AppendLine(indent + "DECLARE @detid" + detNum + " uniqueidentifier");
                sql.AppendLine(indent + "DECLARE curs" + detNum + " CURSOR LOCAL STATIC FOR");
                sql.AppendLine(indent + "SELECT ID FROM [" + detailTable.GetLogTable().Name + "] WHERE [" + detailTable.GetMasterColumn().Name + "]=" + id_variable);
                sql.AppendLine(indent + "OPEN curs" + detNum);
                sql.AppendLine(indent + "FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "WHILE @@FETCH_STATUS = 0");
                sql.AppendLine(indent + "BEGIN");
                detailTable.EmitRestoreFromLogSql(sql, "@detid" + detNum, this, indent + "  ");
                sql.AppendLine(indent + "  FETCH NEXT FROM curs" + detNum + " INTO @detid" + detNum);
                sql.AppendLine(indent + "END");
                sql.AppendLine(indent + "CLOSE curs" + detNum);
                sql.AppendLine(indent + "DEALLOCATE curs" + detNum);

            }

            //sql.AppendLine(indent + "DELETE FROM [" + Name + "] WHERE ID=" + id_variable);
            sql.Append(indent + "INSERT [" + Name + "] (");

            foreach (var col in GetLogTable().Columns)
            {
                if (col.Name == "__lockuser__" || col.Name == "__locksession__" || col.Name == "__locktime__" || col.Name == "__timestamp__" || col.Name == "__mastertimestamp__") continue;

                sql.Append("  [" + col.Name + "],");
            }

            sql.RemoveLastChar(1);
            sql.AppendLine(")");
            sql.Append(indent + "SELECT ");
            foreach (var col in GetLogTable().Columns)
            {
                if (col.Name == "__lockuser__" || col.Name == "__locksession__" || col.Name == "__locktime__" || col.Name == "__timestamp__" || col.Name == "__mastertimestamp__") continue;
                else
                    sql.Append("  [" + col.Name + "],");
            }
            sql.RemoveLastChar(1);
            if (masterTable != null)
            {
                sql.AppendLine(" FROM [" + GetLogTable().Name + "] WHERE ID=" + id_variable + " AND __mastertimestamp__=@master_timestamp");
                sql.AppendLine(indent + "DELETE  /*3*/ FROM [" + GetLogTable().Name + "] WHERE ID=" + id_variable + " AND __mastertimestamp__=@master_timestamp");
            }
            else
            {
                sql.AppendLine(" FROM [" + GetLogTable().Name + "] WHERE ID=" + id_variable + " AND __timestamp__=@master_timestamp");
                sql.AppendLine(indent + "DELETE  /*3*/ FROM [" + GetLogTable().Name + "] WHERE ID=" + id_variable + " AND __timestamp__=@master_timestamp");
            }
        }

        SchemaTableColumn cached_PrimaryKeyColumn;
        public SchemaTableColumn GetPrimaryKeyColumn()
        {
            if (cached_PrimaryKeyColumn == null)
            {
                foreach (var col in Columns)
                {
                    if (col.ColumnRoles.Contains(RoleConst.Таблица_Ключ))
                    {
                        cached_PrimaryKeyColumn = col;
                        break;
                    }
                }
            }
            return cached_PrimaryKeyColumn;
        }


        public string Get4PartsTableName()
        {
            var sb = new StringBuilder();
            if (ExternalDatabase != null)
            {
                var db = App.Schema.GetSampleObject<SchemaExternalDatabase>(ExternalDatabase);
                if (db != null)
                {
                    if (!string.IsNullOrWhiteSpace(db.LinkedServer))
                    {
                        sb.Append("[" + db.LinkedServer + "].[" + db.LinkedDatabase + "].[" + db.LinkedSchema + "].");
                    }
                    else
                        sb.Append("[" + db.LinkedDatabase + "].[" + db.LinkedSchema + "].");

                }
                else
                    sb.Append("[ExternalDatabase?" + ExternalDatabase + "].");
            }
            sb.Append("[" + Name + "]");

            return sb.ToString();
        }

        public bool GetIsSupportReadPermission()
        {
            return true;
        }

        public bool GetIsSupportInsertPermission()
        {
            return true;
        }

        public bool GetIsSupportUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportDeletePermission()
        {
            return true;
        }

        public bool GetIsSupportOwnedUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportOwnedDeletePermission()
        {
            return true;
        }

        public virtual string GetPermissionFolder()
        {
            if (TableRoles.Contains(RoleConst.Регистр))
                return "Регистр";
            else
                return "Таблица";
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
            foreach (var col in Columns)
                if (col.ColumnRoles.Count > 0)
                    list.Add(col);
        }

        public void ImportSchemaFromNativeDB(string tableName)
        {
            if (Columns.Count != 0)
                throw new Exception("Для импорта нужна пустая таблица.");

            TableRoles.Add(RoleConst.Таблица);

            using (var db = App.Schema.SqlDB.GetDbManager())
            {
                db.SetCommand(
@"
SELECT *,  
CASE 
  WHEN (SELECT count(*) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE K 
        WHERE K.COLUMN_NAME=C.COLUMN_NAME AND 
		      K.TABLE_NAME=" + tableName.AsSQL() + @" AND 
			  SUBSTRING(K.CONSTRAINT_NAME,1,4)='PK__')=1 THEN 1 
  ELSE 0 
  END IsKey
FROM INFORMATION_SCHEMA.COLUMNS C WHERE TABLE_NAME=" + tableName.AsSQL() + " order by ORDINAL_POSITION");

                var columns = db.ExecuteDataTable();

                foreach (DataRow row in columns.Rows)
                {
                    var c = new SchemaTableColumn();
                    c.Position = (int)row["ORDINAL_POSITION"];
                    c.Name = row["COLUMN_NAME"].ToString();
                    c.ColumnRoles.Add(RoleConst.Таблица_Колонка);
                    if ((int)row["IsKey"] == 1)
                        c.ColumnRoles.Add(RoleConst.Таблица_Ключ);
                    /*
                    bit
                    datetime
                    decimal
                    int
                    money
                    nvarchar
                    uniqueidentifier
                    varbinary
                     */
                    var dataType = row["DATA_TYPE"].ToString().ToLower();

                    if (dataType == "int")
                    {
                        c.DataType = new IntDataType() { Column = c };
                    }
                    else
                        if (dataType == "bit")
                        {
                            c.DataType = new BitDataType() { Column = c };
                        }
                        else
                            if (dataType == "datetime")
                            {
                                c.DataType = new DateTimeDataType() { Column = c };
                            }
                            else
                                if (dataType == "money")
                                {
                                    c.DataType = new MoneyDataType() { Column = c };
                                }
                                else
                                    if (dataType == "uniqueidentifier")
                                    {
                                        c.DataType = new GuidDataType() { Column = c };
                                    }
                                    else
                                        if (dataType == "decimal")
                                    {
                                        c.DataType = new DecimalDataType() { Column = c, Scale = (int)row["NUMERIC_SCALE"], Precision = (byte)row["NUMERIC_PRECISION"] };
                                    }
                                    else
                                        if (dataType == "nvarchar" || dataType == "varchar" || dataType == "char" || dataType == "nchar")
                                        {
                                            c.DataType = new StringDataType() { MaxSize = (int)row["CHARACTER_MAXIMUM_LENGTH"] == -1 ? 0 : (int)row["CHARACTER_MAXIMUM_LENGTH"], Column = c };
                                        }
                                        else
                                            if (dataType == "binary" || dataType == "varbinary")
                                            {
                                                c.DataType = new StringDataType() { MaxSize = (int)row["CHARACTER_MAXIMUM_LENGTH"] == -1 ? 0 : (int)row["CHARACTER_MAXIMUM_LENGTH"], Column = c };
                                            }
                                            else
                                                throw new Exception("Неизвестный тип данных " + dataType);


                    c.IsNotNullable = row["IS_NULLABLE"].ToString() == "NO";
                    c.Table = this;
                    Columns.Add(c);

                }
            }
        }

    }
}


