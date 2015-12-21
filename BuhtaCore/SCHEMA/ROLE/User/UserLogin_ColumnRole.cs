using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserLogin_ColumnRole : Колонка_ColumnRole
    {
        public UserLogin_ColumnRole()
            : base()
        {
            ID = Guid.Parse("D2E22660-8BEF-4729-8868-E70F12AFF195");
            Name = "^User.Login";
            Description = "Логин пользователя";
            Position = 30;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 100 };
            NewColumnName = "Login";

            TableRoleType = typeof(User_TableRole);

        }
    }
}
