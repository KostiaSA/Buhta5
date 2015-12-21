using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaBaseRole))]
    public class UserLastName_ColumnRole : Колонка_ColumnRole
    {
        public UserLastName_ColumnRole()
            : base()
        {
            ID = Guid.Parse("C6D4E0D3-AA6B-42F4-AFA0-1E688E3DAE6F");
            Name = "^User.LastName";
            Description = "Фамилия пользователя";
            Position = 20;
            IsRequiredColumn = true;
            IsMultiColumn = false;
            IsIndexed = false;
            DataType = new StringDataType() { MaxSize = 100 };
            NewColumnName = "LastName";

            TableRoleType = typeof(User_TableRole);

        }
    }
}
