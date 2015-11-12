using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрКонфигБизнесОперация_ColumnRole : Колонка_ColumnRole
    {
        public РегистрКонфигБизнесОперация_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_КонфигБизнесОперация;
            Name = "^Регистр.КонфигБизнесОперация";
            Description = "Ссылка бизнес-операцию таблицы (конфигурация)";
            Position = 80;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = SchemaTableDetailOper_HelperTable.StaticID };
            NewColumnName = "КонфигБизнесОперация";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
