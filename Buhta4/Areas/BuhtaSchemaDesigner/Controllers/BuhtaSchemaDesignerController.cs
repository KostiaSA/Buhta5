using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class BuhtaSchemaDesignerController : Controller
    {

        // GET: BuhtaSchemaDesigner/BuhtaSchemaDesigner
        public ActionResult Index()
        {
            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\Index.cshtml", new BuhtaSchemaDesignerModel(this, null));
        }

        //public ActionResult SchemaTableDesigner()
        //{

        //    return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\SchemaTableDesigner.cshtml", new BuhtaSchemaDesignerModel(this));
        //}

        public ActionResult SchemaTableDesigner(string ID)
        {
            var model = new SchemaTableDesignerModel(this, null);
            App.Schema.ReloadObjectCache(Guid.Parse(ID));
            model.EditedObject = App.Schema.GetObject<SchemaTable>(Guid.Parse(ID));
            model.StartEditing();

            return View(@"~\Areas\BuhtaSchemaDesigner\Views\BuhtaSchemaDesigner\SchemaTableDesigner.cshtml", model);
        }

    }
}