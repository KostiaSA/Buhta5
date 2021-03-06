﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableColumnBaseModel : BaseEditFormModel
    {
        public virtual SchemaTableColumn Column { get; set; }

        public string EditedColumnDataTypeName { get; set; }
        public List<SqlDataType> EditedColumnDataTypes { get; set; }

        public SchemaTableColumnBaseModel(Controller controller, BaseModel parentModel) : base(controller, parentModel)
        {
            EditedColumnDataTypes = new List<SqlDataType>();
        }


        StringDataType fakeStringDataType;
        public StringDataType ColumnStringDataType
        {
            get
            {
                if (Column != null && Column.DataType is StringDataType)
                    fakeStringDataType = (Column.DataType as StringDataType);
                else
                {
                    if (fakeStringDataType == null)
                        fakeStringDataType = new StringDataType() { Column = Column };
                }
                return fakeStringDataType;
            }
        }

        ForeingKeyDataType fakeForeingKeyDataType;
        public ForeingKeyDataType ColumnForeingKeyDataType
        {
            get
            {
                if (Column != null && Column.DataType is ForeingKeyDataType)
                    fakeForeingKeyDataType = (Column.DataType as ForeingKeyDataType);
                else
                {
                    if (fakeForeingKeyDataType == null)
                        fakeForeingKeyDataType = new ForeingKeyDataType() { Column = Column };
                }
                return fakeForeingKeyDataType;
            }
        }


        string getEditedColumnDataTypeName()
        {
            if (string.IsNullOrWhiteSpace(EditedColumnDataTypeName) && Column != null && Column.DataType != null)
                EditedColumnDataTypeName = Column.DataType.Name;
            return EditedColumnDataTypeName;
        }


        protected void setEditedColumnDataTypeName(string newDataTypeName)
        {
            if (string.IsNullOrWhiteSpace(EditedColumnDataTypeName) || (Column.DataType.Name != newDataTypeName))
            {
                EditedColumnDataTypeName = newDataTypeName;

                if (newDataTypeName == ColumnStringDataType.Name)
                    Column.DataType = ColumnStringDataType;
                else
                {
                    var newDataType = EditedColumnDataTypes.Find((dt) => dt.Name == EditedColumnDataTypeName);
                    Column.DataType = newDataType.Clone();// (SqlDataType)Activator.CreateInstance(newDataType.GetType());
                }
                Column.DataType.Column = Column;
            }

        }

        public MvcHtmlString GetColumnDataTypeInputsHtml()
        {
            //var script = new StringBuilder();
            var html = new StringBuilder();

            var dataTypeTag = new bsSelect(this);
            dataTypeTag.IsRequired = this is SchemaTableColumnAddDialogModel;

            dataTypeTag.Bind_Disabled(() => this is SchemaTableColumnEditDialogModel || EditedColumnDataTypes.Count <= 1);
            dataTypeTag.Bind_Options_To_ObjectsList(nameof(EditedColumnDataTypes), "Name", "Name", "Name");
            //            dataTypeTag.Bind_Value<string>(nameof(EditedColumnDataTypeName));
            dataTypeTag.Bind_Value<string>(getEditedColumnDataTypeName, setEditedColumnDataTypeName);
            dataTypeTag.MaxWidth = 300;
            dataTypeTag.Label = "Тип данных";


            html.Append(dataTypeTag.GetHtml());

            var stringSizeTag = new bsInput(this);
            stringSizeTag.IsRequired = this is SchemaTableColumnAddDialogModel;
            stringSizeTag.Label = "Максимальная длина";
            stringSizeTag.AddStyle("max-width", "80px");
            stringSizeTag.Type = bsInputType.Text;
            stringSizeTag.Bind_Value<string>(nameof(ColumnStringDataType) + "." + nameof(StringDataType.MaxSize));
            stringSizeTag.Bind_Visible(() => Column.DataType.GetType() == typeof(StringDataType));
            html.Append(stringSizeTag.GetHtml());

            var refTableTag = new bsInput(this);
            refTableTag.IsRequired = true;
            refTableTag.Bind_Value_To_SchemaObject(nameof(ColumnForeingKeyDataType) + "." + nameof(ColumnForeingKeyDataType.RefTableID), typeof(SchemaTable));
            refTableTag.Label = "Ссылка на таблицу";
            refTableTag.Bind_Visible(() => Column.DataType.GetType() == typeof(ForeingKeyDataType));
            html.Append(refTableTag.GetHtml());


            return new MvcHtmlString(html.ToString());
        }

    }
}