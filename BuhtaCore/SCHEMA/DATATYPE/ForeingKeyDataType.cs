using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SqlDataType))]
    [Serializable]
    public class ForeingKeyDataType : SqlDataType
    {
        public override string Name { get { return "Ссылка"; } }

        private Guid? refTableID;
        ////[Editor(typeof(FKSchemaTableOrQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(FKSchemaTableOrQuerySelectorTypeConverter))]
        [DisplayName(" Таблица"), Description("Таблица, на которую указывает ссылка"), Category("  Колонка")]
        public Guid? RefTableID
        {
            get { return refTableID; }
            set { refTableID = value; firePropertyChanged("RefTableID"); refTable_cached = null; }
        }

        private Guid? lookupQueryID;
        ////[Editor(typeof(ForeingKeyLookupQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(ForeingKeyLookupQuerySelectorTypeConverter))]
        [DisplayName("Запрос для выбора"), Description("Запрос для выбора значений из таблицы (в полях формы)"), Category("  Колонка")]
        public Guid? LookupQueryID
        {
            get { return lookupQueryID; }
            set { lookupQueryID = value; firePropertyChanged("LookupQueryID"); }
        }

        [NonSerialized]
        SchemaTable refTable_cached;
        public SchemaTable GetRefTable()
        {
            if (RefTableID == null)
                return null;

            if (refTable_cached != null)
                return refTable_cached;

            if (SchemaBaseRole.Roles.ContainsKey((Guid)RefTableID))
                throw new Exception("internal error 4B9C89C6");

            refTable_cached = App.Schema.GetObject<SchemaTable>((Guid)RefTableID);

            return refTable_cached;
        }


        [Browsable(false)]
        public override string GetNameDisplay
        {
            get
            {
                if (RefTableID == null)
                    return "Ссылка-> ?";
                else
                {
                    if (SchemaBaseRole.Roles.ContainsKey((Guid)RefTableID))
                    {
                        return "Ссылка-> " + SchemaBaseRole.Roles[(Guid)RefTableID].Name;
                    }
                    else
                    {
                        var table = App.Schema.GetObject<SchemaTable>((Guid)RefTableID);
                        return table == null ? "Ссылка-> ?" : "Ссылка-> " + table.Name;
                    }
                }
            }
        }

        public override void Validate(StringBuilder error)
        {
            base.Validate(error);

            if (RefTableID == null)
                error.AppendLine("У колонки '" + (Column.Name == null ? "?" : Column.Name) + "' не заполнена ссылка на таблицу - внешний ключ.");
            else
            {
                if (!SchemaBaseRole.Roles.ContainsKey((Guid)RefTableID))
                {
                    var table = App.Schema.GetObject<SchemaTable>((Guid)RefTableID);
                    if (table == null)
                        error.AppendLine("У колонки '" + (Column.Name == null ? "?" : Column.Name) + "' неверная ссылка на таблицу - внешний ключ.");
                }
            }

        }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.UniqueIdentifier;
        }

        public override SqlDataType Clone()
        {
            var clonedColumn = base.Clone();
            (clonedColumn as ForeingKeyDataType).RefTableID = RefTableID;
            return clonedColumn;
        }

        public override string GetDeclareSql()
        {
            return "uniqueidentifier";
        }

        public override bool CanAcceptSqlValueFromDataType(SqlDataType valueDataType)
        {
            if (valueDataType is ForeingKeyDataType)
            {
                if (RefTableID != null && valueDataType != null && (valueDataType as ForeingKeyDataType).RefTableID == RefTableID) // полное совпадение
                {
                    return true;
                }
                else
                    if (RefTableID != null && valueDataType != null && SchemaBaseRole.Roles.ContainsKey((Guid)RefTableID))  // это роль
                    {
                        //var roleTable=SchemaBaseRole.Roles[(Guid)RefTableID] as Таблица_TableRole;
                        var valueRefTable = App.Schema.GetSampleObject<SchemaTable>((Guid)(valueDataType as ForeingKeyDataType).RefTableID);
                        if (valueRefTable.TableRoles.Contains((Guid)RefTableID))
                            return true;
                    }


            }

            return false;
        }

        ////public override IEditControl GetEditControl()
        ////{
        ////    var ctl = new ForeingKeyEditControl();
        ////    ctl.Name = Column.Table.Name + "_" + Column.Name; ;
        ////    ctl.Text = Column.Table.Name + "_" + Column.Name; ;
        ////    ctl.Caption = Column.Name;
        ////    ctl.BindFieldName = Column.Name;

        ////    if (LookupQueryID != null)
        ////        ctl.LookupQuery = App.Schema.GetObject<SchemaQuery>((Guid)LookupQueryID);
        ////    else 
        ////    {
        ////        if (RefTableID!=null  && SchemaBaseRole.Roles.ContainsKey((Guid)RefTableID))
        ////        {
        ////            var tableRole=(Таблица_TableRole)SchemaBaseRole.Roles[(Guid)RefTableID];
        ////            if (tableRole == null)
        ////            {
        ////                throw new Exception("'" + Column.Table.DisplayName + "'.'" + Column.Name + "': Неверная ссылка на таблицу-роль (внешний ключ).");
        ////            }
        ////            if (tableRole.LookupQueryID!=null)
        ////            {
        ////                ctl.LookupQuery = App.Schema.GetObject<SchemaQuery>((Guid)tableRole.LookupQueryID);
        ////                if (ctl.LookupQuery==null)
        ////                {
        ////                    throw new Exception("Таблица-роль '" + tableRole.Name + "'.'" + Column.Name + "': Неверно указан или отсутствует в конфигурации LookupQueryID.");
        ////                }
        ////            }
        ////            else
        ////                if (tableRole.DefaultQueryID != null)
        ////                {
        ////                    ctl.LookupQuery = App.Schema.GetObject<SchemaQuery>((Guid)tableRole.DefaultQueryID);
        ////                    if (ctl.LookupQuery == null)
        ////                    {
        ////                        throw new Exception("Таблица-роль '" + tableRole.Name + "'.'" + Column.Name + "': Неверно указан или отсутствует в конфигурации DefaultQueryID.");
        ////                    }
        ////                }
        ////                else
        ////                    throw new Exception("Таблица-роль '" + tableRole.Name + "'.'" + Column.Name + "': Не указаны ни LookupQueryID, ни DefaultQueryID.");

        ////        }
        ////        else
        ////        {
        ////            if (GetRefTable() == null)
        ////            {
        ////                throw new Exception("'" + Column.Table.DisplayName + "'.'" + Column.Name + "': Отсутствует или неверная ссылка (внешний ключ).");
        ////            }

        ////            if (GetRefTable().LookupQueryID != null)
        ////            {

        ////                ctl.LookupQuery = App.Schema.GetObject<SchemaQuery>((Guid)GetRefTable().LookupQueryID);
        ////            }
        ////            else
        ////            {
        ////                if (GetRefTable().DefaultQueryID == null)
        ////                {
        ////                    throw new Exception("У таблицы '" + GetRefTable().DisplayName + "' не указан запрос для выбора или просмотра.");
        ////                }
        ////                ctl.LookupQuery = App.Schema.GetObject<SchemaQuery>((Guid)GetRefTable().DefaultQueryID);
        ////            }
        ////        }
        ////    }

        ////    if (ctl.MaximumSize.Width == 0 || ctl.MaximumSize.Width > 800)
        ////        ctl.MaximumSize = new System.Drawing.Size(400, ctl.MaximumSize.Height);

        ////    return ctl;
        ////}

    }

    ////public class ForeingTableSelectorTypeConverter : TypeConverter
    ////{
    ////    private TypeConverter mTypeConverter;

    ////    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    ////    {
    ////        if (mTypeConverter == null)
    ////            mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

    ////        if (context != null && destinationType == typeof(string))
    ////        {
    ////            if (value == null)
    ////                return "<null>";
    ////            else
    ////            {
    ////                if (SchemaBaseRole.Roles.ContainsKey((Guid)value))
    ////                {
    ////                    return SchemaBaseRole.Roles[(Guid)value].Name;
    ////                }
    ////                else
    ////                {
    ////                    var table = App.Schema.GetObject<SchemaTable>((Guid)value);
    ////                    return table == null ? "<null>" : table.Name;
    ////                }
    ////            }
    ////        }
    ////        return mTypeConverter.ConvertTo(context, culture, value, destinationType);
    ////    }
    ////}

    ////public class ForeingTableSelectorEditor : ObjectSelectorEditor
    ////{
    ////    protected override void FillTreeWithData(System.ComponentModel.Design.ObjectSelectorEditor.Selector theSel,
    ////      ITypeDescriptorContext theCtx, IServiceProvider theProvider)
    ////    {
    ////        base.FillTreeWithData(theSel, theCtx, theProvider);  //clear the selection

    ////        //    jsqlTableColumn aCtl = (jsqlTableColumn)theCtx.Instance;

    ////        //foreach (Type tableType in mixUtil.GetAllSubclassTypes(typeof(mixTable)))
    ////        //{
    ////        //SelectorNode aNd = new SelectorNode(tableType.FullName, tableType);
    ////        //theSel.Nodes.Add(aNd);
    ////        //}

    ////        foreach (SchemaObject_cache table in App.Schema.Objects_cache.Values)
    ////        {
    ////            if (table.RootClass == typeof(SchemaTable).Name)
    ////            {
    ////                SelectorNode aNd = new SelectorNode(table.Name, table.ID);
    ////                theSel.Nodes.Add(aNd);
    ////            }
    ////        }

    ////    }

    ////}

    ////public class ForeingKeyLookupQuerySelectorTypeConverter : TypeConverter
    ////{
    ////    private TypeConverter mTypeConverter;

    ////    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    ////    {
    ////        if (mTypeConverter == null)
    ////            mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

    ////        if (context != null && destinationType == typeof(string))
    ////        {
    ////            if (value == null)
    ////                return "";
    ////            else
    ////            {
    ////                return App.Schema.GetObjectName((Guid)value);
    ////            }
    ////        }
    ////        return mTypeConverter.ConvertTo(context, culture, value, destinationType);
    ////    }
    ////}

    ////public class ForeingKeyLookupQuerySelectorEditor : ObjectSelectorEditor
    ////{
    ////    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    ////    {
    ////        return UITypeEditorEditStyle.Modal;
    ////    }

    ////    void dialog_OnFilterSchemaQuery(SchemaQuery schemaObject, out bool visible)
    ////    {
    ////        if (editedDataType!=null && editedDataType.RefTableID != null)
    ////        {
    ////            visible = schemaObject.GetRootNativeTableOrRoleID() == editedDataType.RefTableID;
    ////        }
    ////        else
    ////            visible = true;
    ////    }

    ////    ForeingKeyDataType editedDataType;
    ////    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    ////    {

    ////        if (context != null
    ////            && context.Instance != null
    ////            && provider != null)
    ////        {
    ////            editedDataType = (ForeingKeyDataType)context.Instance;


    ////            var dialog = new SchemaObjectSelect_dialog<SchemaQuery>();
    ////            dialog.OnFilterSchemaObject += dialog_OnFilterSchemaQuery;
    ////            dialog.LoadData();
    ////            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ////            {
    ////                value = dialog.SelectedObject.ID;
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

    ////public class FKSchemaTableOrQuerySelectorTypeConverter : TypeConverter
    ////{
    ////    private TypeConverter mTypeConverter;

    ////    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    ////    {
    ////        if (mTypeConverter == null)
    ////            mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

    ////        if (context != null && destinationType == typeof(string))
    ////        {
    ////            if (value == null)
    ////                value = "";
    ////            else
    ////                if (SchemaBaseRole.Roles.ContainsKey((Guid)value))
    ////                    value = (SchemaBaseRole.Roles[(Guid)value] as Таблица_TableRole).DisplayName;
    ////                else
    ////                    value = App.Schema.GetObjectTypeAndName((Guid?)value);
    ////        }
    ////        return mTypeConverter.ConvertTo(context, culture, value, destinationType);
    ////    }
    ////}

    ////public class FKSchemaTableOrQuerySelectorEditor : ObjectSelectorEditor
    ////{
    ////    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    ////    {
    ////        return UITypeEditorEditStyle.Modal;
    ////    }

    ////    void dialog_OnFilterSchemaObject(Object schemaObject, out bool visible)
    ////    {
    ////        visible = schemaObject is SchemaTable || schemaObject is SchemaQuery || schemaObject is Таблица_TableRole;
    ////    }

    ////    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    ////    {
    ////        if (context != null && context.Instance != null && provider != null)
    ////        {
    ////            ForeingKeyDataType editedDataType = (ForeingKeyDataType)context.Instance;

    ////            var dialog = new SchemaObjectSelect_dialog<ISchemaTreeListNode>();
    ////            dialog.IsIncludeRoleTables = true;
    ////            dialog.OnFilterSchemaObject += dialog_OnFilterSchemaObject;
    ////            dialog.LoadData();
    ////            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ////            {
    ////                //editedColumn.SourceQueryTableID = dialog.SelectedObject.ID;
    ////                return (dialog.SelectedObject as ISchemaTreeListNode).ID;
    ////            }

    ////        }
    ////        return value;
    ////    }
      

    ////}

}
