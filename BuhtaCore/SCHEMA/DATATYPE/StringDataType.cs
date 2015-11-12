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
    public class StringDataType : SqlDataType
    {
        [Browsable(false)]
        public override string Name
        {
            get
            {
                return "Строка";
            }
        }

        private int maxSize;
        [DisplayName("Макс.длина"), Description("Максимальная длина строки (до 4096), если 0 - то неограниченно.")]
        public int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; firePropertyChanged("MaxSize"); }
        }

        [Browsable(false)]
        public override string GetNameDisplay
        {
            get
            {
                return MaxSize == 0 ? Name + "(max)" : Name + "(" + MaxSize + ")";
            }
        }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            if (MaxSize == 0)
                return Microsoft.SqlServer.Management.Smo.DataType.NVarCharMax;
            else
                return Microsoft.SqlServer.Management.Smo.DataType.NVarChar(MaxSize);
        }

        public override IEditControl GetEditControl()
        {
            var ctl = new StringEditControl<string>();
            ctl.Name = Column.Table.Name + "_" + Column.Name; ;
            ctl.Text = Column.Table.Name + "_" + Column.Name; ;
            ctl.Caption = Column.Name;
            ctl.BindFieldName = Column.Name;
            if (MaxSize != 0)
            {
                ctl.Properties.MaxLength = MaxSize;
                ctl.Text = new String('0', MaxSize);
                ctl.MaximumSize = ctl.CalcBestSize();
                ctl.Text = null;
            }

            if (ctl.MaximumSize.Width == 0 || ctl.MaximumSize.Width >800)
                ctl.MaximumSize = new System.Drawing.Size(800, ctl.MaximumSize.Height);

            return ctl;
        }

        public override SqlDataType Clone()
        {
            var clonedColumn = base.Clone();
            (clonedColumn as StringDataType).MaxSize = MaxSize;
            return clonedColumn;
        }

        public override string GetDeclareSql()
        {
            if (MaxSize == 0)
                return "nvarchar(max)";
            else
                return "nvarchar(" + MaxSize + ")";
        }


    }
}
