using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ТаблицаКлюч_ColumnRole : Колонка_ColumnRole
    {
        public ТаблицаКлюч_ColumnRole()
            : base()
        {
            ID = RoleConst.Таблица_Ключ;
            Name = "^Таблица.Ключ";
            Description = "Первичный ключ таблицы (Guid)";
            IsNotNullable = true;
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new GuidDataType();
            NewColumnName = "ID";

            TableRoleType = typeof(Таблица_TableRole);
        }
    }
}
