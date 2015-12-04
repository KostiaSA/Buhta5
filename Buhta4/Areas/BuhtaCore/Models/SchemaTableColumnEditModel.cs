using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableColumnEditModel : BaseEditFormModel
    {
        public SchemaTableColumn Column { get { return (SchemaTableColumn)EditedObject; } }

        public override string PageTitle { get { return "Колонка: " + Column.Name; } }

        public SchemaTableColumnEditModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

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
            var table = Column.Table;
            var colIndex = table.Columns.IndexOf(Column);
            var restoredColumn = (SchemaTableColumn)SavedEditedObject;
            restoredColumn.Table = table;
            table.Columns[colIndex] = restoredColumn;
            restoredColumn.CancelChanges();
            Modal.Close();
            if (ParentModel != null)
            {
                ParentModel.Update(true);
                (ParentModel as SchemaTableDesignerModel).SelectedColumnByColumnName(Column.Name);
            }
        }


        public string GetColumnDataTypeInputsHtml()
        {
            var html = new StringBuilder();

            var dt = new bsInput(this);
            dt.Label = "Тип данных";
            dt.AddAttr("disabled", "disabled");
            dt.AddStyle("max-width", "350px");
            dt.Type = bsInputType.Text;
            dt.Bind_Value<string>(nameof(Column) + "." + nameof(Column.DataType) + "." + nameof(Column.DataType.GetNameDisplay));
            html.Append(dt.GetHtml());

            if (Column.DataType.GetType() == typeof(StringDataType))
            {

                var size = new bsInput(this);
                size.Label = "Максимальная длина";
                size.AddStyle("max-width", "80px");
                size.Type = bsInputType.Text;
                size.Bind_Value<string>(nameof(Column) + "." + nameof(Column.DataType) + "." + nameof(StringDataType.MaxSize));
                html.Append(size.GetHtml());
            }

            return html.ToString();
        }
    }
}