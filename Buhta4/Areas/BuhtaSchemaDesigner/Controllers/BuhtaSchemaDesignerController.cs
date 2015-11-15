using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class BuhtaSchemaDesignerController : Controller
    {
        // GET: BuhtaSchemaDesigner/BuhtaSchemaDesigner
        public ActionResult Index()
        {
            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\Index.cshtml", new BuhtaSchemaDesignerModel());
        }
    }
}