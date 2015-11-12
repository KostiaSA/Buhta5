using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class СправочникНомер_ColumnRole : Колонка_ColumnRole
    {
        public СправочникНомер_ColumnRole()
            : base()
        {
            ID = Guid.Parse("8D7635CC-548B-4796-A47D-6BCA6B21B1FF");
            Name = "^Справочник.Номер";
            Description = "'Номер' в справочнике";
            Position = 0;
            IsRequiredColumn = false;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 30 };
            NewColumnName = "Номер";

            TableRoleType = typeof(Справочник_TableRole);

        }
    }
}
