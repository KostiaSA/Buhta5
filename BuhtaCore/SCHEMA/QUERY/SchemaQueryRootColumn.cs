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

    public class SchemaQueryRootColumn : SchemaQueryJoinColumn
    {
        Guid? sourceQueryTableID;
        ////[Editor(typeof(SchemaTableOrQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaTableOrQuerySelectorTypeConverter))]
        [DisplayName(" Таблица/запрос"), Description("Корневая таблица или запрос"), Category(" Таблица/запрос")]
        public Guid? SourceQueryTableID
        {
            get { return sourceQueryTableID; }
            set { sourceQueryTableID = value; joinView_cached = null; firePropertyChanged("SourceQueryTableID"); }
        }

        SchemaVirtualTableProperties virtualTableProperties;
        ////[Editor(typeof(VirtualTablePropertiesSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ////[TypeConverter(typeof(VirtualTablePropertiesSelectorTypeConverter))]
        [DisplayName("Настройка таблицы/запроса"), Description("Настройка корневой таблицы/запроса"), Category(" Таблица/запрос")]
        public SchemaVirtualTableProperties VirtualTableProperties
        {
            get { return virtualTableProperties; }
            set { virtualTableProperties = value; joinView_cached = null; firePropertyChanged("VirtualTableProperties"); }
        }

        public override void GetAllColumns(List<SchemaQueryBaseColumn> list)
        {
            foreach (var col in Columns)
                col.GetAllColumns(list);
        }

        //public override IQueryTable GetSourceTable()
        //{
        //    return SourceQueryTable;
        //}

        public override string GetQueryDesignerDisplayName()
        {
            if (GetJoinView() != null)
                return GetJoinView().GetDisplayName();
            else
                return "Root table or query";
        }

        ////public override void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////    {
        ////        if (GetJoinView() != null)
        ////            info.CellData = GetJoinView().GetDisplayName();
        ////        else
        ////            info.CellData = "Root table or query";
        ////    }
        ////    else
        ////        info.CellData = null;
        ////}

        IViewColumn joinView_cached;
        public override IViewColumn GetJoinView()
        {
            if (sourceQueryTableID == null)
                return null;

            if (joinView_cached != null)
                return joinView_cached;

            if (SchemaVirtualTable.VirtualTables.ContainsKey((Guid)sourceQueryTableID))
            {
                joinView_cached = SchemaVirtualTable.GetVirtualTable((Guid)sourceQueryTableID, VirtualTableProperties);
            }
            else
                if (SchemaBaseRole.Roles.ContainsKey((Guid)sourceQueryTableID))
                {
                    joinView_cached = SchemaBaseRole.Roles[(Guid)sourceQueryTableID] as Таблица_TableRole;
                }
                else
                    joinView_cached = (IViewColumn)App.Schema.GetObject<SchemaObject>((Guid)sourceQueryTableID);

            return joinView_cached;
        }

        public override IViewColumn GetParentViewColumn()
        {
            return null;
        }

        public override IViewColumn GetSourceView()
        {
            return null;
        }

        public override IViewColumn GetSourceViewColumn()
        {
            return null;
        }

        public override string GetJoinTableFillAlias()
        {
            if (!string.IsNullOrWhiteSpace(Alias))
                return Alias;
            else
                if (GetJoinView().GetNativeTable() != null)
                    return GetJoinView().GetNativeTable().DisplayName;
                else
                    return GetJoinView().Name;
        }

        public override string GetJoinTableFillAlias2()
        {
            return "";
        }

        public override void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            if (GetJoinView() is SchemaVirtualTable)
            {
                GetJoinView().EmitJoinSql(sql, withCTE, indent);

            }
            else
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
                    sb.Append(GetJoinView().GetNativeTableRole().GetSqlText(withCTE, ParentQuery.RuntimeRecordIDFilter));
                    sb.AppendLine("),");
                    withCTE.Add(sb.ToString());
                }

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

                sql.AppendLine("FROM");
                //sql.AppendLine("  [" + GetJoinView().Name + "] AS [" + GetJoinTableFillAlias() + "]");
                sql.AppendLine("  " + GetJoinView().Get4PartsTableName() + " AS [" + GetJoinTableFillAlias() + "]");

            }

            foreach (var col in Columns)
                col.EmitJoinSql(sql, withCTE, indent + "  ");
        }

        public override void EmitSelectSql(StringBuilder sql, string indent)
        {
            foreach (var col in Columns)
                col.EmitSelectSql(sql, indent);

            sql.Append(indent);
            sql.Append("[" + GetJoinTableFillAlias() + "].");
            if (GetJoinView().GetNativeTable() != null)
                sql.Append("[" + GetJoinView().GetNativeTable().GetPrimaryKeyColumn().Name + "]");
            else
                sql.Append("__ID__");
            sql.AppendLine(" AS __ID__,");

            ////if (!string.IsNullOrWhiteSpace(Alias))
            ////    sql.Append("[" + Alias + "],");
            ////else
            ////    sql.Append("[" + (ParentColumn.GetJoinTableFillAlias2() + "_" + Name).Substring(1) + "],");

            //sql.AppendLine();
        }

    }

    public class SchemaTableOrQuerySelectorTypeConverter : TypeConverter
    {
        private TypeConverter mTypeConverter;

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (mTypeConverter == null)
                mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

            if (context != null && destinationType == typeof(string))
            {
                if (value == null)
                    value = "";
                else
                    if (SchemaBaseRole.Roles.ContainsKey((Guid)value))
                        value = (SchemaBaseRole.Roles[(Guid)value] as Таблица_TableRole).DisplayName;
                    else
                        if (SchemaVirtualTable.VirtualTables.Keys.Contains((Guid)value))
                            value = (SchemaVirtualTable.VirtualTables[(Guid)value]).DisplayName;
                        else
                            value = App.Schema.GetObjectTypeAndName((Guid?)value);
            }
            return mTypeConverter.ConvertTo(context, culture, value, destinationType);
        }
    }

    ////public class SchemaTableOrQuerySelectorEditor : ObjectSelectorEditor
    ////{
    ////    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    ////    {
    ////        return UITypeEditorEditStyle.Modal;
    ////    }

    ////    void dialog_OnFilterSchemaObject(Object schemaObject, out bool visible)
    ////    {
    ////        visible = schemaObject is SchemaTable || schemaObject is SchemaQuery || schemaObject is Таблица_TableRole || schemaObject is SchemaVirtualTable;
    ////    }

    ////    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    ////    {
    ////        if (context != null && context.Instance != null && provider != null)
    ////        {
    ////            SchemaQueryJoinColumn editedColumn = (SchemaQueryJoinColumn)context.Instance;

    ////            var dialog = new SchemaObjectSelect_dialog<ISchemaTreeListNode>();
    ////            dialog.IsIncludeRoleTables = true;
    ////            dialog.OnFilterSchemaObject += dialog_OnFilterSchemaObject;
    ////            dialog.LoadData();
    ////            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ////            {
    ////                //editedColumn.SourceQueryTableID = dialog.SelectedObject.ID;
    ////                if (editedColumn is SchemaQueryRootColumn)
    ////                {
    ////                    if (dialog.SelectedObject is SchemaVirtualTable)
    ////                    {
    ////                        (editedColumn as SchemaQueryRootColumn).VirtualTableProperties = (dialog.SelectedObject as SchemaVirtualTable).Properties;
    ////                        (editedColumn as SchemaQueryRootColumn).VirtualTableProperties.ParentQueryColumn = editedColumn as SchemaQueryRootColumn;
    ////                    }
    ////                    else
    ////                        (editedColumn as SchemaQueryRootColumn).VirtualTableProperties = null;
    ////                }

    ////                return (dialog.SelectedObject as ISchemaTreeListNode).ID;
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

    ////    //    foreach (SchemaObject_cache query in App.Schema.Objects_cache.Values)
    ////    //    {
    ////    //        if (query.RootClass == typeof(SchemaTable).Name)
    ////    //        {
    ////    //            SelectorNode aNd = new SelectorNode(App.Schema.GetObjectTypeAndName(query.ID), query.ID);
    ////    //            theSel.Nodes.Add(aNd);
    ////    //        }

    ////    //        if (query.RootClass == typeof(SchemaQuery).Name)
    ////    //        {
    ////    //            SelectorNode aNd = new SelectorNode(App.Schema.GetObjectTypeAndName(query.ID), query.ID);
    ////    //            theSel.Nodes.Add(aNd);
    ////    //        }
    ////    //    }

    ////    //}

    ////}

    ////public class VirtualTablePropertiesSelectorTypeConverter : TypeConverter
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
    ////                value = (value as SchemaVirtualTableProperties).DisplayName;
    ////            //if (SchemaBaseRole.Roles.ContainsKey((Guid)value))
    ////            //    value = (SchemaBaseRole.Roles[(Guid)value] as Таблица_TableRole).DisplayName;
    ////            //else
    ////            //    value = App.Schema.GetObjectTypeAndName((Guid?)value);
    ////        }
    ////        return mTypeConverter.ConvertTo(context, culture, value, destinationType);
    ////    }
    ////}

    ////public class VirtualTablePropertiesSelectorEditor : ObjectSelectorEditor
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
    ////            SchemaQueryJoinColumn editedColumn = (SchemaQueryJoinColumn)context.Instance;

    ////            if ((editedColumn as SchemaQueryRootColumn).VirtualTableProperties != null)
    ////            {
    ////                var dialog = new OborotkaPropertiesSetup_dialog();
    ////                dialog.EditedProperties = (editedColumn as SchemaQueryRootColumn).VirtualTableProperties as OborotkaProperties;
    ////                dialog.LoadData();
    ////                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ////                {
    ////                    //editedColumn.SourceQueryTableID = dialog.SelectedObject.ID;
    ////                    return dialog.EditedProperties;
    ////                }
    ////            }
    ////        }
    ////        return value;
    ////    }

    ////}
}
