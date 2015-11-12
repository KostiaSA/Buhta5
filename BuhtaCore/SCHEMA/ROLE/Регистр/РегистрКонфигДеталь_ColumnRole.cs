using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрКонфигДеталь_ColumnRole : Колонка_ColumnRole
    {
        public РегистрКонфигДеталь_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_КонфигДеталь;
            Name = "^Регистр.КонфигДеталь";
            Description = "Ссылка деталь таблицы (конфигурация)";
            Position = 70;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = SchemaTableDetail_HelperTable.StaticID };
            NewColumnName = "КонфигДеталь";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
