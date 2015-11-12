using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрМера_ColumnRole : Колонка_ColumnRole
    {
        public РегистрМера_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_Мера;
            Name = "^Регистр.Мера";
            Description = "Мера - количественные или денежные данные, например остаток товара или сумма проводки";
            Position = 20;
            IsRequiredColumn = true;
            IsMultiColumn = true;
            IsIndexed = false;
            DataType = new MoneyDataType();
            NewColumnName = "Сумма";

            AllowedDataTypes.Add("Количество", new QuantityDataType());
            AllowedDataTypes.Add("Сумма", new MoneyDataType());
            AllowedDataTypes.Add("Штуки", new IntDataType());

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
