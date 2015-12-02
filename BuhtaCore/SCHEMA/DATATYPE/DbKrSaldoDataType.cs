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
    public class DbKrSaldoDataType : SqlDataType
    {
        public override string Name { get { return "Дб/Кр/Сальдо"; } }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.NChar(1);
        }

        public override string GetDeclareSql()
        {
            return "nchar(1)";
        }

    }

    public enum DbKrSaldo { Дебет, Кредит, Сальдо }
}
