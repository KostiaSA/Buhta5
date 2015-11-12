using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public interface IPermissionSupportObject
    {
        string GetPermissionFolder();
        Guid GetID();
        string GetFieldName();
        string GetName();
        bool GetIsSupportReadPermission();
        bool GetIsSupportInsertPermission();
        bool GetIsSupportUpdatePermission();
        bool GetIsSupportDeletePermission();
        bool GetIsSupportOwnedUpdatePermission();
        bool GetIsSupportOwnedDeletePermission();
        void GetChildPermissionObjects(List<IPermissionSupportObject> list);
    }

    public enum AccessType { Allowed, Denied }

    public class SchemaObjectPermission : INotifyPropertyChanged
    {

        byte[] accessNodeID;
        [Browsable(false)]
        public byte[] AccessNodeID
        {
            get { return accessNodeID; }
            set { accessNodeID = value; firePropertyChanged("AccessNodeID"); }
        }

        AccessType access;
        public AccessType Access
        {
            get { return access; }
            set { access = value; firePropertyChanged("Access"); }
        }

        bool isReadOk;
        public bool IsReadOk
        {
            get { return isReadOk; }
            set 
            { 
                isReadOk = value; 
                firePropertyChanged("IsReadOk"); 
            }
        }

        bool isInsertOk;
        public bool IsInsertOk
        {
            get { return isInsertOk; }
            set { isInsertOk = value; firePropertyChanged("IsInsertOk"); }
        }

        bool isUpdateOk;
        public bool IsUpdateOk
        {
            get { return isUpdateOk; }
            set 
            { 
                isUpdateOk = value;
                firePropertyChanged("IsUpdateOk"); 
            }
        }

        bool isDeleteOk;
        public bool IsDeleteOk
        {
            get { return isDeleteOk; }
            set { isDeleteOk = value; firePropertyChanged("IsDeleteOk"); }
        }

        bool isOwnedUpdateOk;
        public bool IsOwnedUpdateOk
        {
            get { return isOwnedUpdateOk; }
            set { isOwnedUpdateOk = value; firePropertyChanged("IsOwnedUpdateOk"); }
        }

        bool isOwnedDeleteOk;
        public bool IsOwnedDeleteOk
        {
            get { return isOwnedDeleteOk; }
            set { isOwnedDeleteOk = value; firePropertyChanged("IsDeleteOk"); }
        }

        private SchemaAccessRoleItem parentRoleItem;
        [Browsable(false)]
        public SchemaAccessRoleItem ParentRoleItem
        {
            get { return parentRoleItem; }
            set { parentRoleItem = value; firePropertyChanged("ParentRole"); }
        }

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentRoleItem != null)
                parentRoleItem.firePropertyChanged("Permissions");

        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SchemaTablePermission:SchemaObjectPermission
    {

    }


}
