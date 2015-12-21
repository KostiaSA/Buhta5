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
    public class User_TableRole : Справочник_TableRole
    {
        public User_TableRole()
            : base()
        {
            ID = RoleConst.User;
            Name = "^User";
            Description = "Список пользователей (логины, пароли, доступы)";
            Position = 10;
        }

    }
}
