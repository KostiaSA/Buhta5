using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserFirstName_ColumnRole : Колонка_ColumnRole
    {
        public UserFirstName_ColumnRole()
            : base()
        {
            ID = Guid.Parse("A08AFF01-F02E-46BB-B89B-07CD25FFDFF4");
            Name = "^User.FirstName";
            Description = "Имя пользователя";
            Position = 10;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 100 };
            NewColumnName = "FirstName";

            TableRoleType = typeof(User_TableRole);

        }
    }
}
