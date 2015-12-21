using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserGroupLinkUserID_ColumnRole : Колонка_ColumnRole
    {
        public UserGroupLinkUserID_ColumnRole()
            : base()
        {
            ID = Guid.Parse("21EF6BBE-3717-40E6-857F-731912E980C9");
            Name = "^UserGroupLink.UserID";
            Description = "ID пользователя";
            Position = 20;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = RoleConst.User };
            NewColumnName = "UserID";

            TableRoleType = typeof(UserGroupLink_TableRole);

        }
    }
}
