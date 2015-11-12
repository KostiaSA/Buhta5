using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ТаблицаЧисло_ColumnRole : Колонка_ColumnRole
    {
        public ТаблицаЧисло_ColumnRole()
            : base()
        {
            ID = Guid.Parse("8BB69D23-B72E-47CD-81AD-BD1584347CE9");
            Name = "^Таблица.Число";
            Description = "Числовое значение";
            Position = 30;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new QuantityDataType();
            NewColumnName = "Новое число";

            AllowedDataTypes.Add("Новое количество", new QuantityDataType());
            AllowedDataTypes.Add("Новое деньги", new MoneyDataType());
            AllowedDataTypes.Add("Новое целое", new IntDataType());

            TableRoleType = typeof(Таблица_TableRole);

        }
    }
}
