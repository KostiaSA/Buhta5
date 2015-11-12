using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class СправочникНазвание_ColumnRole : Колонка_ColumnRole
    {
        public СправочникНазвание_ColumnRole()
            : base()
        {
            ID = Guid.Parse("715B4131-7BDD-4614-A7CE-1E175386FBCD");
            Name = "^Справочник.Название";
            Description = "'Название' в справочнике";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 150 };
            NewColumnName = "Название";

            TableRoleType = typeof(Справочник_TableRole);

        }
    }
}
