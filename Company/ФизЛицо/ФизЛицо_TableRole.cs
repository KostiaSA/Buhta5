using Buhta;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компания
{


    [Export(typeof(SchemaBaseRole))]
    public class ФизЛицо_TableRole : Справочник_TableRole
    {
        public ФизЛицо_TableRole()
            : base()
        {
            ID = Const.ФизЛицо;
            Name = "^ФизЛицо";
            Description = "Физическое лицо";
            Position = 10;
        }

    }
}
