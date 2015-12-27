using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaQueryDesignerModel : SchemaObjectEditModel<SchemaQuery>
    {

        public SchemaQuery Query { get { return (SchemaQuery)EditedObject; } }

        public SchemaQueryDesignerModel(Controller controller, BaseModel parentModel) : base(controller, parentModel)
        {
            SqlText = "";
        }

        public bsTree ColumnsTree;

        public string SelectedColumnName;

        public void SelectedColumnByColumnName(string columnName)
        {
            SelectedColumnName = columnName;
            //  ColumnsTree.SelectRowById(columnName);
        }

        class columnNode
        {
            public columnNode()
            {
                ParentID = "";
                Alias = "";
            }
            public string ID { get; set; }
            public string ParentID { get; set; }
            public string Title { get; set; }
            public string Alias { get; set; }

        }

        public List<Object> ColumnsList
        {
            get
            {
                var list = new List<Object>();
                Query.RootColumn.Name = "*root*";
                foreach (SchemaQueryBaseColumn col in Query.RootColumn.GetAllColumns())
                {
                    var node = new columnNode();
                    node.ID = col.GetFullName();
                    node.ParentID = col.ParentColumn.GetFullName();
                    node.Title = col.GetQueryDesignerDisplayName();
                    if (col is SchemaQueryJoinColumn)
                        node.Alias = "";
                    else
                        node.Alias = col.Alias ?? col.Name;
                    list.Add(node);

                }

                // добавляем root
                list.Add(new columnNode()
                {
                    ID = Query.RootColumn.GetFullName(),
                    Title = Query.RootColumn.GetQueryDesignerDisplayName(),
                });


                return list;
            }
        }


        //public void ColumnGridRowDblClick(dynamic args)
        //{
        //    EditColumn(args.rowId.Value);
        //}

        //public void EditColumnButtonClick(dynamic args)
        //{
        //    if (!String.IsNullOrEmpty(SelectedColumnName))
        //        EditColumn(SelectedColumnName);
        //}

        //public void DeleteColumnButtonClick(dynamic args)
        //{
        //    if (!string.IsNullOrWhiteSpace(SelectedColumnName))
        //        ShowDeleteConfirmationMessageDialog("Удаление", "Удалить колонку '" + SelectedColumnName + "'?", DeleteColumn);
        //}

        //public void DeleteColumn(dynamic args)
        //{

        //    var column = Query.GetColumnByName(SelectedColumnName);
        //    Query.Columns.Remove(column);
        //    SelectedColumnName = "";
        //    UpdateDatasets();
        //}

        //public void AddColumnButtonClick(dynamic args)
        //{
        //    AddColumn();
        //}

        //void EditColumn(string columnName)
        //{
        //    var model = new SchemaQueryColumnEditDialogModel(Controller, this);
        //    model.IsInsertMode = false;
        //    model.EditedObject = Query.GetColumnByName(columnName);
        //    model.StartEditing();
        //    var modal = CreateModal(@"~\MODULES\BUHTA\CORE\SCHEMA\SCHEMA-Query\SchemaQueryColumnEditDialogView.cshtml", model);
        //    modal.Show();
        //}

        //void AddColumn()
        //{
        //    var model = new SchemaQueryColumnAddDialogModel(Controller, this, Query);
        //    model.IsInsertMode = true;
        //    model.StartEditing();
        //    var modal = CreateModal(@"~\MODULES\BUHTA\CORE\SCHEMA\SCHEMA-Query\SchemaQueryColumnAddDialogView.cshtml", model);
        //    modal.Show();
        //}

        public override void StartEditing()
        {
            if (EditedObject != null)
            {
                EditedObject.StartEditing();
            }
        }

        public string SqlText { get; set; }

        public void SqlTabClick(dynamic args)
        {
            SqlText = Query.GetSqlText(null);
        }

    }
}