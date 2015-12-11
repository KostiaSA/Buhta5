using Buhta;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компания
{
    [Export(typeof(SchemaBaseRole))]
    public class ДоговорРеализации_Клиент_ColumnRole : Колонка_ColumnRole
    {
        public ДоговорРеализации_Клиент_ColumnRole()
            : base()
        {
            ID = Const.ДоговорРеализации_Клиент;
            Name = "^ДоговорРеализации.Клиент";
            Description = "Клиент в договоре";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new ForeingKeyDataType() { RefTableID = Const.Клиент };
            NewColumnName = "Клиент";

            TableRoleType = typeof(ДоговорРеализации_TableRole);

        }
    }
}
