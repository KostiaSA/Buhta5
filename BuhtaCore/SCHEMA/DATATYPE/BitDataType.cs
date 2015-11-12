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
    public class BitDataType : SqlDataType
    {
        public override string Name { get { return "Bit"; } }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.Bit;
        }

        public override string GetDeclareSql()
        {
            return "bit";
        }

        public override bool IsNumeric()
        {
            return true;
        }
        public override IEditControl GetEditControl()
        {
            var ctl = new BitEditControl();
            ctl.Name = Column.Table.Name + "_" + Column.Name; ;
            ctl.Text = Column.Table.Name + "_" + Column.Name; ;
            ctl.Caption = Column.Name;
            ctl.BindFieldName = Column.Name;

            if (ctl.MaximumSize.Width == 0 || ctl.MaximumSize.Width > 200)
                ctl.MaximumSize = new System.Drawing.Size(200, ctl.MaximumSize.Height);

            return ctl;
        }


    }
}
