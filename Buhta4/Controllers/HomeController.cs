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

        //public ActionResult SchemaTableDesigner(string ID, string mode = "edit")
        //{
        //    var model = new SchemaTableDesignerModel(this, null);
        //    if (mode == "edit")
        //        App.Schema.ReloadObjectCache(Guid.Parse(ID));
        //    model.EditedObject = App.Schema.GetObject<SchemaTable>(Guid.Parse(ID));
        //    model.StartEditing();
        //    return View(@"~\MODULES\BUHTA\CORE\SCHEMA\SCHEMA-TABLE\SchemaTableDesignerView.cshtml", model);
        //}


        //public ActionResult SchemaDesigner()
        //{
        //    var model = new SchemaDesignerModel(this, null);
        //    return View(@"~\MODULES\BUHTA\CORE\SCHEMA\DESIGNER\SchemaDesignerView.cshtml", model);

        //}

        public ActionResult Index()
        {

            var model = new HomePageModel(this, null);
            return View(@"~\MODULES\BUHTA\CORE\HOME-PAGE\HomePageView.cshtml", model);
        }


    }
}