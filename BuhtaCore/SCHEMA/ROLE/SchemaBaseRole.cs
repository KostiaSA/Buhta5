using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaBaseRole :  ISchemaTreeListNode
    {
        public Guid ID { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }
        public int Position { get;  set; }

        public virtual string DisplayName { get { return Name; } }

        public static Dictionary<Guid, SchemaBaseRole> Roles;
        public static void LoadRoles()
        {

            Roles = new Dictionary<Guid, SchemaBaseRole>();
            foreach (Lazy<SchemaBaseRole> role in App.Mef.SchemaRoles)
            {
                if (Roles.ContainsKey(role.Value.ID))
                {
                    throw new Exception("Ошибка загрузка роли '" + role.Value.GetType().FullName + "'.\n" +
                                        "Роль '" + Roles[role.Value.ID].GetType().FullName +
                                        " имеет такой-же ID: " + Roles[role.Value.ID].ID.AsSQL());
                }
                Roles.Add(role.Value.ID, role.Value);

                Debug.Print("роль: " + role.Value.GetType().FullName);
            }

            // инициализация
            foreach (var role in Roles.Values)
            {
                role.Initialize();
            }

        }

        public virtual void Initialize()
        {

        }


        public static string GetRolesDisplayText(IList<Guid> rolesList)
        {
            if (rolesList == null || rolesList.Count == 0)
                return "";

            var list = new List<SchemaBaseRole>();
            string errorStr = ""; ;
            foreach (var roleID in rolesList)
            {
                if (SchemaBaseRole.Roles.ContainsKey(roleID) && SchemaBaseRole.Roles[roleID] is SchemaBaseRole)
                    list.Add(SchemaBaseRole.Roles[roleID] as SchemaBaseRole);
                else
                    errorStr += ", ?ошибка";
            }
            var sb = new StringBuilder();
            foreach (var tableRole in from role in list orderby role.Position, role.Name select role)
            {
                sb.Append(tableRole.Name + ", ");
            }
            sb.RemoveLastChar(2);
            sb.Append(errorStr);
            return sb.ToString();

        }


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
        ////    var list = new List<SchemaBaseRole>();
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

        Guid? parentObjectID;
        public Guid? ParentObjectID
        {
            get
            {
                if (parentObjectID == null)
                {
                    foreach (var role in Roles.Values)
                    {
                        if (this.GetType().BaseType!=null && this.GetType().BaseType.Equals(role.GetType()))
                        {
                            parentObjectID = role.ID;
                            break;
                        }
                    }
                }
                return parentObjectID;
            }
            set { }
        }

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}
    }
}
