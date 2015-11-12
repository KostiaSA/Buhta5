using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class РегистрДбКр_ColumnRole : Колонка_ColumnRole
    {
        public РегистрДбКр_ColumnRole()
            : base()
        {
            ID = RoleConst.Регистр_ДбКр;
            Name = "^Регистр.ДбКр";
            Description = "Признак Дебет, Кредит или Сальдо";
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new DbKrSaldoDataType();
            NewColumnName = "ДбКр";

            TableRoleType = typeof(Регистр_TableRole);

        }
    }
}
