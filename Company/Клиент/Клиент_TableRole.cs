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
    public class Клиент_TableRole : Справочник_TableRole
    {
        public Клиент_TableRole()
            : base()
        {
            ID = Const.Клиент;
            Name = "^Клиент";
            Description = "Клиент";
            Position = 30;
            DefaultQueryID = Guid.Parse("FB242E57-C00B-4DB4-A016-607039A3AFCF"); 
        }

    }
}
