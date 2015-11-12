using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class Регистр_TableRole : Таблица_TableRole
    {
        public Регистр_TableRole()
            : base()
        {
            ID = RoleConst.Регистр;
            Name = "^Регистр";
            Description = "Регистр, счет управленческого или бухгалтерского учета";
            Position = 2000000;
        }

    }
}
