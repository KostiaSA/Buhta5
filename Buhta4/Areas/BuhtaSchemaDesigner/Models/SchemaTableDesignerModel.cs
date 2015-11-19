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
        public SchemaTable Table { get { return EditedObject; } }

        public SchemaTableDesignerModel(Controller controller) : base(controller) { }

        public void CloseColumnEditor(string chromeWindowId, dynamic args)
        {
            Table.Name = "закрыто";
        }

        public void EditFirstColumnButtonClick(string chromeWindowId, dynamic args)
        {
            var model = new SchemaTableColumnEditModel(Controller);
            model.Column = Table.Columns[0];

            //var xx = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает


            var win = CreateWindow(chromeWindowId, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model);
            win.OnClose_Bind = nameof(CloseColumnEditor);
            win.Show();
            //ShowWindow(@"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
        }

        public void Test1ButtonClick(string chromeWindowId, dynamic args)
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