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

        public SchemaTableColumnAddModel(Controller controller, BaseModel parentModel, SchemaTable table) : base(controller, parentModel)
        {
            NewColumnDataTypes = new List<SqlDataType>();

            Column = new SchemaTableColumn();
            Column.Position = table.GetMaxColumnPosition() + 1;
            Column.Table = table;
            Column.Name = "";
            //c.ColumnRoles.Add(newRole.ID);
            //c.DataType = newDataType.Clone();// (SqlDataType)Activator.CreateInstance(newDataType.GetType());
            //c.DataType.Column = c;
            //EditedTable.Columns.Add(c);
            //    ColumnsGrid.DataSource = null;
        }

        public string NewColumnDataTypeName { get; set; }
        public List<SqlDataType> NewColumnDataTypes { get; set; }

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

        public void OnChangeRoles(dynamic args)
        {
            var newRoleID = Column.ColumnRoles.First();
            var newRole = SchemaBaseRole.Roles[newRoleID] as Колонка_ColumnRole;

            NewColumnDataTypes.Clear();

            SqlDataType newDataType = null;
            //string newColumName = "Новая колонка";
            if (newRole.AllowedDataTypes.Count > 0)
            {
                foreach (var dataType in newRole.AllowedDataTypes.Values)
                    NewColumnDataTypes.Add(dataType);

                foreach (var keyVP in newRole.AllowedDataTypes)
                    if (keyVP.Value == newDataType)
                    {
                        if (string.IsNullOrWhiteSpace(Column.Name))
                            Column.Name = keyVP.Key;
                        break;
                    }

                newDataType = newRole.DataType;
                NewColumnDataTypes.First();
            }
            else
            {
                newDataType = newRole.DataType;
                NewColumnDataTypes.Add(newDataType);
            }

            NewColumnDataTypeName = newDataType.Name;
            if (string.IsNullOrWhiteSpace(Column.Name))
                Column.Name = newRole.NewColumnName;
            UpdateDatasets();

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