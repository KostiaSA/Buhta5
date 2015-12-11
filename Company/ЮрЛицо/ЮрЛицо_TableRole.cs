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
    public class ЮрЛицо_TableRole : Справочник_TableRole
    {
        public ЮрЛицо_TableRole()
            : base()
        {
            ID = Const.ЮрЛицо;
            Name = "^ЮрЛицо";
            Description = "Юридическое лицо";
            Position = 10;
        }

    }
}
