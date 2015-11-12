using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ДокументДата_ColumnRole : Колонка_ColumnRole
    {
        public ДокументДата_ColumnRole()
            : base()
        {
            ID = RoleConst.Документ_Дата;
            Name = "^Документ.Дата";
            Description = "Дата документа";
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new DateDataType();
            NewColumnName = "Дата";

            TableRoleType = typeof(Документ_TableRole);

        }
    }
}
