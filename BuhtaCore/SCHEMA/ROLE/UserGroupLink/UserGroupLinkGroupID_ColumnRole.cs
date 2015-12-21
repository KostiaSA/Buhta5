using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserGroupLinkGroupID_ColumnRole : Колонка_ColumnRole
    {
        public UserGroupLinkGroupID_ColumnRole()
            : base()
        {
            ID = Guid.Parse("C15F81A6-FDE1-45A7-BD76-751D7C61B9E0");
            Name = "^UserGroupLink.GroupID";
            Description = "ID группы";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = RoleConst.UserGroup };
            NewColumnName = "GroupID";

            TableRoleType = typeof(UserGroupLink_TableRole);

        }
    }
}
