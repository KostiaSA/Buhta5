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
            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\Index.cshtml", new BuhtaSchemaDesignerModel(this));
        }

        //public ActionResult SchemaTableDesigner()
        //{

        //    return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\SchemaTableDesigner.cshtml", new BuhtaSchemaDesignerModel(this));
        //}

        public ActionResult SchemaTableDesigner(string ID)
        {
            var model = new SchemaTableDesignerModel(this);
            model.EditedObject = App.Schema.GetObject<SchemaTable>(Guid.Parse(ID));

            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\SchemaTableDesigner.cshtml", model);
        }

    }
}