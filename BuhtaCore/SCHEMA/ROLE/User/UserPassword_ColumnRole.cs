using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserPassword_ColumnRole : Колонка_ColumnRole
    {
        public UserPassword_ColumnRole()
            : base()
        {
            ID = Guid.Parse("FEE417DD-E797-469D-BACC-D842051782D8");
            Name = "^User.Password";
            Description = "Пароль пользователя, зашифрованный в MD5 ";
            Position = 40;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = true;
            DataType = new StringDataType() { MaxSize = 100 };
            NewColumnName = "Password";

            TableRoleType = typeof(User_TableRole);

        }
    }
}
