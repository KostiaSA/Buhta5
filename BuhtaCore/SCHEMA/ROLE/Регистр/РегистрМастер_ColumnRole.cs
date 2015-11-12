using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрМастер_ColumnRole : Колонка_ColumnRole
    {
        public РегистрМастер_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Мастер;
            Name = "^Регистр.Мастер";
            Description = "Ссылка на запись в мастер-таблице (обычно документ)";
            Position = 50;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = RoleConst.Таблица };
            NewColumnName = "Мастер";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
