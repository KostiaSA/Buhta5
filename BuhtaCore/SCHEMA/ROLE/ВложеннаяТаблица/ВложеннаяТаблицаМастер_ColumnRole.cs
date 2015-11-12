using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ВложеннаяТаблицаМастер_ColumnRole : Колонка_ColumnRole
    {
        public ВложеннаяТаблицаМастер_ColumnRole()
            : base()
        {
            ID = RoleConst.ВложеннаяТаблица_Мастер;
            Name = "^ВложеннаяТаблица.Мастер";
            Description = "Ссылка на мастер-таблицу (Guid)";
            Position = 0;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new GuidDataType();
            NewColumnName = "Мастер";

            TableRoleType = typeof(ВложеннаяТаблица_TableRole);
        }
    }
}
