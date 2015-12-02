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
    public class IntDataType : SqlDataType
    {
        public override string Name { get { return "Целое"; } }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.Int;
        }

        public override string GetDeclareSql()
        {
            return "int";
        }
        public override bool IsNumeric()
        {
            return true;
        }
        ////public override IEditControl GetEditControl()
        ////{
        ////    var ctl = new StringEditControl<int?>();
        ////    ctl.Name = Column.Table.Name + "_" + Column.Name; ;
        ////    ctl.Text = Column.Table.Name + "_" + Column.Name; ;
        ////    ctl.Caption = Column.Name;
        ////    ctl.BindFieldName = Column.Name;

        ////    if (ctl.MaximumSize.Width == 0 || ctl.MaximumSize.Width > 200)
        ////        ctl.MaximumSize = new System.Drawing.Size(200, ctl.MaximumSize.Height);

        ////    return ctl;
        ////}

    }
}
