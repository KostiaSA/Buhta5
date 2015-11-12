using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ТаблицаСтрока_ColumnRole : Колонка_ColumnRole
    {
        public ТаблицаСтрока_ColumnRole()
            : base()
        {
            ID = Guid.Parse("4978799F-1C44-4AFC-9F52-E094D0D9FFFC");
            Name = "^Таблица.Строка";
            Description = "Строка";
            Position = 10;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Новая строка";

            TableRoleType = typeof(Таблица_TableRole);

        }
    }
}
