using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class SchemaTableColumnEditModel : BaseModel
    {
        public SchemaTableColumn Column { get; set; }

        public void SaveButtonClick(dynamic args)
        {

            //Table.Name = "Жопа";

            //SchemaTableColumn col;

            //col = new SchemaTableColumn(); col.Table = Table; Table.Columns.Add(col);
            //col.Name = "новая колонка";
            //col.Description = "давай!";


            //UpdateCollection(nameof(Table) + "." + nameof(Table.Columns));


        }
    }
}