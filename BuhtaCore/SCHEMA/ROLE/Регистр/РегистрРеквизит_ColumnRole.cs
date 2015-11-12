using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрРеквизит_ColumnRole : Колонка_ColumnRole
    {
        public РегистрРеквизит_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Реквизит;
            Name = "^Регистр.Реквизит";
            Description = "Реквизит - любые дополнительные данные в регистрах и проводках";
            Position = 25;
            IsRequiredColumn = false;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 50 };
            NewColumnName = "Сумма";

            AllowedDataTypes.Add("Ссылка", new ForeingKeyDataType());
            AllowedDataTypes.Add("Строка", new StringDataType());
            AllowedDataTypes.Add("Дата", new DateDataType());
            AllowedDataTypes.Add("ДатаВремя", new DateTimeDataType());
            AllowedDataTypes.Add("Время", new TimeDataType());
            AllowedDataTypes.Add("Количество", new QuantityDataType());
            AllowedDataTypes.Add("Сумма", new MoneyDataType());
            AllowedDataTypes.Add("Целое", new IntDataType());
            AllowedDataTypes.Add("Guid", new GuidDataType());

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
