using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableColumnAddDialogModel : SchemaTableColumnBaseModel
    {

        //public override bool GetNeedSave()
        //{
        //    if (Column != null)
        //        return Column.GetNeedSave();
        //    else
        //        return false;
        //}


        public SchemaTableColumnAddDialogModel(Controller controller, BaseModel parentModel, SchemaTable table) : base(controller, parentModel)
        {
            EditedColumnDataTypes = new List<SqlDataType>();

            Column = new SchemaTableColumn();
            Column.Position = table.GetMaxColumnPosition() + 1;
            Column.Table = table;
            Column.DataType = new GuidDataType() { Column = Column };
            Column.Name = "";

            EditedObject = Column;
        }


        public override void SaveChanges()
        {

            ActivateAllValidatorBinders();

            var errors = new ValidateErrorList();
            Column.Validate(errors);
            if (!errors.IsEmpty)
            {
                ShowErrorMessageDialog("Есть ошибки", "@" + errors.ToHtmlString());
            }
            else
            {
                //var newDataType = EditedColumnDataTypes.Find((dt) => dt.Name == EditedColumnDataTypeName);
                //Column.DataType = newDataType.Clone();// (SqlDataType)Activator.CreateInstance(newDataType.GetType());
                //Column.DataType.Column = Column;

                Column.Table.Columns.Add(Column);

                base.SaveChanges();
                Modal.Close();
                if (ParentModel != null)
                {
                    ParentModel.Update(true);
                    (ParentModel as SchemaTableDesignerModel).SelectedColumnByColumnName(Column.Name);
                }
            }
        }

        public void OnChangeRoles(dynamic args)
        {
            var newRoleID = Column.ColumnRoles.First();
            var newRole = SchemaBaseRole.Roles[newRoleID] as Колонка_ColumnRole;

            EditedColumnDataTypes.Clear();

            SqlDataType newDataType = null;
            //string newColumName = "Новая колонка";
            if (newRole.AllowedDataTypes.Count > 0)
            {
                foreach (var dataType in newRole.AllowedDataTypes.Values)
                    EditedColumnDataTypes.Add(dataType);

                foreach (var keyVP in newRole.AllowedDataTypes)
                    if (keyVP.Value == newDataType)
                    {
                        if (string.IsNullOrWhiteSpace(Column.Name))
                            Column.Name = keyVP.Key;
                        break;
                    }

                newDataType = newRole.DataType;
            }
            else
            {
                newDataType = newRole.DataType;
                EditedColumnDataTypes.Add(newDataType);
            }

            if (string.IsNullOrWhiteSpace(Column.Name))
                Column.Name = newRole.NewColumnName;

            setEditedColumnDataTypeName(newDataType.Name);
            UpdateDatasets();
        }


        public override void CancelChanges()
        {
            if (GetNeedSave())
            {
                ShowLostChangesConfirmationDialog((args => { Modal.Close(); }));
            }
            else
                Modal.Close();
        }


    }
}