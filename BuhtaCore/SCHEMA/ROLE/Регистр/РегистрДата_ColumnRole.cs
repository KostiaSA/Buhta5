using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрДата_ColumnRole : Колонка_ColumnRole
    {
        public РегистрДата_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Дата;
            Name = "^Регистр.Дата";
            Description = "Дата проводки/операции в регистре";
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new DateDataType();
            NewColumnName = "Дата";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
