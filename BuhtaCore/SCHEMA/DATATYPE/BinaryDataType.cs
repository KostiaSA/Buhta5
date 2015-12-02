using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [Export(typeof(SqlDataType))]
    [Serializable]
    public class BinaryDataType : SqlDataType
    {
        public override string Name { get { return "Binary"; } }

        public int MaxLength { get; set; }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.Binary(MaxLength);
        }
        public override IEditControl GetEditControl()
        {
            return null;
        }

        public override string GetDeclareSql()
        {
            if (MaxLength == 0)
                return "binary(max)";
            else
                return "binary(" + MaxLength + ")";
        }

    }
}
