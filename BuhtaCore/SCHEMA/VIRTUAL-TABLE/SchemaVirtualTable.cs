using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public static class VirtualTableConst
    {
        public static Guid Оборотка = Guid.Parse("4698D105-963F-41CC-B3FA-B3FA8C966A78");
    }

    public class SchemaVirtualTableProperties : INotifyPropertyChanged
    {
        [JsonIgnore]
        public SchemaVirtualTable Table;

        private SchemaQueryRootColumn parentQueryColumn;
        public SchemaQueryRootColumn ParentQueryColumn
        {
            get { return parentQueryColumn; }
            set { parentQueryColumn = value; firePropertyChanged("ParentQueryColumn"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void firePropertyChanged(string propertyName)
        {
            //if (Table!=null)
            //    Table.ResetColumns();
            if (ParentQueryColumn != null)
                ParentQueryColumn.firePropertyChanged("VirtualTableProperties");
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        public virtual string DisplayName { get { return ToString(); } }


    }

    public class SchemaVirtualTable : ISchemaTreeListNode, IViewColumn
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }

        [ImportingConstructorAttribute]
        public SchemaVirtualTable()
        {
            properties = new SchemaVirtualTableProperties();
        }

        //public SchemaVirtualTable(SchemaVirtualTableProperties _properties)
        //{
        //    properties = _properties;
        //    if (properties != null)
        //    {
        //        Properties.Table = this;
        //        properties.PropertyChanged += properties_PropertyChanged;
        //        ResetColumns();
        //    }
        //}


        protected SchemaVirtualTableProperties properties;
        public SchemaVirtualTableProperties Properties
        {
            get { return properties; }
        }

        void properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetColumns();
        }

        public virtual string DisplayName { get { return Name; } }

        public void ResetColumns()
        {
            columns = null;
        }

        public virtual void CreateColumns()
        {
            columns = new List<SchemaVirtualTableColumn>();
        }

        private List<SchemaVirtualTableColumn> columns;
        public List<SchemaVirtualTableColumn> Columns
        {
            get
            {
                if (columns == null)
                {
                    CreateColumns();
                }
                return columns;
            }
        }


        public static SchemaVirtualTable GetVirtualTable(Guid id, SchemaVirtualTableProperties _properties)
        {
            if (VirtualTables.ContainsKey(id))
            {
                var retTable = (SchemaVirtualTable)Activator.CreateInstance(VirtualTables[id].GetType());

                retTable.properties = _properties;
                if (retTable.properties != null)
                {
                    retTable.properties.Table = retTable;
                    retTable.properties.PropertyChanged += retTable.properties_PropertyChanged;
                    retTable.ResetColumns();
                }

                return retTable;
            }
            else
                return null;

        }

        public static Dictionary<Guid, SchemaVirtualTable> VirtualTables;
        public static void LoadVirtualTables()
        {

            ////VirtualTables = new Dictionary<Guid, SchemaVirtualTable>();
            ////foreach (Lazy<SchemaVirtualTable> vTable in App.Mef.SchemaVirtualTables)
            ////{
            ////    if (VirtualTables.ContainsKey(vTable.Value.ID))
            ////    {
            ////        throw new Exception("Ошибка загрузки витруальных таблиц  '" + vTable.Value.GetType().FullName + "'.\n" +
            ////                            "Роль '" + VirtualTables[vTable.Value.ID].GetType().FullName +
            ////                            " имеет такой-же ID: " + VirtualTables[vTable.Value.ID].ID.AsSQL());
            ////    }
            ////    VirtualTables.Add(vTable.Value.ID, vTable.Value);
            ////}

            ////// инициализация
            ////foreach (var vTable in VirtualTables.Values)
            ////{
            ////    vTable.Initialize();
            ////}

        }

        public virtual void Initialize()
        {

        }


        //public static string GetRolesDisplayText(IList<Guid> rolesList)
        //{
        //    if (rolesList == null || rolesList.Count == 0)
        //        return "";

        //    var list = new List<SchemaBaseRole>();
        //    string errorStr = ""; ;
        //    foreach (var roleID in rolesList)
        //    {
        //        if (SchemaBaseRole.Roles.ContainsKey(roleID) && SchemaBaseRole.Roles[roleID] is SchemaBaseRole)
        //            list.Add(SchemaBaseRole.Roles[roleID] as SchemaBaseRole);
        //        else
        //            errorStr += ", ?ошибка";
        //    }
        //    var sb = new StringBuilder();
        //    foreach (var tableRole in from role in list orderby role.Position, role.Name select role)
        //    {
        //        sb.Append(tableRole.Name + ", ");
        //    }
        //    sb.RemoveLastChar(2);
        //    sb.Append(errorStr);
        //    return sb.ToString();

        //}


        ////public virtual void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////        info.CellData = Name;
        ////    else
        ////        if (info.Column.FieldName == "Description")
        ////            info.CellData = Description;
        ////        else
        ////            if (info.Column.FieldName == "Position")
        ////                info.CellData = Position;
        ////            else
        ////                info.CellData = "";
        ////}

        ////public virtual void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    //var list = new List<SchemaBaseRole>();
        ////    //foreach (var role in Roles)
        ////    //{
        ////    //    if (role.Value.GetType().BaseType.Equals(this.GetType()))
        ////    //    {
        ////    //        list.Add(role.Value);
        ////    //    }
        ////    //}
        ////    //if (list.Count > 0)
        ////    //    info.Children = list;
        ////    //else
        ////    info.Children = null;
        ////}

        Guid? parentObjectID;
        public Guid? ParentObjectID
        {
            get { return parentObjectID; }
            //    {
            //        if (parentObjectID == null)
            //        {
            //            foreach (var role in Roles.Values)
            //            {
            //                if (this.GetType().BaseType!=null && this.GetType().BaseType.Equals(role.GetType()))
            //                {
            //                    parentObjectID = role.ID;
            //                    break;
            //                }
            //            }
            //        }
            //        return parentObjectID;
            //    }
            set { parentObjectID = value; }
        }

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}


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
            return this;
        }

        public IViewColumn GetRootColumn()
        {
            return this;
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
            return Columns.ToList<IViewColumn>();
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
            return null;
        }

        public SchemaVirtualTable GetNativeVirtualTable()
        {
            return this;
        }

        public SchemaVirtualTableColumn GetNativeVirtualTableColumn()
        {
            return null;
        }

        public void EmitSelectSql(StringBuilder sql, string indent)
        {
            throw new NotImplementedException();
        }

        public virtual void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            sql.AppendLine("FROM [$virtual]");
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
