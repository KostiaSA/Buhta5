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
    public class ЮрЛицо_ИНН_ColumnRole : Колонка_ColumnRole
    {
        public ЮрЛицо_ИНН_ColumnRole()
            : base()
        {
            ID = Const.ЮрЛицо_ИНН;
            Name = "^ЮрЛицо.ИНН";
            Description = "ИНН юр.лица";
            Position = 10;
            IsRequiredColumn = false;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 15 };
            NewColumnName = "ИНН";

            TableRoleType = typeof(ЮрЛицо_TableRole);

        }
    }
}
