using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserAccessRoleLinkUserID_ColumnRole : Колонка_ColumnRole
    {
        public UserAccessRoleLinkUserID_ColumnRole()
            : base()
        {
            ID = Guid.Parse("CE25CE81-2C46-4E81-B8F8-42FE33066481");
            Name = "^UserAccessRoleLink.UserID";
            Description = "ID пользователя";
            Position = 20;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = RoleConst.User };
            NewColumnName = "UserID";

            TableRoleType = typeof(UserAccessRoleLink_TableRole);

        }
    }
}
