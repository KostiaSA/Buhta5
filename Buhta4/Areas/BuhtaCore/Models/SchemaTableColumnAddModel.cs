using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableColumnAddModel : BaseEditFormModel
    {
        public SchemaTableColumn Column { get; set; }

        public SchemaTableColumnAddModel(Controller controller, BaseModel parentModel, SchemaTable table) : base(controller, parentModel) {
            Column = new SchemaTableColumn();
            Column.Position = table.GetMaxColumnPosition()+1;
            Column.Table = table;
            Column.Name = "";
            //c.ColumnRoles.Add(newRole.ID);
            //c.DataType = newDataType.Clone();// (SqlDataType)Activator.CreateInstance(newDataType.GetType());
            //c.DataType.Column = c;
            //EditedTable.Columns.Add(c);
            //    ColumnsGrid.DataSource = null;
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            Modal.Close();
            if (ParentModel != null)
            {
                ParentModel.Update(true);
                (ParentModel as SchemaTableDesignerModel).SelectedColumnByColumnName(Column.Name);
            }
        }

        public override void CancelChanges()
        {
            Modal.Close();
            //if (ParentModel != null)
            //{
            //    ParentModel.Update(true);
            //    (ParentModel as SchemaTableDesignerModel).SelectedColumnByColumnName(Column.Name);
            //}
        }


        //public string GetColumnDataTypeInputsHtml()
        //{
        //    var html = new StringBuilder();

        //    var dt = new bsInput(this);
        //    dt.Label = "Тип данных";
        //    dt.AddAttr("disabled", "disabled");
        //    dt.AddStyle("max-width", "350px");
        //    dt.Type = bsInputType.Text;
        //    dt.Bind_Value_To_String(nameof(Column) + "." + nameof(Column.DataType) + "." + nameof(Column.DataType.GetNameDisplay));
        //    html.Append(dt.GetHtml());

        //    if (Column.DataType.GetType() == typeof(StringDataType))
        //    {

        //        var size = new bsInput(this);
        //        size.Label = "Максимальная длина";
        //        size.AddStyle("max-width", "80px");
        //        size.Type = bsInputType.Text;
        //        size.Bind_Value_To_String(nameof(Column) + "." + nameof(Column.DataType) + "." + nameof(StringDataType.MaxSize));
        //        html.Append(size.GetHtml());
        //    }

        //    return html.ToString();
        //}
    }
}