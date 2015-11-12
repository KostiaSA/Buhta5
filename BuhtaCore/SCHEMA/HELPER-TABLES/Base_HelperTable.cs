using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class Base_HelperTable : SchemaTable
    {
        protected Schema schema;
        public Base_HelperTable(Schema _schema)
            : base()
        {
            schema = _schema;
        }

        public virtual string GetFillSql()
        {
            return "";
        }
    }
}
