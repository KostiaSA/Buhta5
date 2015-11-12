using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class ВложеннаяТаблицаБизнесОперация_ColumnRole : Колонка_ColumnRole
    {
        public ВложеннаяТаблицаБизнесОперация_ColumnRole()
            : base()
        {
            ID = RoleConst.ВложеннаяТаблица_БизнесОперация;
            Name = "^ВложеннаяТаблица.БизнесОперация";
            Description = "Бизнес-операция в табличной части";
            Position = 0;
            IsRequiredColumn = false;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new ForeingKeyDataType() { RefTableID=SchemaTableDetailOper_HelperTable.StaticID};
            NewColumnName = "БизнесОперация";

            TableRoleType = typeof(ВложеннаяТаблица_TableRole);
        }
    }
}
