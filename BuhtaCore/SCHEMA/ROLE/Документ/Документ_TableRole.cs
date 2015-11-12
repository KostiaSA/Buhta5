using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class Документ_TableRole : Справочник_TableRole
    {
        public Документ_TableRole()
            : base()
        {
            ID = RoleConst.Документ;
            Name = "^Документ";
            Description = "Документ, Договор, Бизнес-операция";
            Position = 3000000;
        }

    }
}
