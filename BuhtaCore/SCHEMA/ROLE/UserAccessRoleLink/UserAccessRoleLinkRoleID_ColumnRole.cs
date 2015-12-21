using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserAccessRoleLinkRoleID_ColumnRole : Колонка_ColumnRole
    {
        public UserAccessRoleLinkRoleID_ColumnRole()
            : base()
        {
            ID = Guid.Parse("98FC0F51-68B0-481C-BDD7-599657BEFB4A");
            Name = "^UserAccessRoleLink.RoleID";
            Description = "ID роли доступа";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new GuidDataType();
            NewColumnName = "RoleID";

            TableRoleType = typeof(UserAccessRoleLink_TableRole);

        }
    }
}
