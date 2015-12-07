using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public enum RegistrColumnRole { Реквизит, Измерение, Ресурс }

    [JsonObject(IsReference = true)]
    [Serializable]
    public class SchemaTableColumn : IBuhtaEditable, ISupportInitialize, INotifyPropertyChanged, IViewColumn, IPermissionSupportObject
    {
        [JsonIgnore]
        bool needSave;

        private string name;
        [DisplayName("  Имя колонки"), Description("Имя колонки"), Category("  Колонка")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

////        [Editor(typeof(TableColumnRolesSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(TableColumnRolesSelectorTypeConverter))]
        [DisplayName(" Роль"), Description("Роли"), Category("  Колонка")]
        public ObservableCollection<Guid> ColumnRoles { get; private set; }

        private string description;
        [DisplayName("Описание"), Description("Описание"), Category("  Колонка")]
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("ID"); }
        }

        private SqlDataType dataType;
        [DisplayName("Тип данных"), Description("Тип данных"), Category(" Тип данных")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SqlDataType DataType
        {
            get { return dataType; }
            set { dataType = value; firePropertyChanged("DataType"); }
        }

        private RegistrColumnRole registrRole;
        [DisplayName("Тип поля"), Description("Тип поля"), Category("Регистр")]
        public RegistrColumnRole RegistrRole
        {
            get { return registrRole; }
            set { registrRole = value; firePropertyChanged("RegistrRole"); }
        }


        private bool isNotNullable;
        [DisplayName("Обязательное"), Description("Поле может быть заполнено (not null)"), Category("  Колонка")]
        public bool IsNotNullable
        {
            get { return isNotNullable; }
            set { isNotNullable = value; firePropertyChanged("IsNotNullable"); }
        }

        private bool isHidden;
        [DisplayName("Скрытое"), Description("Поле не показывается в форме редактирования"), Category("Форма редактирования")]
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; firePropertyChanged("IsHidden"); }
        }

        private bool isReadOnly;
        [DisplayName("Только-чтение"), Description("Поле не редактируется в формах."), Category("Форма редактирования")]
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; firePropertyChanged("IsReadOnly"); }
        }

        private int position;
        public int Position
        {
            get { return position; }
            set { position = value; firePropertyChanged("Position"); }
        }

        [NonSerialized]
        private SchemaTable table;
        public SchemaTable Table
        {
            get { return table; }
            set { table = value; firePropertyChanged("Table"); }
        }

        [JsonIgnore]
        public string GetDataTypeDisplay
        {
            get
            {
                return DataType == null ? "null" : DataType.GetNameDisplay;
            }
        }

        public SchemaTableColumn()
        {
            ColumnRoles = new ObservableCollection<Guid>();
            ColumnRoles.CollectionChanged += ColumnRoles_CollectionChanged;

        }

        public virtual void Validate_Name(StringBuilder error)
        {
            if (string.IsNullOrWhiteSpace(Name))
                error.AppendLine("Имя колонки не может быть пустым");

            if (Name != null && Name.Length > 128)
                error.AppendLine("Имя колонки длинее 128 символов");

        }

        public virtual void Validate_Roles(StringBuilder error)
        {
            if (ColumnRoles.Count == 0 && !(this is SchemaTableSystemColumn))
                error.AppendLine("У колонки нет ролей.");

        }

        public virtual void Validate(StringBuilder error)
        {
            if (string.IsNullOrWhiteSpace(Name))
                error.AppendLine("У колонки не заполнено поле 'Имя'.");

            if (Name != null && Name.Length > 128)
                error.AppendLine("Имя колонки длинее 128 символов: " + Name.Substring(0, 50).AsSQL());

            if (DataType == null)
                error.AppendLine("У колонки не заполнено поле 'Тип данных'.");

            if (Table == null)
                error.AppendLine("У колонки не заполнено поле 'Таблица'.");

            if (ColumnRoles.Count == 0 && !(this is SchemaTableSystemColumn))
                error.AppendLine("У колонки " + Name.AsSQL() + " нет ролей.");

            if (DataType != null)
                DataType.Validate(error);

        }


        void ColumnRoles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("ColumnRoles");
        }



        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (table != null)
                table.firePropertyChanged("Columns");
            needSave = true;
        }


        public virtual Column GetSmoColumn(Table parentSmoTable)
        {
            var col = new Column();
            col.Name = Name;
            col.Parent = parentSmoTable;
            col.DataType = DataType.GetSmoDataType();
            if (Table.GetPrimaryKeyColumn() != null && Name == Table.GetPrimaryKeyColumn().Name && Table.GetPrimaryKeyColumn().DataType is GuidDataType)
                col.RowGuidCol = true;
            col.Nullable = !IsNotNullable;
            return col;
        }

        void ISupportInitialize.BeginInit()
        {
            //throw new NotImplementedException();
        }

        void ISupportInitialize.EndInit()
        {
            //throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IViewColumn GetParentViewColumn()
        {
            return Table;
        }

        public IViewColumn GetSourceView()
        {
            return null;
        }

        public IViewColumn GetSourceViewColumn()
        {
            return null;
        }

        public virtual IViewColumn GetJoinView()
        {
            if (DataType is ForeingKeyDataType)
            {
                if (SchemaBaseRole.Roles.ContainsKey((Guid)(DataType as ForeingKeyDataType).RefTableID))
                    return (IViewColumn)SchemaBaseRole.Roles[(Guid)(DataType as ForeingKeyDataType).RefTableID];
                else
                    return (DataType as ForeingKeyDataType).GetRefTable();
            }
            else
                return null;
        }

        public IViewColumn GetColumnByName(string name)
        {
            return null;
        }

        public string GetDisplayName()
        {
            return Name;
        }

        public string Get4PartsTableName()
        {
            return "[?Get4PartsTableName?]";
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
            return this;
        }

        public Таблица_TableRole GetNativeTableRole()
        {
            return null;
        }

        public Колонка_ColumnRole GetNativeTableColumnRole()
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


        public string GetDisplayNameAndDataType()
        {
            return Name + ":  " + DataType.GetNameDisplay;
        }


        public void EmitSelectSql(StringBuilder sql, string indent)
        {
            throw new NotImplementedException();
        }

        public void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            throw new NotImplementedException();
        }

        public IEditControl GetEditControl()
        {
            return DataType.GetEditControl();
        }

        public string GetFullAlias()
        {
            return Name;
        }

        [Browsable(false)]
        [JsonIgnore]
        public string RolesDisplayValue
        {
            get { return SchemaBaseRole.GetRolesDisplayText(ColumnRoles); }
        }

        public SchemaTable GetRootNativeTable()
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

        public IViewColumn GetRootColumn()
        {
            throw new NotImplementedException();
        }

        public bool CanAcceptSqlValueFromColumn(SchemaTableColumn valueColumn)
        {
            return DataType.CanAcceptSqlValueFromDataType(valueColumn.DataType);
        }


        public bool GetIsSupportReadPermission()
        {
            return true;
        }

        public bool GetIsSupportInsertPermission()
        {
            return false;
        }

        public bool GetIsSupportUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportDeletePermission()
        {
            return false;
        }

        public bool GetIsSupportOwnedUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportOwnedDeletePermission()
        {
            return false;
        }

        public virtual string GetPermissionFolder()
        {
            return null;
        }

        public virtual Guid GetID()
        {
            return Table.ID;
        }

        public virtual string GetFieldName()
        {
            return Name;
        }

        public virtual string GetName()
        {
            return GetDisplayName();
        }

        public void GetChildPermissionObjects(List<IPermissionSupportObject> list)
        {

        }

        public bool GetNeedSave()
        {
            return needSave;
        }

        public void StartEditing()
        {
            needSave = false;
        }

        public void SaveChanges()
        {
            needSave = false;
        }

        public void CancelChanges()
        {
            needSave = false;
        }

        public string GetEditedObjectName()
        {
            return Name;
        }
    }

    public class TableColumnRolesSelectorTypeConverter : TypeConverter
    {
        private TypeConverter mTypeConverter;

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (mTypeConverter == null)
                mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

            if (context != null && destinationType == typeof(string))
            {
                if (value == null)
                    return "";
                else
                {
                    return SchemaBaseRole.GetRolesDisplayText(value as ObservableCollection<Guid>);
                }
            }
            return mTypeConverter.ConvertTo(context, culture, value, destinationType);
        }
    }

    ////public class TableColumnRolesSelectorEditor : ObjectSelectorEditor
    ////{
    ////    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    ////    {
    ////        return UITypeEditorEditStyle.Modal;
    ////    }

    ////    void dialog_OnFilterSchemaObject(SchemaBaseRole schemaObject, out bool visible)
    ////    {
    ////        visible = schemaObject is Колонка_ColumnRole;
    ////    }

    ////    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    ////    {

    ////        if (context != null
    ////            && context.Instance != null
    ////            && provider != null)
    ////        {
    ////            var editedRoles = value as ObservableCollection<Guid>;
    ////            SchemaTableColumn editedColumn = (SchemaTableColumn)context.Instance;


    ////            var dialog = new SchemaTableColumnRoleSelect_dialog();
    ////            dialog.MultiSelect = true;
    ////            dialog.AllowedTableRoles = editedColumn.Table.TableRoles;
    ////            dialog.OnFilterSchemaRole += dialog_OnFilterSchemaObject;
    ////            foreach (var role in editedRoles)
    ////                dialog.SelectedTableColumnRoles.Add(role);
    ////            dialog.LoadData();
    ////            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ////            {
    ////                editedRoles.Clear();
    ////                foreach (var role in dialog.SelectedTableColumnRoles)
    ////                    editedRoles.Add(role);
    ////            }
    ////        }

    ////        return value;
    ////    }
    ////    //protected override void FillTreeWithData(System.ComponentModel.Design.ObjectSelectorEditor.Selector theSel,
    ////    //  ITypeDescriptorContext theCtx, IServiceProvider theProvider)
    ////    //{
    ////    //    base.FillTreeWithData(theSel, theCtx, theProvider);  //clear the selection

    ////    //    //    jsqlTableColumn aCtl = (jsqlTableColumn)theCtx.Instance;

    ////    //    //foreach (Type tableType in mixUtil.GetAllSubclassTypes(typeof(mixTable)))
    ////    //    //{
    ////    //    //SelectorNode aNd = new SelectorNode(tableType.FullName, tableType);
    ////    //    //theSel.Nodes.Add(aNd);
    ////    //    //}

    ////    //    foreach (SchemaObject_cache table in App.Schema.Objects_cache.Values)
    ////    //    {
    ////    //        if (table.RootClass == typeof(SchemaTable).Name)
    ////    //        {
    ////    //            SelectorNode aNd = new SelectorNode(table.Name, table.ID);
    ////    //            theSel.Nodes.Add(aNd);
    ////    //        }
    ////    //    }

    ////    //}

    ////}



}
