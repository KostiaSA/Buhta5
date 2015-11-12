using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ВложеннаяТаблица_TableRole : Таблица_TableRole
    {
        public ВложеннаяТаблица_TableRole()
            : base()
        {
            ID = RoleConst.ВложеннаяТаблица;
            Name = "^ВложеннаяТаблица";
            Description = "Вложенная таблица";
            Position = 2000000;
        }

    }
}
