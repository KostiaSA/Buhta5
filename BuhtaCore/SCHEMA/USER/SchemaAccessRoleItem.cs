using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaAccessRoleItem : ISupportInitialize, TreeList.IVirtualTreeListData, INotifyPropertyChanged
    {
        public const string Role_category = "  Роль";

        Guid id;
        [Browsable(false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; firePropertyChanged("ID"); }
        }

        private string name;
        [DisplayName("Имя"), Description("Имя роли"), Category(Role_category)]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private string description;
        [DisplayName("Описание"), Description("Описание роли"), Category(Role_category)]
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("Description"); }
        }

        private Guid? parentRoleItemID;
        [Browsable(false)]
        public Guid? ParentRoleItemID
        {
            get { return parentRoleItemID; }
            set { parentRoleItemID = value; firePropertyChanged("ParentRoleItemID"); }
        }

        private SchemaAccessRole parentRole;
        [Browsable(false)]
        public SchemaAccessRole ParentRole
        {
            get { return parentRole; }
            set { parentRole = value; firePropertyChanged("ParentRole"); }
        }


        public ObservableCollection<SchemaObjectPermission> Permissions { get; private set; }

        public SchemaObjectPermission GetPermission(byte[] id)
        {
            foreach (var perm in Permissions)
            {
                if (perm.AccessNodeID.SequenceEqual(id))
                    return perm;
            }
            return null;
        }

        public SchemaAccessRoleItem()
        {
            Permissions = new ObservableCollection<SchemaObjectPermission>();
            Permissions.CollectionChanged += Permissions_CollectionChanged;

        }
        void Permissions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Permissions");
        }

        [JsonIgnore]
        public SchemaAccessRoleItem DesignTimeEditedMenu;


        public virtual void GetAllItems(List<SchemaAccessRoleItem> list)
        {
            list.Add(this);
        }

        public virtual List<SchemaAccessRoleItem> GetAllItems()
        {
            var list = new List<SchemaAccessRoleItem>();
            GetAllItems(list);
            return list;
        }

        //public int GetLevel()
        //{
        //    if (ParentRoleItemID == null)
        //        return 0;
        //    else
        //    {
        //        var parentFolder = SchemaMenu.GetFolderItemByID(ParentRoleItemID);
        //        if (parentFolder != null)
        //            return parentFolder.GetLevel() + 1;
        //        else
        //            return 0;
        //    }
        //}

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }

        public virtual void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            if (info.Column.FieldName == "Name")
                info.CellData = (info.Node as SchemaAccessRoleItem).Name;
            else
                if (info.Column.FieldName == "Description")
                    info.CellData = (info.Node as SchemaAccessRoleItem).Description;
                else
                    info.CellData = null;
        }

        public virtual void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            info.Children = null;
        }

        public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentRole != null)
                parentRole.firePropertyChanged("Items");

        }


        public string GetDisplayName()
        {
            return Name;
        }


        public string GetDisplayNameAndDataType()
        {
            return "GetFullAlias()";
        }


    }

}
