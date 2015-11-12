using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ТаблицаДатаВремя_ColumnRole : Колонка_ColumnRole
    {
        public ТаблицаДатаВремя_ColumnRole()
            : base()
        {
            ID = Guid.Parse("0FFDEC1D-259F-4EF8-872F-7A972DED5D88");
            Name = "^Таблица.ДатаВремя";
            Description = "Дата или время";
            Position = 40;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new DateDataType();
            NewColumnName = "Новая дата";

            AllowedDataTypes.Add("Новое дата", new DateDataType());
            AllowedDataTypes.Add("Новое время", new TimeDataType());
            AllowedDataTypes.Add("Новое ДатаВремя", new DateTimeDataType());

            TableRoleType = typeof(Таблица_TableRole);

        }
    }
}
