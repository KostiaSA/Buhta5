using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var model = new SchemaTableColumnEditModel(this, null);
            //model.Column = new SchemaTableColumn() { Name = "это жопа1" };
//            return View();
            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\Index.cshtml", new BuhtaSchemaDesignerModel(this, null));

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
            return View();
        }

    }
}