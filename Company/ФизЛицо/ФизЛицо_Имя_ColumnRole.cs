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
    public class ФизЛицо_Имя_ColumnRole : Колонка_ColumnRole
    {
        public ФизЛицо_Имя_ColumnRole()
            : base()
        {
            ID = Const.ФизЛицо_Имя;
            Name = "^ФизЛицо.Имя";
            Description = "Имя";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Имя";

            TableRoleType = typeof(ФизЛицо_TableRole);

        }
    }
}
