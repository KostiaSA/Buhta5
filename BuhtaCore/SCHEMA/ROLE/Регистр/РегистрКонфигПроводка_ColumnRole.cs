using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрКонфигПроводка_ColumnRole : Колонка_ColumnRole
    {
        public РегистрКонфигПроводка_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_КонфигПроводка;
            Name = "^Регистр.КонфигПроводка";
            Description = "Ссылка проводку таблицы (конфигурация)";
            Position = 90;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = SchemaTableDetailOperProvodka_HelperTable.StaticID };
            NewColumnName = "КонфигПроводка";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
