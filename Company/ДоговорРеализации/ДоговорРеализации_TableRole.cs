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
    public class ДоговорРеализации_TableRole : Документ_TableRole
    {
        public ДоговорРеализации_TableRole()
            : base()
        {
            ID = Const.ДоговорРеализации;
            Name = "^ДоговорРеализации";
            Description = "Договор реализации товаров/услуг";
            Position = 30;
        }

    }
}
