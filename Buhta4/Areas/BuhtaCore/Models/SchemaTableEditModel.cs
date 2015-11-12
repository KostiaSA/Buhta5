﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class SchemaTableEditModel : SchemaObjectEditModel<SchemaTable>
    {
        public SchemaTable Table { get { return EditedObject; } }

        public void EditFirstColumnButtonClick(dynamic args)
        {
            var model = new SchemaTableColumnEditModel();
            model.Column = Table.Columns[0];

            var xx = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
            ShowWindow(@"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
        }

        public void Test1ButtonClick(dynamic args)
        {
            Table.Name = "Жопа";

            SchemaTableColumn col;

            col = new SchemaTableColumn(); col.Table = Table; Table.Columns.Add(col);
            col.Name = "новая колонка";
            col.Description = "давай!";


            UpdateCollection(nameof(Table) + "." + nameof(Table.Columns));

            //var arr = new List<string[]>();
            //arr.Add(new string[] { "бухта"," воронеж"});
            //arr.Add(new string[] { "бухта-сбп", "питер" });
            //arr.Add(new string[] { "дом", "москва","париж" });

            //Hub.Clients.Group(BindingId).receiveBindedValuesChanged(BindingId, arr);

        }
    }
}