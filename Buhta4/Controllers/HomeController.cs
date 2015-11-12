using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application 1 description page.";

            TestClass1.Test1();
            TestClass1.Test2();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }


        SchemaTable OrgTable;

        public ActionResult EditTable()
        {
            if (OrgTable == null)
            {
                OrgTable = new SchemaTable();
                OrgTable.ID = Guid.NewGuid();
                OrgTable.Name = @"Организация";
                OrgTable.Description = "Справочник организаций и контрагентов";

                SchemaTableColumn col;

                col = new SchemaTableColumn(); col.Table = OrgTable; OrgTable.Columns.Add(col);
                col.Name = "Номер";
                col.Description = "Номер оганизации";
                col.Position = 1;

                col = new SchemaTableColumn(); col.Table = OrgTable; OrgTable.Columns.Add(col);
                col.Name = "Название";
                col.Description = "Название оганизации";
                col.Position = 2;

                col = new SchemaTableColumn(); col.Table = OrgTable; OrgTable.Columns.Add(col);
                col.Name = "Город";
                col.Description = "Родной город оганизации";
                col.Position = 1010;


            }
            var model = new SchemaTableEditModel();
            model.Controller = this;
            model.EditedObject = OrgTable;
            return View(model);
        }

        private void X_OnChange(xInput sender, string newValue)
        {
            throw new NotImplementedException();
        }
    }
}