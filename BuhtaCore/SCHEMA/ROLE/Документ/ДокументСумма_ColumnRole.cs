using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ДокументСумма_ColumnRole : Колонка_ColumnRole
    {
        public ДокументСумма_ColumnRole()
            : base()
        {
            ID = RoleConst.Документ_Сумма;
            Name = "^Документ.Сумма";
            Description = "Сумма документа в валюте учета";
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new MoneyDataType();
            NewColumnName = "Сумма";

            TableRoleType = typeof(Документ_TableRole);

        }
    }
}
