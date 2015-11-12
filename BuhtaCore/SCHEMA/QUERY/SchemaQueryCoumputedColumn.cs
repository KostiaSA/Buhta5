using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaQueryComputedColumn : SchemaQueryBaseColumn
    {
        private string sqlText;
        [DisplayName("SqlText")]
        public string SqlText
        {
            get { return sqlText; }
            set { sqlText = value; firePropertyChanged("SqlText"); }
        }

        public override void EmitSelectSql(StringBuilder sql, string indent)
        {
            sql.Append(indent);
            if (!string.IsNullOrWhiteSpace(sqlText))
                sql.Append(sqlText);
            else
                sql.Append("'ошибка'");
            sql.Append(" AS [" + GetFullAlias() + "] /* computed column */,");

            //if (!string.IsNullOrWhiteSpace(Alias))
            //    sql.Append("[" + Alias + "],");
            //else
            //    sql.Append("[" + (ParentColumn.GetJoinTableFillAlias2() + "_" + Name).Substring(1) + "],");

            sql.AppendLine();
        }

        public override IViewColumn GetSourceTableColumn()
        {
            return null;
        }

    }

}
