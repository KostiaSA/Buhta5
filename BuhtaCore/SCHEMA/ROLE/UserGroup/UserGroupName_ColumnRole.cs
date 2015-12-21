using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserGroupName_ColumnRole : Колонка_ColumnRole
    {
        public UserGroupName_ColumnRole()
            : base()
        {
            ID = Guid.Parse("027F07BA-75B2-4F2A-9FE9-A0B66250A7C3");
            Name = "^UserGroup.Name";
            Description = "Имя группы пользователей";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 100 };
            NewColumnName = "Name";

            TableRoleType = typeof(UserGroup_TableRole);

        }
    }
}
