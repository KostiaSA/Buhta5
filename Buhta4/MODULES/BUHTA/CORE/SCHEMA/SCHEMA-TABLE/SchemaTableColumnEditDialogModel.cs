using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableColumnEditDialogModel : SchemaTableColumnBaseModel
    {
        public override SchemaTableColumn Column { get { return (SchemaTableColumn)EditedObject; } set { } }

        public override string PageTitle { get { return "Колонка: " + Column.Name; } }

        public SchemaTableColumnEditDialogModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        public override void StartEditing()
        {
            base.StartEditing();
            EditedColumnDataTypes.Add(Column.DataType);
            EditedColumnDataTypeName = Column.DataType.Name;

        }

        public override void SaveChanges()
        {
            ActivateAllValidatorBinders();

            var errors = new ValidateErrorList();
            Column.Validate(errors);
            if (!errors.IsEmpty)
            {
                ShowErrorMessageDialog("Есть ошибки", errors.ToHtmlString());
            }
            else
            {
                base.SaveChanges();
                Modal.Close();
                if (ParentModel != null)
                {
                    ParentModel.Update(true);
                    (ParentModel as SchemaTableDesignerModel).SelectedColumnByColumnName(Column.Name);
                }
            }
        }

        public override void CancelChanges()
        {
            if (GetNeedSave())
            {
                ShowLostChangesConfirmationDialog(CancelAndClose);
            }
            else
                CancelAndClose(null);
        }

        public void CancelAndClose(dynamic args)
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


    }
}