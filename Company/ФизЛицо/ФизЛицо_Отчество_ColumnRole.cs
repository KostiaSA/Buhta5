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
    public class ФизЛицо_Отчество_ColumnRole : Колонка_ColumnRole
    {
        public ФизЛицо_Отчество_ColumnRole()
            : base()
        {
            ID = Const.ФизЛицо_Отчество;
            Name = "^ФизЛицо.Отчество";
            Description = "Отчество";
            Position = 10;
            IsRequiredColumn = false;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Отчество";

            TableRoleType = typeof(ФизЛицо_TableRole);

        }
    }
}
