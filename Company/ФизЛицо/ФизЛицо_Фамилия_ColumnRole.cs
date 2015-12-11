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
    public class ФизЛицо_Фамилия_ColumnRole : Колонка_ColumnRole
    {
        public ФизЛицо_Фамилия_ColumnRole()
            : base()
        {
            ID = Const.ФизЛицо_Фамилия;
            Name = "^ФизЛицо.Фамилия";
            Description = "Фамилия";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Фамилия";

            TableRoleType = typeof(ФизЛицо_TableRole);

        }
    }
}
