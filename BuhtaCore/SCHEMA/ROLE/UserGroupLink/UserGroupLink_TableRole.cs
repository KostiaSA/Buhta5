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
    public class UserGroupLink_TableRole : Таблица_TableRole
    {
        public UserGroupLink_TableRole()
            : base()
        {
            ID = RoleConst.UserGroupLink;
            Name = "^UserGroupLink";
            Description = "Привязка пользователей к группам";
            Position = 30;
        }

    }
}
