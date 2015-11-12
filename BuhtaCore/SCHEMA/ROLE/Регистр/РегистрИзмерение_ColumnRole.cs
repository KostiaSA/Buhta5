using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрИзмерение_ColumnRole : Колонка_ColumnRole
    {
        public РегистрИзмерение_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Измерение;
            Name = "^Регистр.Измерение";
            Description = "Измерения - данные предметной области, например заказчики, магазины или товары";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = true;
            IsIndexed = true;
            DataType = new ForeingKeyDataType();
            NewColumnName = "Субконто";

            AllowedDataTypes.Add("Ссылка", new ForeingKeyDataType());
            AllowedDataTypes.Add("Строка", new StringDataType());
            AllowedDataTypes.Add("Дата", new DateDataType());
            AllowedDataTypes.Add("ДатаВремя", new DateTimeDataType());
            AllowedDataTypes.Add("Время", new TimeDataType());
            AllowedDataTypes.Add("Количество", new QuantityDataType());
            AllowedDataTypes.Add("Сумма", new MoneyDataType());
            AllowedDataTypes.Add("Целое", new IntDataType());
            AllowedDataTypes.Add("Guid", new GuidDataType());
            //AllowedDataTypes.Add("Сумма", new MoneyDataType());

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
