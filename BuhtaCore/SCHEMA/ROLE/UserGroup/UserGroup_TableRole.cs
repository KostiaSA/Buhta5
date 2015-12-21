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
    public class UserGroup_TableRole : Справочник_TableRole
    {
        public UserGroup_TableRole()
            : base()
        {
            ID = RoleConst.UserGroup;
            Name = "^UserGroup";
            Description = "Список групп пользователей";
            Position = 20;
        }

    }
}
