using DevExpress.XtraTreeList;
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

    public class SchemaQuerySelectColumn : SchemaQueryBaseColumn
    {
        public const string Сортировка_category = " Сортировка";
        public const string Видимость_category = " Видимость";

        private QueryOrderBy orderBy;
        [DisplayName("  Сортировка"), Description("Порядок сортировки"), Category(Сортировка_category)]
        public QueryOrderBy OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; firePropertyChanged("OrderBy"); }
        }

        private bool hidden;
        [DisplayName("Скрытая"), Description("Признак скрытой колонки"), Category(Видимость_category)]
        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; firePropertyChanged("Hidden"); }
        }

    }

}
