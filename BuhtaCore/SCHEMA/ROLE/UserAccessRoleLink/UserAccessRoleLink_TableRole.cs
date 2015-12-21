using Buhta;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{


    [Export(typeof(SchemaBaseRole))]
    public class UserAccessRoleLink_TableRole : Таблица_TableRole
    {
        public UserAccessRoleLink_TableRole()
            : base()
        {
            ID = RoleConst.UserAccessRoleLink;
            Name = "^UserAccessRoleLink";
            Description = "Привязка пользователей к ролям доступа";
            Position = 40;
        }

    }
}
