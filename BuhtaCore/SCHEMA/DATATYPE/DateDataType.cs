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
    public class DateDataType : SqlDataType
    {
        public override string Name { get { return "Дата"; } }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.Date;
        }

        //public override IEditControl GetEditControl()
        //{
        //    var ctl = new DateEditControl();
        //    ctl.Name = Column.Table.Name + "_" + Column.Name; ;
        //  //  ctl.Text = Column.Table.Name + "_" + Column.Name; ;
        //    ctl.Caption = Column.Name;
        //    ctl.BindFieldName = Column.Name;

        //    if (ctl.MaximumSize.Width == 0 || ctl.MaximumSize.Width > 200)
        //        ctl.MaximumSize = new System.Drawing.Size(200, ctl.MaximumSize.Height);

        //    return ctl;
        //}

        public override string GetDeclareSql()
        {
            return "date";
        }

    }
}
