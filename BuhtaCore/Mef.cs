using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class Mef
    {
        [ImportMany]
        public IEnumerable<Lazy<SqlDataType>> SqlDataTypes;

        [ImportMany]
        public IEnumerable<Lazy<SchemaFormControl>> FormControls;

        [ImportMany]
        public IEnumerable<Lazy<SchemaAction>> SchemaActions;

        [ImportMany]
        public IEnumerable<Lazy<SchemaObject>> SchemaObjects;

        [ImportMany]
        public IEnumerable<Lazy<SchemaBaseRole>> SchemaRoles;

        [ImportMany]
        public IEnumerable<Lazy<SchemaVirtualTable>> SchemaVirtualTables;

        [ImportMany]
        public IEnumerable<Lazy<SchemaMenuBaseAction>> SchemaMenuActions;
    }
}
