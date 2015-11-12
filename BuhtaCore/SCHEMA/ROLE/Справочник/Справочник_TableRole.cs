using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class Справочник_TableRole : Таблица_TableRole
    {
        public Справочник_TableRole()
            : base()
        {
            ID = RoleConst.Справочник;
            Name = "^Справочник";
            Description = "Справочник с номером и названием";
            Position = 2000000;
        }

    }
}
