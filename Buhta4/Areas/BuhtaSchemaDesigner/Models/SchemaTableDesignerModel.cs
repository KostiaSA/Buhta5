using System;
using System.Collections.Generic;
using System.Linq;
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

        public void CloseColumnEditor(dynamic args)
        {
            Table.Name = "закрыто";
        }
         
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

        void EditColumn(string columnName)
        {
            var model = new SchemaTableColumnEditModel(Controller, this);
            model.IsInsertMode = false;
            model.EditedObject = Table.GetColumnByName(columnName);
            model.StartEditing();
            var modal = CreateModal(@"~\Areas\BuhtaCore\Views\SchemaTableColumnEditDialog.cshtml", model);
            //modal.OnClose_Bind = nameof(CloseColumnEditor);
            modal.Show();
        }

        void AddColumn()
        {
            //var model = new SchemaTableColumnEditModel(Controller, this);
            //model.IsInsertMode = true;
            //model.EditedObject = Table.GetColumnByName(columnName);
            //model.StartEditing();
            //var modal = CreateModal(@"~\Areas\BuhtaCore\Views\SchemaTableColumnEditDialog.cshtml", model);
            //modal.OnClose_Bind = nameof(CloseColumnEditor);
            //modal.Show();
        }

        public override void StartEditing()
        {
            if (EditedObject != null)
            {
                EditedObject.StartEditing();
            }
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