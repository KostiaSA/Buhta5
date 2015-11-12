using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрДеталь_ColumnRole : Колонка_ColumnRole
    {
        public РегистрДеталь_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Деталь;
            Name = "^Регистр.Деталь";
            Description = "Ссылка на запись в деталь-таблице (обычно спецификация документа)";
            Position = 60;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = RoleConst.Таблица };
            NewColumnName = "Деталь";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
