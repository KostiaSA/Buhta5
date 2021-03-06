﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaTableDesignerModel : SchemaObjectEditModel<SchemaTable>
    {

        public SchemaTable Table { get { return (SchemaTable)EditedObject; } }

        public SchemaTableDesignerModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        public bsGrid ColumnsGrid;

        public string SelectedColumnName;

        public void SelectedColumnByColumnName(string columnName)
        {
            SelectedColumnName = columnName;
            ColumnsGrid.SelectRowById(columnName);
        }

        public void ColumnGridRowDblClick(dynamic args)
        {
            EditColumn(args.rowId.Value);
        }

        public void EditColumnButtonClick(dynamic args)
        {
            if (!String.IsNullOrEmpty(SelectedColumnName))
                EditColumn(SelectedColumnName);
        }

        public void DeleteColumnButtonClick(dynamic args)
        {
            if (!string.IsNullOrWhiteSpace(SelectedColumnName))
                ShowDeleteConfirmationMessageDialog("Удаление", "Удалить колонку '" + SelectedColumnName + "'?", DeleteColumn);
        }

        public void DeleteColumn(dynamic args)
        {

            var column = Table.GetColumnByName(SelectedColumnName);
            Table.Columns.Remove(column);
            SelectedColumnName = "";
            UpdateDatasets();
        }

        public void AddColumnButtonClick(dynamic args)
        {
            AddColumn();
        }

        void EditColumn(string columnName)
        {
            var model = new SchemaTableColumnEditDialogModel(Controller, this);
            model.IsInsertMode = false;
            model.EditedObject = Table.GetColumnByName(columnName);
            model.StartEditing();
            var modal = CreateModal(@"~\MODULES\BUHTA\CORE\SCHEMA\SCHEMA-TABLE\SchemaTableColumnEditDialogView.cshtml", model);
            modal.Show();
        }

        void AddColumn()
        {
            var model = new SchemaTableColumnAddDialogModel(Controller, this, Table);
            model.IsInsertMode = true;
            model.StartEditing();
            var modal = CreateModal(@"~\MODULES\BUHTA\CORE\SCHEMA\SCHEMA-TABLE\SchemaTableColumnAddDialogView.cshtml", model);
            modal.Show();
        }

        public override void StartEditing()
        {
            if (EditedObject != null)
            {
                EditedObject.StartEditing();
            }
        }

        public void SqlSyncButtonClick(dynamic args)
        {
            var validateError = new ValidateErrorList();
            Table.Validate(validateError);
            if (!validateError.IsEmpty)
            {
                ShowErrorMessageDialog("синхронизация с SQL", validateError.ToString());
                return;
            }

            Table.GetLogTable();
            App.Schema.SaveObject(Table);
            App.Schema.SqlDB.SynchronizeTable(Table);

            if (Table.IsUserEditable && !Table.TableRoles.Contains(RoleConst.ВложеннаяТаблица) && !Table.TableRoles.Contains(RoleConst.Регистр))
                App.Schema.SqlDB.SynchronizeTable(Table.GetLogTable());

            if (Table.IsProvodkaOwner)
                App.Schema.SqlDB.SynchronizeTable(Table.GetProvodkasTable());

            ShowInfoMessageDialog("синхронизация с SQL", "Синхронизация выполнена успешно");
        }

        public void Test1ButtonClick(dynamic args)
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 1; i < 1000000; i++)
                {
                    Table.Name = "Жопа-" + i;
                    Update();
                    Thread.Sleep(1);
                }
            });

            Table.Name = "Жопа99";

            //SchemaTableColumn col;

            //col = new SchemaTableColumn(); col.Table = Table; Table.Columns.Add(col);
            //col.Name = "новая колонка";
            //col.Description = "давай!";


            //UpdateCollection(nameof(Table) + "." + nameof(Table.Columns));

        }
    }
}