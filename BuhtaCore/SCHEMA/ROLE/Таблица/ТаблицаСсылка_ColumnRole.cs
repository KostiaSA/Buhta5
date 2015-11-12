using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ТаблицаСсылка_ColumnRole : Колонка_ColumnRole
    {
        public ТаблицаСсылка_ColumnRole()
            : base()
        {
            ID = Guid.Parse("F13C7097-6619-4EF6-9CEB-0CD0D8F5CE48");
            Name = "^Таблица.Ссылка";
            Description = "Ссылка на запись в другой таблице (внешний ключ)";
            Position = 30;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = true;
            DataType = new ForeingKeyDataType();
            NewColumnName = "Новая ссылка";

            //AllowedDataTypes.Add("Новое количество", new QuantityDataType());
            //AllowedDataTypes.Add("Новое деньги", new MoneyDataType());
            //AllowedDataTypes.Add("Новое целое", new IntDataType());

            TableRoleType = typeof(Таблица_TableRole);

        }
    }
}
